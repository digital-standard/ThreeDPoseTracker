using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor.Recorder;
//using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class UIScript : MonoBehaviour
{
    private Material skeletonMaterial;

    public List<AvatarSetting> AvatarList = new List<AvatarSetting>();
    private ConfigurationSetting configurationSetting = new ConfigurationSetting();

    private CameraMover cameraMover;
    private GameObject Menu;
    private AvatarSettingScript avatarSetting;
    private ConfigurationScript configuration;
    private VNectBarracudaRunner barracudaRunner;
    private VideoCapture videoCapture;

    private GameObject pnlVideoIF;
    private Button btnPause;
    private InputField ifFrame;
    private Text txFrameCount;
    private Button btnSkip;

    private Dropdown sourceDevice;
    private Dropdown avatars;
    private Button btnSourceDevice;
    private Text txtFPS;

    private Button btnRecord;

    public RawImage BackgroundImage;
    public Texture BackgroundTexture;
    public Camera Maincamera;

    private string AppVer = "0.13";
    private int ConfigColums = 21;

    public MessageBoxScript message;

    public GameObject OSCClient;


    private void Awake()
    {
        var appVer = PlayerPrefs.GetString("AppVer", "");
        if(appVer != AppVer)
        {
            PlayerPrefs.SetString("AppVer", AppVer);
            PlayerPrefs.SetString("Configuration", "");
            PlayerPrefs.SetString("AvatarSettings", "");
            PlayerPrefs.Save();
        }
        var configs = PlayerPrefs.GetString("Configuration", "");
        string[] cCol = configs.Split(',');
        if (cCol.Length == ConfigColums)
        {
            int.TryParse(cCol[0], out configurationSetting.ShowSource);
            int.TryParse(cCol[1], out configurationSetting.ShowInput);
            int.TryParse(cCol[2], out configurationSetting.SkipOnDrop);
            int.TryParse(cCol[3], out configurationSetting.RepeatPlayback);
            float.TryParse(cCol[4], out configurationSetting.SourceCutScale);
            float.TryParse(cCol[5], out configurationSetting.SourceCutX);
            float.TryParse(cCol[6], out configurationSetting.SourceCutY);
            float.TryParse(cCol[7], out configurationSetting.LowPassFilter);
            int.TryParse(cCol[8], out configurationSetting.TrainedModel);

            int.TryParse(cCol[9], out configurationSetting.ShowBackground);
            configurationSetting.BackgroundFile = cCol[10];
            float.TryParse(cCol[11], out configurationSetting.BackgroundScale);
            int.TryParse(cCol[12], out configurationSetting.BackgroundR);
            int.TryParse(cCol[13], out configurationSetting.BackgroundG);
            int.TryParse(cCol[14], out configurationSetting.BackgroundB);

            int.TryParse(cCol[15], out configurationSetting.UseUnityCapture);
            int.TryParse(cCol[16], out configurationSetting.MirrorUseCamera);

            int.TryParse(cCol[17], out configurationSetting.UseVMCProtocol);
            configurationSetting.VMCPIP = cCol[18];
            int.TryParse(cCol[19], out configurationSetting.VMCPPort);
            int.TryParse(cCol[20], out configurationSetting.VMCPRot);
        }
        else
        {
            SaveConfiguration(configurationSetting);
        }

        message = GameObject.Find("pnlMessage").GetComponent<MessageBoxScript>();
        message.Init();
        message.Hide();

        OSCClient.SetActive(false);

    }

    void Start()
    {
        skeletonMaterial = Resources.Load("Skeleton", typeof(Material)) as Material;

        barracudaRunner = GameObject.Find("BarracudaRunner").GetComponent<VNectBarracudaRunner>();
        barracudaRunner.ModelQuality = configurationSetting.TrainedModel;

        videoCapture = GameObject.Find("MainTexrure").GetComponent<VideoCapture>();

        Menu = GameObject.Find("Menu");
        cameraMover = GameObject.Find("MainCamera").GetComponent<CameraMover>();

        sourceDevice = GameObject.Find("SourceDevice").GetComponent<Dropdown>();
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (var d in devices)
        {
            sourceDevice.options.Add(new Dropdown.OptionData(d.name));
        }
        sourceDevice.value = 0;
        
        btnPause = GameObject.Find("btnPause").GetComponent<Button>();
        ifFrame = GameObject.Find("ifFrame").GetComponent<InputField>();
        txFrameCount = GameObject.Find("txFrameCount").GetComponent<Text>();
        btnSkip = GameObject.Find("btnSkip").GetComponent<Button>();
        pnlVideoIF = GameObject.Find("pnlVideoIF");
        pnlVideoIF.SetActive(false);

        btnSourceDevice = GameObject.Find("btnSourceDevice").GetComponent<Button>();
        txtFPS = GameObject.Find("txtFPS").GetComponent<Text>();
        btnRecord = GameObject.Find("btnRecord").GetComponent<Button>();

        avatars = GameObject.Find("Avatars").GetComponent<Dropdown>();

        avatarSetting = GameObject.Find("AvatarSetting").GetComponent<AvatarSettingScript>();
        avatarSetting.Init();
        avatarSetting.gameObject.SetActive(false);
        configuration = GameObject.Find("Configuration").GetComponent<ConfigurationScript>();
        configuration.Init();
        configuration.gameObject.SetActive(false);

        ReflectConfiguration(configurationSetting);

        var settings = PlayerPrefs.GetString("AvatarSettings", "");
        //settings = "";
        // Decode Avatar Setting
        string[] asStr = settings.Split(';');
        foreach (var s in asStr)
        {
            string[] col = s.Split(',');
            if (col.Length != 16)
            {
                continue;
            }
            var setting = new AvatarSetting();

            if (!int.TryParse(col[0], out setting.AvatarType))
            {
                continue;
            }
            if (setting.AvatarType < 0)
            {

            }
            else if (setting.AvatarType == 0)
            {
                setting.VRMFilePath = col[1];
            }
            else if (setting.AvatarType == 1)
            {
                setting.FBXFilePath = col[1];
            }
            setting.AvatarName = col[2];
            if (!float.TryParse(col[3], out setting.PosX))
            {
                continue;
            }
            if (!float.TryParse(col[4], out setting.PosY))
            {
                continue;
            }
            if (!float.TryParse(col[5], out setting.PosZ))
            {
                continue;
            }
            if (!float.TryParse(col[6], out setting.DepthScale))
            {
                continue;
            }
            if (!float.TryParse(col[7], out setting.Scale))
            {
                continue;
            }
            if (!float.TryParse(col[8], out setting.FaceOriX))
            {
                continue;
            }
            if (!float.TryParse(col[9], out setting.FaceOriY))
            {
                continue;
            }
            if (!float.TryParse(col[10], out setting.FaceOriZ))
            {
                continue;
            }
            if (!int.TryParse(col[11], out setting.SkeletonVisible))
            {
                continue;
            }
            if (!float.TryParse(col[12], out setting.SkeletonPosX))
            {
                continue;
            }
            if (!float.TryParse(col[13], out setting.SkeletonPosY))
            {
                continue;
            }
            if (!float.TryParse(col[14], out setting.SkeletonPosZ))
            {
                continue;
            }
            if (!float.TryParse(col[15], out setting.SkeletonScale))
            {
                continue;
            }

            AvatarList.Add(setting);
        };

        if (AvatarList.Count == 0)
        {
            var setting = new AvatarSetting()
            {
                AvatarType = -1,
                AvatarName = "蘭鈴りゅん (Lune)",
                Avatar = GameObject.Find("Lune").GetComponent<VNectModel>(),
            };
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            AvatarList.Add(setting);
            barracudaRunner.InitVNectModel(setting.Avatar);

            setting = new AvatarSetting()
            {
                AvatarType = -2,
                AvatarName = "yukihiko-chan",
                Avatar = GameObject.Find("YukihikoAoyagi").GetComponent<VNectModel>(),
            };
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            AvatarList.Add(setting);
            barracudaRunner.InitVNectModel(setting.Avatar);

        }

        avatars.options.Clear();
        foreach (var setting in AvatarList)
        {
            if (setting.AvatarType >= 0)
            {
                LoadAvatar(setting);
            }
            else if (setting.AvatarType < 0)
            {
                avatars.options.Add(new Dropdown.OptionData(setting.AvatarName));

                switch (setting.AvatarType)
                {
                    case -1:
                        setting.Avatar = GameObject.Find("Lune").GetComponent<VNectModel>();
                        break;

                    case -2:
                        setting.Avatar = GameObject.Find("YukihikoAoyagi").GetComponent<VNectModel>();
                        break;
                }

                setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
                barracudaRunner.InitVNectModel(setting.Avatar);
            }
        }
        avatars.value = 0;
        ChangedAvatar(0);
    }

    public void NextAvatar()
    {
        var v = avatars.value + 1;
        if(AvatarList.Count <= v || AvatarList[v].Avatar == null)
        {
            v = 0;
        }
        avatars.value = v;
    }

    void Update()
    {
        if(Menu != null && !Menu.activeSelf)
        {
            if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Escape))
            {
                Menu.gameObject.SetActive(true);
                cameraMover.CameraMoveActive = false;
            }

        }
        else if (barracudaRunner != null)
        {
            txtFPS.text = "FPS:" + barracudaRunner.FPS.ToString("0.0");
        }

        if (pnlVideoIF != null && pnlVideoIF.activeSelf && barracudaRunner.videoCapture.IsPlay())
        {
            ifFrame.text = barracudaRunner.videoCapture.VideoPlayer.frame.ToString();
            txFrameCount.text = barracudaRunner.videoCapture.VideoPlayer.frameCount.ToString();
        }
    }

    public void onCloseMenu()
    {
        Menu.gameObject.SetActive(false);
        cameraMover.CameraMoveActive = true;
    }

    public void onVideoPause()
    {
        if(barracudaRunner.videoCapture.IsPlay())
        {
            barracudaRunner.PlayPause();
        }
        else if (barracudaRunner.videoCapture.IsPause())
        {
            barracudaRunner.Resume();
        }
    }

    public void onVideoSkip()
    {
        if (pnlVideoIF != null && pnlVideoIF.activeSelf)
        {
            long l = 0;
            if (long.TryParse(ifFrame.text, out l))
            {
                barracudaRunner.videoCapture.VideoPlayer.frame = l;
            }
        }
    }

    private void SetBackgroundImage(ConfigurationSetting config)
    {
        if(config.ShowBackground == 0)
        {
            BackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            BackgroundImage.gameObject.SetActive(true);

            Texture texture;

            if (config.BackgroundFile != string.Empty && File.Exists(config.BackgroundFile))
            {
                texture = PngToTex2D(config.BackgroundFile);
            }
            else
            {
                texture = BackgroundTexture;
            }

            BackgroundImage.texture = texture;
            var rt = BackgroundImage.GetComponent< RectTransform>();
            rt.sizeDelta = new Vector2(texture.width, texture.height);
            BackgroundImage.rectTransform.localScale = new Vector3(config.BackgroundScale, config.BackgroundScale, config.BackgroundScale);
        }

        Maincamera.backgroundColor = new Color(config.BackgroundR / 255f, config.BackgroundG / 255f, config.BackgroundB / 255f );
    }

    Texture2D PngToTex2D(string path)
    {
        BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        byte[] rb = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        int pos = 16, width = 0, height = 0;
        for (int i = 0; i < 4; i++) width = width * 256 + rb[pos++];
        for (int i = 0; i < 4; i++) height = height * 256 + rb[pos++];
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(rb);
        return texture;
    }

    private void ReflectConfiguration(ConfigurationSetting config)
    {
        barracudaRunner.videoCapture.VideoScreen.gameObject.SetActive(config.ShowSource == 1);
        barracudaRunner.videoCapture.InputTexture.gameObject.SetActive(config.ShowInput == 1);
        barracudaRunner.videoCapture.VideoPlayer.skipOnDrop = (config.SkipOnDrop == 1);
        barracudaRunner.videoCapture.VideoPlayer.isLooping = (config.RepeatPlayback == 1);
        barracudaRunner.videoCapture.ResetScale(config.SourceCutScale, config.SourceCutX, config.SourceCutY, config.MirrorUseCamera == 1);
        barracudaRunner.Smooth = config.LowPassFilter;

        Maincamera.GetComponent<UnityCapture>().enabled = config.UseUnityCapture == 1;

        SetBackgroundImage(config);

        if(config.UseVMCProtocol == 1)
        {
            StartVMCProtocol(configurationSetting);
        }
        else
        {
            StopVMCProtocol();
        }
    }

    private void SaveConfiguration(ConfigurationSetting config)
    {
        PlayerPrefs.SetString("AppVer", AppVer);
        PlayerPrefs.SetString("Configuration", config.ToString());
        PlayerPrefs.Save();
    }

    public void SetConfiguration(ConfigurationSetting config)
    {
        ReflectConfiguration(config);
        SaveConfiguration(config);
    }

    public void SourceDevice_Changed(int value)
    {
        if(value == 0)
        {
            btnSourceDevice.GetComponentInChildren<Text>().text = "Load Movie";
        }
        else
        {
            btnSourceDevice.GetComponentInChildren<Text>().text = "Start Cam";
        }
    }

    public void onSourceDevice()
    {
        barracudaRunner.PlayStop();

        if (sourceDevice.value == 0)
        {
            var extensions = new[]
            {
                new ExtensionFilter( "Movie Files", "mp4", "mov", "wmv" ),
            };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

            if (paths.Length != 0)
            {
                barracudaRunner.VideoPlayStart(paths[0]);
                pnlVideoIF.SetActive(true);
            }
        }
        else
        {
            barracudaRunner.CameraPlayStart(sourceDevice.value - 1);
            pnlVideoIF.SetActive(false);
        }
    }

    public void onConfiguration()
    {
        configuration.Show(this, configurationSetting);
    }

    public void Avatars_Changed(int value)
    {
        ChangedAvatar(value);
    }

    private void ChangedAvatar(int value)
    {
        DiactivateAvatars();
        var setting = AvatarList[value];
        setting.Avatar.gameObject.SetActive(true);
        setting.Avatar.SetSettings(setting);

        barracudaRunner.SetVNectModel(setting.Avatar);

        var src = OSCClient.GetComponent<VMCPBonesSender>();
        src.Model = AvatarList[avatars.value].Avatar.gameObject;
    }

    public void onAddAvatar()
    {
        avatarSetting.ShowAddAvatar(this, new AvatarSetting());
    }

    public async void AddAvatar(AvatarSetting setting)
    {
        await onLoadVRMAsync(setting);

        AvatarList.Add(setting);
        avatars.value = avatars.options.Count - 1;
        SaveSetting();
    }

    public async void LoadAvatar(AvatarSetting setting)
    {
        await onLoadVRMAsync(setting);
        avatars.value = avatars.options.Count - 1;
    }

    public void onAvatarSetting()
    {
        avatarSetting.ShowAvatarSetting(this, AvatarList[avatars.value].Clone());
    }

    public void SetAvatar(AvatarSetting setting)
    {
        AvatarList[avatars.value] = setting;
        avatars.options[avatars.value].text = setting.AvatarName;
        ChangedAvatar(avatars.value);
        SaveSetting();
    }

    public void RemoveAvatar()
    {
        AvatarList.RemoveAt(avatars.value);
        avatars.options.RemoveAt(avatars.value);
        avatars.value = 0;
        SaveSetting();
    }

    private void SaveSetting()
    {
        var saveStr = "";
        foreach (var setting in AvatarList)
        {
            saveStr += setting.ToString();
        }

        PlayerPrefs.SetString("AppVer", AppVer);
        PlayerPrefs.SetString("AvatarSettings", saveStr);
        PlayerPrefs.Save();
    }

    private async System.Threading.Tasks.Task onLoadVRMAsync(AvatarSetting setting)
    {
        videoCapture.PlayStop();

        var path = "";
        if (setting.AvatarType == 0)
        {
            path = setting.VRMFilePath;
        }
        else
        {
            path = setting.FBXFilePath;
        }

        if (path != "")
        {
            //ファイルをByte配列に読み込みます
            var bytes = File.ReadAllBytes(path);

            //VRMImporterContextがVRMを読み込む機能を提供します
            var context = new VRMImporterContext();

            // GLB形式でJSONを取得しParseします
            context.ParseGlb(bytes);

            // VRMのメタデータを取得
            var meta = context.ReadMeta(false); //引数をTrueに変えるとサムネイルも読み込みます

            //読み込めたかどうかログにモデル名を出力してみる
            //Debug.LogFormat("meta: title:{0}", meta.Title);

            //非同期処理(Task)で読み込みます
            await context.LoadAsyncTask();

            ///
            //読込が完了するとcontext.RootにモデルのGameObjectが入っています
            var avatarObject = context.Root;
            avatarObject.name = setting.AvatarName;

            //モデルをワールド上に配置します
            avatarObject.transform.SetParent(transform.parent, false);

            SetVRMBounds(avatarObject.transform);

            //メッシュを表示します
            context.ShowMeshes();

            setting.Avatar = avatarObject.AddComponent<VNectModel>();
            setting.Avatar.ModelObject = avatarObject;
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            setting.Avatar.SkeletonMaterial = skeletonMaterial;
            DiactivateAvatars();
            avatars.options.Add(new Dropdown.OptionData(setting.AvatarName));
            barracudaRunner.InitVNectModel(setting.Avatar);
        }
    }

    /// <summary>
    /// カメラでアップするとメッシュが消えてしまう場合の対応
    /// </summary>
    /// <param name="t"></param>
    private void SetVRMBounds(Transform t)
    {
        for (var i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            var smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.localBounds = new Bounds(new Vector3(), smr.localBounds.size);
            }
            
            if(child.childCount > 0)
            {
                SetVRMBounds(child);
            }
        }
    }

    private void DiactivateAvatars()
    {
        foreach (var setting in AvatarList)
        {
            if (setting.Avatar != null)
            {
                setting.Avatar.Hide();
                setting.Avatar.gameObject.SetActive(false);
            }
        }
    }

    //private RecorderController m_RecorderController;
    private bool isRecording = false;
    BVHRecorder recorder;

    public void onRecord()
    {
        if (isRecording == false)
        {
            recorder = gameObject.AddComponent<BVHRecorder>();
            var setting = AvatarList[avatars.value];
            recorder.targetAvatar = setting.Avatar.GetComponent<Animator>();
            recorder.scripted = true;
            recorder.getBones();
            recorder.rootBone = setting.Avatar.JointPoints[PositionIndex.hip.Int()].Transform;
            recorder.buildSkeleton();
            recorder.genHierarchy();
            recorder.capturing = true;
            isRecording = true;
            btnRecord.image.color = Color.red;
            btnRecord.GetComponentInChildren<Text>().text = "Recording";
            btnRecord.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            var extensions = new[]
            {
                new ExtensionFilter( "BVH Files", "bvh" ),
            };
            var path = StandaloneFileBrowser.SaveFilePanel("SaveFile", "", "motion.bvh", extensions);

            if (path.Length != 0)
            {
                isRecording = false;
                recorder.capturing = false;
                var streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, "motion.bvh");
                FileInfo fi = new FileInfo(path);
                recorder.directory = fi.DirectoryName;
                recorder.filename = fi.Name;
                recorder.saveBVH();
            }
            recorder.clearCapture();
            recorder = null;
            btnRecord.image.color = Color.white; 
            btnRecord.GetComponentInChildren<Text>().text = "Record BVH";
            btnRecord.GetComponentInChildren<Text>().color = Color.black;
        }
    }

    public void onExit()
    {

    }

    public void ShowMessage(string msg)
    {
        message.ShowMessage(msg);
    }

    public void RestoreSettings()
    {
        var msg = "The application settings and avatar settings will also be restored to default. Please restart the application after running it.Are you sure?";
        message.ShowMessage(msg,
            (b) =>
            {
                if(b)
                {
                    PlayerPrefs.SetString("AppVer", AppVer);
                    configurationSetting = new ConfigurationSetting();
                    SaveConfiguration(configurationSetting);
                    SetConfiguration(configurationSetting);
                    PlayerPrefs.SetString("AvatarSettings", "");
                    PlayerPrefs.Save();
                    configuration.Close();
                }
            });
    }

    public void StartVMCProtocol(ConfigurationSetting config)
    {
        var client = OSCClient.GetComponent<uOscClientTDP>();

        if (!OSCClient.activeSelf)
        {
            client.address = config.VMCPIP;
            client.port = config.VMCPPort;
            var vmcp = OSCClient.GetComponent<VMCPBonesSender>();
            vmcp.SetRot(config.VMCPRot == 1);
            if (AvatarList != null && AvatarList.Count > 0)
            {
                vmcp.Model = AvatarList[avatars.value].Avatar.gameObject;
            }
            OSCClient.SetActive(true);
        }
        else
        {
            if(client.address != config.VMCPIP || client.port != config.VMCPPort)
            {
                client.address = config.VMCPIP;
                client.port = config.VMCPPort;
                StartCoroutine(RestartVMCP(config));
            }
        }
    }

    private IEnumerator RestartVMCP(ConfigurationSetting config)
    {
        OSCClient.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        var vmcp = OSCClient.GetComponent<VMCPBonesSender>();
        vmcp.SetRot(config.VMCPRot == 1);
        if (AvatarList != null && AvatarList.Count > 0)
        {
            vmcp.Model = AvatarList[avatars.value].Avatar.gameObject;
        }
        OSCClient.SetActive(true);
    }

    public void StopVMCProtocol()
    {
        OSCClient.SetActive(false);
    }

    void OnDisable()
    {
        /*
        if (m_RecorderController != null)
        {
            m_RecorderController.StopRecording();
        }
        */
    }
}

public class AvatarSetting
{
    public int AvatarType;
    public string VRMFilePath;
    public string FBXFilePath;
    public string AvatarName;
    public float PosX;
    public float PosY;
    public float PosZ;
    public float DepthScale;
    public float Scale;
    public float FaceOriX;
    public float FaceOriY;
    public float FaceOriZ;
    public int SkeletonVisible;
    public float SkeletonPosX;
    public float SkeletonPosY;
    public float SkeletonPosZ;
    public float SkeletonScale;
    public VNectModel Avatar;

    public AvatarSetting()
    {
        AvatarType = 0;
        VRMFilePath = "";
        FBXFilePath = "";
        AvatarName = "New Avatar";
        PosX = 0f;
        PosY = 0f;
        PosZ = 0f;
        DepthScale = 1.0f;
        Scale = 1.0f;
        FaceOriX = 0.0f;
        FaceOriY = -0.001f;
        FaceOriZ = 0.01f;
        SkeletonVisible = 0;
        SkeletonPosX = -0.8f;
        SkeletonPosY = 0.9f;
        SkeletonPosZ = 0f;
        SkeletonScale = 0.005f;
        Avatar = null;
    }

    public AvatarSetting Clone()
    {
        return new AvatarSetting()
        {
            AvatarType = AvatarType,
            VRMFilePath = VRMFilePath,
            FBXFilePath = FBXFilePath,
            AvatarName = AvatarName,
            PosX = PosX,
            PosY = PosY,
            PosZ = PosZ,
            DepthScale = DepthScale,
            Scale = Scale,
            FaceOriX = FaceOriX,
            FaceOriY = FaceOriY,
            FaceOriZ = FaceOriZ,
            SkeletonVisible = SkeletonVisible,
            SkeletonPosX = SkeletonPosX,
            SkeletonPosY = SkeletonPosY,
            SkeletonPosZ = SkeletonPosZ,
            SkeletonScale = SkeletonScale,
            Avatar = Avatar,
        };
}

    public override string ToString()
    {
        var path = "";
        if(AvatarType < 0)
        {

        }
        else if(AvatarType == 0)
        {
            path = VRMFilePath;
        }
        else if (AvatarType == 1)
        {
            path = FBXFilePath;
        }
        return AvatarType.ToString() + "," + path + "," + AvatarName + "," + PosX.ToString() + "," + PosY.ToString() + "," + PosZ.ToString() + "," + DepthScale.ToString()
            + "," + Scale.ToString() + "," + FaceOriX.ToString() + "," + FaceOriY.ToString() + "," + FaceOriZ.ToString()
             + "," + SkeletonVisible.ToString() + "," + SkeletonPosX.ToString() + "," + SkeletonPosY.ToString() + "," + SkeletonPosZ.ToString() + "," + SkeletonScale.ToString() + ";";
    }
}


public class ConfigurationSetting
{
    public int ShowSource;
    public int ShowInput;
    public int SkipOnDrop;
    public int RepeatPlayback;
    public float SourceCutScale;
    public float SourceCutX;
    public float SourceCutY;
    public float LowPassFilter;
    public int TrainedModel;

    public int ShowBackground;
    public string BackgroundFile;
    public float BackgroundScale;
    public int BackgroundR;
    public int BackgroundG;
    public int BackgroundB;

    public int UseUnityCapture;
    public int MirrorUseCamera;

    public int UseVMCProtocol;
    public string VMCPIP;
    public int VMCPPort;
    public int VMCPRot;


    public ConfigurationSetting()
    {
        ShowSource = 1;
        ShowInput = 1;
        SkipOnDrop = 1;
        RepeatPlayback = 1;
        SourceCutScale = 1f;
        SourceCutX = 0f;
        SourceCutY = 0f;
        LowPassFilter = 0.1f;
        TrainedModel = 1;

        ShowBackground = 1;
        BackgroundFile = "";
        BackgroundScale = 1f;
        BackgroundR = 0;
        BackgroundG = 255;
        BackgroundB = 0;

        UseUnityCapture = 0;
        MirrorUseCamera = 1;

        UseVMCProtocol = 0;
        VMCPIP = "127.0.0.1";
        VMCPPort = 39539;
        VMCPRot = 1;
}

public ConfigurationSetting Clone()
    {
        return new ConfigurationSetting()
        {
            ShowSource = ShowSource,
            ShowInput = ShowInput,
            SkipOnDrop = SkipOnDrop,
            RepeatPlayback = RepeatPlayback,
            SourceCutScale = SourceCutScale,
            SourceCutX = SourceCutX,
            SourceCutY = SourceCutY,
            LowPassFilter = LowPassFilter,
            TrainedModel = TrainedModel,
            ShowBackground = ShowBackground,
            BackgroundFile = BackgroundFile,
            BackgroundScale = BackgroundScale,
            BackgroundR = BackgroundR,
            BackgroundG = BackgroundG,
            BackgroundB = BackgroundB,
            UseUnityCapture = UseUnityCapture,
            MirrorUseCamera = MirrorUseCamera,

            UseVMCProtocol = UseVMCProtocol,
            VMCPIP = VMCPIP,
            VMCPPort = VMCPPort,
            VMCPRot = VMCPRot,
        };
    }

    public override string ToString()
    {
        return ShowSource.ToString() + "," + ShowInput.ToString() + "," + SkipOnDrop.ToString() + "," + RepeatPlayback.ToString()
            + "," + SourceCutScale.ToString() + "," + SourceCutX.ToString() + "," + SourceCutY.ToString() + "," + LowPassFilter.ToString()
            + "," + TrainedModel.ToString()
            + "," + ShowBackground.ToString() + "," + BackgroundFile + "," + BackgroundScale.ToString()
            + "," + BackgroundR.ToString() + "," + BackgroundG.ToString() + "," + BackgroundB.ToString()
            + "," + UseUnityCapture.ToString() + "," + MirrorUseCamera.ToString()
            + "," + UseVMCProtocol.ToString() + "," + VMCPIP + "," + VMCPPort.ToString() + "," + VMCPRot.ToString()
            ;
    }
}