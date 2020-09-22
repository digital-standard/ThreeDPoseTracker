using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoCapture : MonoBehaviour
{
    public enum Status
    {
        Stop,
        VideoPlay,
        CamPlay,
        VideoPause,
        CamPause
    }
    private Status status = Status.Stop;

    public GameObject InputTexture;
    public RawImage VideoScreen;
    public GameObject VideoBackground;
    public float SourceCutScale;
    public float SourceCutX;
    public float SourceCutY;
    public LayerMask _layer;
    public bool UseWebCam = true;
    public int WebCamNum = 0;
    public VideoPlayer VideoPlayer;

    private WebCamTexture webCamTexture;
    private RenderTexture videoTexture;
    private GameObject MainTextureCamera = null;

    private int videoScreenWidth = 2560 * 2;
    private int bgWidth, bgHeight;
    public float SourceFps = 30f;

    private Vector3 localScale;
    public RenderTexture MainTexture { get; private set; }

    public void Init(int bgWidth, int bgHeight)
    {
        this.bgWidth = bgWidth;
        this.bgHeight = bgHeight;

        localScale = VideoScreen.transform.localScale;

        //if (UseWebCam) CameraPlayStart();
        //else VideoPlayStart();
    }

    public void OnCompletePrepare()
    {

    }

    public void CameraPlayStart(int index)
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamNum = index;
        if (devices.Length < WebCamNum)
        {
            webCamTexture = new WebCamTexture(devices[index].name);
        }
        else
        {
            webCamTexture = new WebCamTexture(devices[WebCamNum].name);
        }

        var sd = VideoScreen.GetComponent<RectTransform>();
        VideoScreen.texture = webCamTexture;

        VideoScreen.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);

        SourceFps = 30f;

        webCamTexture.Play();

        sd.sizeDelta = new Vector2(videoScreenWidth, videoScreenWidth * webCamTexture.height / webCamTexture.width);
        var aspect = (float)webCamTexture.width / webCamTexture.height;
        VideoBackground.transform.localScale = new Vector3(-aspect * SourceCutScale, 1 * SourceCutScale, 1) ;
        VideoBackground.transform.localPosition = new Vector3(SourceCutX, SourceCutY, -2f);
        VideoBackground.GetComponent<Renderer>().material.mainTexture = webCamTexture;

        InitMainTexture();

        MainTextureCamera.transform.localPosition = new Vector3(SourceCutX, SourceCutY, -2f);

        status =  Status.CamPlay;
    }

    public delegate void VideoReadyDelegate();
    public VideoReadyDelegate VideoReady;

    public IEnumerator VideoStart(string path)
    {
        VideoPlayer.url = path;

        // Video Playerの準備（完了まで待つ）
        yield return new WaitForSeconds(1);

        VideoClip vclip = (VideoClip)Resources.Load(path);
        yield return new WaitForSeconds(1);
        //videoTexture = new RenderTexture((int)vclip.width, (int)vclip.height, 24);
        if (VideoPlayer.clip != null)
        {
            if (VideoPlayer.clip.width == 0 || VideoPlayer.clip.height == 0)
            {
                videoTexture = new RenderTexture(1920, 1080, 24);
            }
            else
            {
                videoTexture = new RenderTexture((int)VideoPlayer.clip.width, (int)VideoPlayer.clip.height, 24);
            }
        }
        else
        {
            videoTexture = new RenderTexture(1920, 1080, 24);
        }
        VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        VideoPlayer.targetTexture = videoTexture;

        // Video Playerの準備（完了まで待つ）
        VideoPlayer.Prepare();
        while (!VideoPlayer.isPrepared)
            yield return null;

        SourceFps = (float)VideoPlayer.frameRate;

        var sd = VideoScreen.GetComponent<RectTransform>();
        if (VideoPlayer.clip != null)
        {
            sd.sizeDelta = new Vector2(videoScreenWidth, (int)(videoScreenWidth * VideoPlayer.clip.height / VideoPlayer.clip.width));
        }
        else
        {
            sd.sizeDelta = new Vector2(videoScreenWidth, (int)(videoScreenWidth * 1080 / 1920));
        }
        
        VideoScreen.texture = videoTexture;
        VideoScreen.transform.localScale = localScale;

        VideoPlayer.Play();
        
        var aspect = (float)videoTexture.width / videoTexture.height;

        VideoBackground.transform.localScale = new Vector3(aspect * SourceCutScale, 1 * SourceCutScale, 1) ;
        VideoBackground.transform.localPosition = new Vector3(0f, 0f, -2f);
        VideoBackground.GetComponent<Renderer>().material.mainTexture = videoTexture;
        
        InitMainTexture();
        
        MainTextureCamera.transform.localPosition = new Vector3(SourceCutX, SourceCutY, -2f);

        status = Status.VideoPlay;

        if (VideoReady != null) VideoReady();
    }

    public void ResetScale(float scale, float x, float y, bool isMirror)
    {
        SourceCutScale = scale;
        SourceCutX = x;
        SourceCutY = y;
        if (VideoScreen.texture != null)
        {
            var aspect = (float)VideoScreen.texture.width / VideoScreen.texture.height;
            if(status == Status.Stop || status == Status.VideoPlay || status == Status.VideoPause)
            {
                // ビデオの場合、反転しない
            }
            else
            {
                // カメラは設定による
                if(isMirror)
                {
                    aspect =  -aspect;
                }

            }
            VideoBackground.transform.localScale = new Vector3(aspect * SourceCutScale, 1 * SourceCutScale, 1) ;

        }

        if(MainTextureCamera != null)
        {
            MainTextureCamera.transform.localPosition = new Vector3(SourceCutX, SourceCutY, -2f);
        }
    }

    public bool IsPlay()
    {
        if (status == Status.VideoPlay || status == Status.CamPlay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPause()
    {
        if (status == Status.VideoPause || status == Status.CamPause)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayStop()
    {
        if (status == Status.VideoPlay || status == Status.VideoPause)
        {
            VideoPlayer.Stop();
        }
        else if (status == Status.CamPlay || status == Status.CamPause)
        {
            webCamTexture.Stop();
        }

        status = Status.Stop;
    }

    public void Pause()
    {
        if (status == Status.VideoPlay)
        {
            VideoPlayer.Pause();
            status = Status.VideoPause;
        }
        else if (status == Status.CamPlay)
        {
            webCamTexture.Pause();
            status = Status.CamPause;
        }
    }

    public void Resume()
    {
        if (status == Status.VideoPause)
        {
            VideoPlayer.Play();
            status = Status.VideoPlay;
        }
        else if (status == Status.CamPause)
        {
            webCamTexture.Play();
            status = Status.CamPlay;
        }
    }

    private void InitMainTexture()
    {
        if(MainTextureCamera != null)
        {
            return;
        }

        MainTextureCamera = new GameObject("MainTextureCamera", typeof(Camera));

        MainTextureCamera.transform.parent = VideoBackground.transform;
        MainTextureCamera.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        MainTextureCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f );
        MainTextureCamera.transform.localEulerAngles = Vector3.zero;
        MainTextureCamera.layer = _layer;
        
        var camera = MainTextureCamera.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 0.5f ;
        camera.depth = -5;
        camera.depthTextureMode = 0;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.cullingMask = _layer;
        camera.useOcclusionCulling = false;
        camera.nearClipPlane = 1.0f;
        camera.farClipPlane = 5.0f;
        camera.allowMSAA = false;
        camera.allowHDR = false;

        MainTexture = new RenderTexture(bgWidth, bgHeight, 0, RenderTextureFormat.RGB565, RenderTextureReadWrite.sRGB)
        {
            useMipMap = false,
            autoGenerateMips = false,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };
        
        camera.targetTexture = MainTexture;
        if(InputTexture.activeSelf) InputTexture.GetComponent<Renderer>().material.mainTexture = MainTexture;
        
    }
}
