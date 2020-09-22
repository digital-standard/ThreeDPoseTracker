using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.Barracuda;
using System.Text;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;

//https://www.codeproject.com/Articles/865935/Object-Tracking-Kalman-Filter-with-Ease
using ModelState = ConstantVelocity3DModel;

public class VNectBarracudaRunner : MonoBehaviour
{
    public NNModel NNModel;
    public WorkerFactory.Type WorkerType = WorkerFactory.Type.Auto;
    public bool Verbose = false;

    public VNectModel VNectModel;
    public VideoCapture videoCapture;

    private Model _model;
    private IWorker _worker;
    private VNectModel.JointPoint[] jointPoints;
    private const int JointNum = 24;
    private const int JointNum_Squared = JointNum * 2;
    private const int JointNum_Cube = JointNum * 3;

    public int InputImageSize;
    private float InputImageSizeF;
    private float InputImageSizeHalf;
    public int HeatMapCol;
    public int HeatMapCol_Half;
    private int HeatMapCol_Squared;
    private int HeatMapCol_Cube;
    private int HeatMapCol_JointNum;
    private float[] heatMap2D;
    private float[] offset2D;
    private float[] heatMap3D;
    private float[] offset3D;
    private float unit;

    private int cubeOffsetLinear;
    private int cubeOffsetSquared;

    private bool Lock = true;
    private float waitSec = 1f / 30f;

    private float elapsedMeasurementSec = 0f;
    private float fpsMeasurementSec = 0f;
    public float FPS = 0f;
    private int fpsCounter = 0;

    public bool UseLPF;
    public float Smooth;
    public bool UseKalmanF;
    public float KalmanParamQ;
    public float KalmanParamR;
    public float ForwardThreshold;
    public float BackwardThreshold;
    public int NOrderLPF;
    private List<FIRFilter> filter = new List<FIRFilter>();
    private FilterWindow filterWindow = new FilterWindow();
    private List<KalmanFilter> kalmanFilter = new List<KalmanFilter>();
    [Range(0.0f, 1.0f)] public double FilterTimeInterval = 0.45;
    [Range(0.0f, 1.0f)] public double FilterNoise = 0.4;

    private delegate void UpdateVNectModelDelegate();
    private UpdateVNectModelDelegate UpdateVNectModel;
    public int ModelQuality = 1;
    private string HighQualityModelName = "HighQualityTrainedModel.nn";

    [SerializeField]
    private float EstimatedScore;

    public bool DebugMode;
    public bool User3Input;
    public bool UpperBodyMode;

    StreamWriter writer;

    private void Start()
    {

        // デバッグ用ファイルを開く
        //Encoding enc = Encoding.GetEncoding("Shift_JIS");
        //var csvPath = System.IO.Path.Combine(Application.streamingAssetsPath, "data.csv");
        //writer = new StreamWriter(csvPath, false, enc);

        if (DebugMode)
        {
            if (User3Input)
            {
                UpdateVNectModel = new UpdateVNectModelDelegate(UpdateVNectAsync);
            }
            else
            {
                UpdateVNectModel = new UpdateVNectModelDelegate(UpdateVNect);
            }
            /*
            var streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, HighQualityModelName);
            var writer = new BinaryWriter(new FileStream(streamingPath, FileMode.Create));
            writer.Write(NNModel.modelData.Value);
            writer.Close();
            */
            _model = ModelLoader.Load(NNModel, Verbose);
        }
        else
        {
            var streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, HighQualityModelName);
            if (!File.Exists(streamingPath))
            {
                ModelQuality = 0;
            }

            if (ModelQuality == 0)
            {
                InputImageSize = 224;
                HeatMapCol = 14;
                User3Input = false;
                UpdateVNectModel = new UpdateVNectModelDelegate(UpdateVNect);
                _model = ModelLoader.Load(NNModel, Verbose);
            }
            else
            {
                InputImageSize = 448;
                HeatMapCol = 28;
                User3Input = true;
                UpdateVNectModel = new UpdateVNectModelDelegate(UpdateVNectAsync);
                _model = ModelLoader.LoadFromStreamingAssets(streamingPath);
            }
        }

        // Init VideoCapture
        videoCapture.Init(InputImageSize, InputImageSize);
        videoCapture.VideoReady += videoCapture_VideoReady;

        HeatMapCol_Half = HeatMapCol / 2;
        HeatMapCol_Squared = HeatMapCol * HeatMapCol;
        HeatMapCol_Cube = HeatMapCol * HeatMapCol * HeatMapCol;
        HeatMapCol_JointNum = HeatMapCol*JointNum;
        heatMap2D = new float[JointNum * HeatMapCol_Squared];
        offset2D = new float[JointNum * HeatMapCol_Squared * 2];
        heatMap3D = new float[JointNum * HeatMapCol_Cube];
        offset3D = new float[JointNum * HeatMapCol_Cube * 3];
        InputImageSizeF = InputImageSize ;
        InputImageSizeHalf = InputImageSizeF / 2f ;
        unit = 1f / (float)HeatMapCol;

        cubeOffsetLinear = HeatMapCol * JointNum_Cube;
        cubeOffsetSquared = HeatMapCol_Squared * JointNum_Cube;

        // Disabel sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _worker = WorkerFactory.CreateWorker(WorkerType, _model, Verbose);
        StartCoroutine("WaitLoad");

        var texture = new RenderTexture(InputImageSize, InputImageSize, 0, RenderTextureFormat.RGB565, RenderTextureReadWrite.sRGB)
        {
            useMipMap = false,
            autoGenerateMips = false,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };

        if (User3Input)
        {
            inputs[inputName_1] = new Tensor(texture, 3);
            inputs[inputName_2] = new Tensor(texture, 3);
            inputs[inputName_3] = new Tensor(texture, 3);
            _worker.Execute(inputs);
            inputs[inputName_1].Dispose();
            inputs[inputName_2].Dispose();
            inputs[inputName_3].Dispose();
        }
        else
        {
            input = new Tensor(texture, 3);
            _worker.Execute(input);
            input.Dispose();
        }
    }

    public void InitVNectModel(VNectModel avatar, ConfigurationSetting config)
    {
        VNectModel = avatar;
        jointPoints = VNectModel.Init(InputImageSize, config);

    }

    public void Exit()
    {
        Lock = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }

    public void SetVNectModel(VNectModel avatar)
    {
        VNectModel = avatar;
        jointPoints = avatar.JointPoints;

        VNectModel.Show();
    }

    public void VideoPlayStart(string path)
    {
        Lock = true;
        VNectModel.IsPoseUpdate = false;
        waitSec = 1f / videoCapture.SourceFps * 0.85f;

        StartCoroutine(videoCapture.VideoStart(path));
    }

    public void videoCapture_VideoReady()
    {
        Lock = false;
    }

    public void CameraPlayStart(int index)
    {
        Lock = false;
        waitSec = 1f / videoCapture.SourceFps * 0.85f;

        videoCapture.CameraPlayStart(index);
    }

    public void PlayStop()
    {
        Lock = true;
        videoCapture.PlayStop();
    }

    public void PlayPause()
    {
        Lock = true;
        videoCapture.Pause();
        //StartCoroutine(PlayPauseAsync());
    }
    public void Resume()
    {
        Lock = false;
        videoCapture.Resume();
    }
    /*
    private IEnumerator PlayPauseAsync()
    {
        yield return new WaitForSeconds(1f);
        videoCapture.Pause();
    }
    */
    public Vector3 GetHeadPosition()
    {
        return VNectModel.GetHeadPosition();
    }

    public void SetPredictSetting(ConfigurationSetting config)
    {
        Smooth = config.LowPassFilter;
        NOrderLPF = config.NOrderLPF;

        filterWindow.Init(config.FIROrderN03, config.FIRFromHz, config.FIRToHz, 30f);

        filter.Clear();
        for (var i = 0; i < JointNum; i++)
        {
            filter.Add(new FIRFilter(filterWindow, config.RangePathFilterBuffer03));
        }
        FilterTimeInterval = config.KalmanFilterInterval;
        FilterNoise = config.KalmanFilterNoise;
        kalmanFilter.Clear();
        for (var i = 0; i < JointNum; i++)
        {
            kalmanFilter.Add(new KalmanFilter(FilterTimeInterval, FilterNoise));
        }

        ForwardThreshold = config.ForwardThreshold;
        BackwardThreshold = config.BackwardThreshold;

        if(VNectModel != null)
        {
            VNectModel.SetPredictSetting(config);
        }
    }

    private void Update()
    {
        if (videoCapture.IsPlay())
        {
            var v = Time.deltaTime;
            elapsedMeasurementSec += v;
            fpsMeasurementSec += v;
            //if (elapsedMeasurementSec > waitSec)
            {
                UpdateVNectModel();

                if (fpsMeasurementSec >= 1f)
                {
                    FPS = (float)fpsCounter / fpsMeasurementSec;
                    fpsCounter = 0;
                    fpsMeasurementSec = 0f;
                }

                elapsedMeasurementSec = 0f;
            }
        }
    }

    private IEnumerator WaitLoad()
    {
        yield return new WaitForSeconds(10f);
        Lock = false;
    }

    private void UpdateVNect()
    {
        ExecuteModel();
        PredictPose();

        fpsCounter++;
    }

    private void ExecuteModel()
    {

        // Create input and Execute model
        input = new Tensor(videoCapture.MainTexture, 3);
        _worker.Execute(input);
        input.Dispose();

        // Get outputs
        for (var i = 2; i < _model.outputs.Count; i++)
        {
            b_outputs[i] = _worker.PeekOutput(_model.outputs[i]);
        }

        // Get data from outputs
        //heatMap2D = b_outputs[0].data.Download(b_outputs[0].shape);
        //offset2D = b_outputs[1].data.Download(b_outputs[1].shape);
        offset3D = b_outputs[2].data.Download(b_outputs[2].shape);
        heatMap3D = b_outputs[3].data.Download(b_outputs[3].shape);
    }

    private void UpdateVNectAsync()
    {
        input = new Tensor(videoCapture.MainTexture, 3);
        if (inputs[inputName_1] == null)
        {
            inputs[inputName_1] = input;
            inputs[inputName_2] = new Tensor(videoCapture.MainTexture, 3);
            inputs[inputName_3] = new Tensor(videoCapture.MainTexture, 3);
        }
        else
        {/*
            inputs[inputName_3].Dispose();
            inputs[inputName_1] = input;
            inputs[inputName_2] = input;
            inputs[inputName_3] = input;
            */
            /**/
             inputs[inputName_3].Dispose();
            
             inputs[inputName_3] = inputs[inputName_2];
              inputs[inputName_2] = inputs[inputName_1];
              inputs[inputName_1] = input;
              /**/
        }

        if (!Lock && videoCapture.IsPlay())
        {
            StartCoroutine(ExecuteModelAsync());
        }
    }
    
    private const string inputName_1 = "input.1";
    private const string inputName_2 = "input.4";
    private const string inputName_3 = "input.7";
    /*
    private const string inputName_1 = "input.1";
    private const string inputName_2 = "input.3";
    private const string inputName_3 = "input.5";
    */
    /*
    private const string inputName_1 = "0";
    private const string inputName_2 = "1";
    private const string inputName_3 = "2";
    */

    Tensor input = new Tensor();
    Dictionary<string, Tensor> inputs = new Dictionary<string, Tensor>() { { inputName_1, null }, { inputName_2, null }, { inputName_3, null }, };
    Tensor[] b_outputs = new Tensor[4];

    private IEnumerator ExecuteModelAsync()
    {
        if(Lock)
        {
            yield return null;
        }

        // Create input and Execute model
        yield return _worker.StartManualSchedule(inputs);

        if (!Lock)
        {
            // Get outputs
            for (var i = 2; i < _model.outputs.Count; i++)
            {
                b_outputs[i] = _worker.PeekOutput(_model.outputs[i]);
            }

            // Get data from outputs
            //heatMap2D = b_outputs[0].data.Download(b_outputs[0].shape);
            //offset2D = b_outputs[1].data.Download(b_outputs[1].shape);
            offset3D = b_outputs[2].data.Download(b_outputs[2].shape);
            heatMap3D = b_outputs[3].data.Download(b_outputs[3].shape);

            PredictPose();

            fpsCounter++;
        }
    }

    private void PredictPose()
    {
        var score = 0f;
        //var csv = videoCapture.VideoPlayer.frame.ToString();
        filterWindow.SetFps(FPS);

        for (var j = 0; j < JointNum; j++)
        {
            var jp = jointPoints[j];
            var maxXIndex = 0;
            var maxYIndex = 0;
            var maxZIndex = 0;
            jp.Score3D = 0.0f;
            var jj = j * HeatMapCol;

            for (var z = 0; z < HeatMapCol; z++)
            {
                var zz = jj + z;
                for (var y = 0; y < HeatMapCol; y++)
                {
                    var yy = y * HeatMapCol_Squared * JointNum + zz;
                    for (var x = 0; x < HeatMapCol; x++)
                    {
                        float v = heatMap3D[yy + x * HeatMapCol_JointNum];
                        if (v > jp.Score3D)
                        {
                            jp.Score3D = v;
                            maxXIndex = x;
                            maxYIndex = y;
                            maxZIndex = z;
                        }
                    }
                }
            }

            //jp.PrevNow3D = jp.Now3D;
            score += jp.Score3D;
            var yi = maxYIndex * cubeOffsetSquared + maxXIndex * cubeOffsetLinear;
            jp.Now3D.x = ((offset3D[yi + jj + maxZIndex] + 0.5f + (float)maxXIndex) / (float)HeatMapCol) * InputImageSizeF - InputImageSizeHalf;
            jp.Now3D.y = InputImageSizeF - ((offset3D[yi + (j + JointNum) * HeatMapCol + maxZIndex] + 0.5f + (float)maxYIndex) / (float)HeatMapCol) * InputImageSizeF - InputImageSizeHalf;
            jp.Now3D.z = ((offset3D[yi + (j + JointNum_Squared) * HeatMapCol + maxZIndex] + 0.5f + (float)(maxZIndex - HeatMapCol_Half)) / (float)HeatMapCol) * InputImageSizeF;
            (jp.Now3D.x, jp.Now3D.y, jp.Now3D.z) = filter[j].Add(jp.Now3D.x, jp.Now3D.y, jp.Now3D.z, FPS);
            jp.Now3D = kalmanFilter[j].CorrectAndPredict(jp);

        }
        //writer.WriteLine(csv);

        EstimatedScore = score / JointNum;

        // Calculate hip location
        var lc = (jointPoints[PositionIndex.rThighBend.Int()].Now3D + jointPoints[PositionIndex.lThighBend.Int()].Now3D) / 2f;
        jointPoints[PositionIndex.hip.Int()].Now3D = (jointPoints[PositionIndex.abdomenUpper.Int()].Now3D + lc) / 2f;
        // Calculate neck location
        jointPoints[PositionIndex.neck.Int()].Now3D = (jointPoints[PositionIndex.rShldrBend.Int()].Now3D + jointPoints[PositionIndex.lShldrBend.Int()].Now3D) / 2f;
        // Calculate head location
        var cEar = (jointPoints[PositionIndex.rEar.Int()].Now3D + jointPoints[PositionIndex.lEar.Int()].Now3D) / 2f;
        var hv = cEar - jointPoints[PositionIndex.neck.Int()].Now3D;
        var nhv = Vector3.Normalize(hv);
        var nv = jointPoints[PositionIndex.Nose.Int()].Now3D - jointPoints[PositionIndex.neck.Int()].Now3D;
        jointPoints[PositionIndex.head.Int()].Now3D = jointPoints[PositionIndex.neck.Int()].Now3D + nhv * Vector3.Dot(nhv, nv);
        // Calculate spine location
        jointPoints[PositionIndex.spine.Int()].Now3D = jointPoints[PositionIndex.abdomenUpper.Int()].Now3D;

        // Filters
        //

        var frwd = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Now3D, jointPoints[PositionIndex.lThighBend.Int()].Now3D, jointPoints[PositionIndex.rThighBend.Int()].Now3D);
        var frwdAngle = Vector3.Angle(frwd, Vector3.back);

        var jpindex = 0;
        foreach (var jp in jointPoints)
        {
            //KalmanUpdate(jp);
            jp.Pos3D = jp.Now3D;

            jp.PrevPos3D[0] = jp.Pos3D;
            for (var i = 1; i < NOrderLPF; i++)
            {
                //jp.PrevPos3D[i] = jp.PrevPos3D[i] * Smooth + jp.PrevPos3D[i - 1] * (1f - Smooth);
                jp.PrevPos3D[i] = jp.PrevPos3D[i] * Smooth + jp.PrevPos3D[i - 1] * (1f - Smooth);
            }
            jp.Pos3D = jp.PrevPos3D[NOrderLPF - 1];

 
            jp.Visibled = true;
            jpindex++;
        }

        if (frwdAngle < 45f)
        {
            if (EstimatedScore > ForwardThreshold)
            {
                VNectModel.IsPoseUpdate = true;
            }
        }
        else
        {
            if (EstimatedScore > BackwardThreshold)
            {
                VNectModel.IsPoseUpdate = true;
            }
        }
    }
    
    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }
    /*
    bool FrontBackCheckv(VNectModel.JointPoint jp1, VNectModel.JointPoint jp2, bool flag)
    {
        var l1 = Vector3.Distance(jp1.PrevNow3D, jp1.Now3D);
        var c1 = Vector3.Distance(jp2.PrevNow3D, jp1.Now3D);

        var l2 = Vector3.Distance(jp2.PrevNow3D, jp2.Now3D);
        var c2 = Vector3.Distance(jp1.PrevNow3D, jp2.Now3D);

        if(l1 > c1 && l2 > c2)
        {
            jp1.Error++;
            jp2.Error++;
            if (!flag && jp1.Error == jp1.RattlingCheckFrame)
            {
                jp1.Error = 0;
                jp2.Error = 0;
                return false;
            }

            return true;
        }
        else
        {
            jp1.Error = 0;
            jp2.Error = 0;

            return false;
        }
    }
    */

    void KalmanUpdate(VNectModel.JointPoint measurement)
    {
        measurementUpdate(measurement);
        measurement.Pos3D.x = measurement.X.x + (measurement.Now3D.x - measurement.X.x) * measurement.K.x;
        measurement.Pos3D.y = measurement.X.y + (measurement.Now3D.y - measurement.X.y) * measurement.K.y;
        measurement.Pos3D.z = measurement.X.z + (measurement.Now3D.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.Pos3D;
    }

    void measurementUpdate(VNectModel.JointPoint measurement)
    {
        measurement.K.x = (measurement.P.x + KalmanParamQ) / (measurement.P.x + KalmanParamQ + KalmanParamR);
        measurement.K.y = (measurement.P.y + KalmanParamQ) / (measurement.P.y + KalmanParamQ + KalmanParamR);
        measurement.K.z = (measurement.P.z + KalmanParamQ) / (measurement.P.z + KalmanParamQ + KalmanParamR);
        measurement.P.x = KalmanParamR * (measurement.P.x + KalmanParamQ) / (KalmanParamR + measurement.P.x + KalmanParamQ);
        measurement.P.y = KalmanParamR * (measurement.P.y + KalmanParamQ) / (KalmanParamR + measurement.P.y + KalmanParamQ);
        measurement.P.z = KalmanParamR * (measurement.P.z + KalmanParamQ) / (KalmanParamR + measurement.P.z + KalmanParamQ);
    }

    private void OnDestroy()
    {
        _worker?.Dispose();

        if (User3Input)
        {
            // Assuming model with multiple inputs that were passed as a Dictionary
            foreach (var key in inputs.Keys)
            {
                inputs[key].Dispose();
            }

            inputs.Clear();
        }
        else
        {
            input.Dispose();
        }
    }
}
