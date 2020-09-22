using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSettingScript : MonoBehaviour
{
    private Text lblAvatarTitle;
    private Button btnAvatarSettingAdd;
    private Button btnAvatarSettingOK;
    private Button btnAvatarSettingCancel;
    private Button btnAvatarSettingRemove;
    private Button btnAvatarSettingApply;

    private InputField ifVRMFile;
    //private InputField ifFBXFile;
    private Button btnVRMFile;
    //private Button btnFBXFile;

    private InputField ifAvatarName;

    private InputField ifX;
    private InputField ifY;
    private InputField ifZ;
    private InputField ifZDeepScale;
    private InputField ifScale;
    private InputField ifFaceOriX;
    private InputField ifFaceOriY;
    private InputField ifFaceOriZ;

    private Toggle tgSkVisible;
    private InputField ifSkX;
    private InputField ifSkY;
    private InputField ifSkZ;
    private InputField ifDisplayScale;

    private UIScript currentUI;
    private AvatarSetting currentSetting;

    public void Init()
    {
        lblAvatarTitle = GameObject.Find("lblAvatarTitle").GetComponent<Text>();

        btnAvatarSettingAdd = GameObject.Find("btnAvatarSettingAdd").GetComponent<Button>();
        btnAvatarSettingOK = GameObject.Find("btnAvatarSettingOK").GetComponent<Button>();
        btnAvatarSettingCancel = GameObject.Find("btnAvatarSettingCancel").GetComponent<Button>();
        btnAvatarSettingRemove = GameObject.Find("btnAvatarSettingRemove").GetComponent<Button>();
        btnAvatarSettingApply = GameObject.Find("btnAvatarSettingApply").GetComponent<Button>();

        ifVRMFile = GameObject.Find("ifVRMFile").GetComponent<InputField>();
        //ifFBXFile = GameObject.Find("ifFBXFile").GetComponent<InputField>();
        btnVRMFile = GameObject.Find("btnVRMFile").GetComponent<Button>();
        //btnFBXFile = GameObject.Find("btnFBXFile").GetComponent<Button>();
        ifAvatarName = GameObject.Find("ifAvatarName").GetComponent<InputField>();
        ifX = GameObject.Find("ifX").GetComponent<InputField>();
        ifY = GameObject.Find("ifY").GetComponent<InputField>();
        ifZ = GameObject.Find("ifZ").GetComponent<InputField>();
        ifZDeepScale = GameObject.Find("ifZDeepScale").GetComponent<InputField>();
        ifScale = GameObject.Find("ifScale").GetComponent<InputField>();
        ifFaceOriX = GameObject.Find("ifFaceOriX").GetComponent<InputField>();
        ifFaceOriY = GameObject.Find("ifFaceOriY").GetComponent<InputField>();
        ifFaceOriZ = GameObject.Find("ifFaceOriZ").GetComponent<InputField>();
        tgSkVisible = GameObject.Find("tgSkVisible").GetComponent<Toggle>();
        ifSkX = GameObject.Find("ifSkX").GetComponent<InputField>();
        ifSkY = GameObject.Find("ifSkY").GetComponent<InputField>();
        ifSkZ = GameObject.Find("ifSkZ").GetComponent<InputField>();
        ifDisplayScale = GameObject.Find("ifDisplayScale").GetComponent<InputField>();
    }

    public void ShowAddAvatar(UIScript ui, AvatarSetting setting)
    {
        currentUI = ui;

        lblAvatarTitle.text = "Add Avatar";
        ifVRMFile.enabled = true;
        btnVRMFile.enabled = true;

        btnAvatarSettingAdd.gameObject.SetActive(true);
        btnAvatarSettingOK.gameObject.SetActive(false);
        btnAvatarSettingCancel.gameObject.SetActive(true);
        btnAvatarSettingRemove.gameObject.SetActive(false);
        btnAvatarSettingApply.gameObject.SetActive(false);

        ShowSetting(setting);

        this.gameObject.SetActive(true);
    }

    public void ShowAvatarSetting(UIScript ui, AvatarSetting setting)
    {
        currentUI = ui;

        lblAvatarTitle.text = "Avatar Setting";
        btnAvatarSettingAdd.gameObject.SetActive(false);
        btnAvatarSettingOK.gameObject.SetActive(true);
        btnAvatarSettingCancel.gameObject.SetActive(true);
        if (setting.AvatarType < 0)
        {
            btnAvatarSettingRemove.gameObject.SetActive(false);
            ifVRMFile.enabled = false;
            btnVRMFile.enabled = false;
        }
        else
        {
            btnAvatarSettingRemove.gameObject.SetActive(true);
            ifVRMFile.enabled = true;
            btnVRMFile.enabled = true;
        }
        btnAvatarSettingApply.gameObject.SetActive(true);

        ShowSetting(setting);

        this.gameObject.SetActive(true);
    }

    public void ShowSetting(AvatarSetting setting)
    {
        currentSetting = setting;

        ifVRMFile.text = setting.VRMFilePath;
        ifAvatarName.text = setting.AvatarName;

        ifX.text = setting.PosX.ToString("0.00");
        ifY.text = setting.PosY.ToString("0.00");
        ifZ.text = setting.PosZ.ToString("0.00");
        ifZDeepScale.text = setting.DepthScale.ToString("0.00");
        ifScale.text = setting.Scale.ToString("0.00");
        ifFaceOriX.text = setting.FaceOriX.ToString("0.000");
        ifFaceOriY.text = setting.FaceOriY.ToString("0.000");
        ifFaceOriZ.text = setting.FaceOriZ.ToString("0.000");
        tgSkVisible.isOn = setting.SkeletonVisible == 1;
        ifSkX.text = setting.SkeletonPosX.ToString("0.00");
        ifSkY.text = setting.SkeletonPosY.ToString("0.00");
        ifSkZ.text = setting.SkeletonPosZ.ToString("0.00");
        ifDisplayScale.text = setting.SkeletonScale.ToString("0.000");

    }

    private string SetSetting()
    {
        if(currentSetting.AvatarType < 0)
        {

        }
        else if(ifVRMFile.text != "" && File.Exists(ifVRMFile.text))
        {
            currentSetting.AvatarType = 0;
            currentSetting.VRMFilePath = ifVRMFile.text;
            currentSetting.FBXFilePath = "";
        }
        else
        {
            return "Please specify VRM File.";
        }

        if (ifAvatarName.text != "")
        {
            currentSetting.AvatarName = ifAvatarName.text;
        }
        else
        {
            return "Avatar Name is required.";
        }

        if (!float.TryParse(ifX.text, out currentSetting.PosX))
        {
            return "Default Position X is required.";
        }
        if (!float.TryParse(ifY.text, out currentSetting.PosY))
        {
            return "Default Position Y is required.";
        }
        if (!float.TryParse(ifZ.text, out currentSetting.PosZ))
        {
            return "Default Position Z is required.";
        }
        if (!float.TryParse(ifZDeepScale.text, out currentSetting.DepthScale))
        {
            return "Depth scale in Z is required.";
        }
        if (!float.TryParse(ifScale.text, out currentSetting.Scale))
        {
            return "Scale is required.";
        }
        if (!float.TryParse(ifFaceOriX.text, out currentSetting.FaceOriX))
        {
            return "Face orientation X is required.";
        }
        if (!float.TryParse(ifFaceOriY.text, out currentSetting.FaceOriY))
        {
            return "Face orientation Y is required.";
        }
        if (!float.TryParse(ifFaceOriZ.text, out currentSetting.FaceOriZ))
        {
            return "Face orientation Z is required.";
        }

        if (tgSkVisible.isOn)
        {
            currentSetting.SkeletonVisible = 1;
        }
        else
        {
            currentSetting.SkeletonVisible = 0;
        }

        if (!float.TryParse(ifSkX.text, out currentSetting.SkeletonPosX))
        {
            return "Skeleton Default Position X is required.";
        }
        if (!float.TryParse(ifSkY.text, out currentSetting.SkeletonPosY))
        {
            return "Skeleton Default Position X is required.";
        }
        if (!float.TryParse(ifSkZ.text, out currentSetting.SkeletonPosZ))
        {
            return "Skeleton Default Position X is required.";
        }
        if (!float.TryParse(ifDisplayScale.text, out currentSetting.SkeletonScale))
        {
            return "Skeleton Scale is required.";
        }

        return "";
    }

    public void onOK()
    {
        var msg = SetSetting();
        if(msg != "")
        {
            currentUI.ShowMessage(msg);
        }
        else
        {
            currentUI.SetAvatar(currentSetting);
            this.gameObject.SetActive(false);
        }
    }

    public void onApply()
    {
        var msg = SetSetting();
        if (msg != "")
        {
            currentUI.ShowMessage(msg);
        }
        else
        {
            currentUI.SetAvatar(currentSetting);
        }
    }

    public void onAdd()
    {
        var msg = SetSetting();
        if (msg != "")
        {
            currentUI.ShowMessage(msg);
        }
        else
        {
            currentUI.AddAvatar(currentSetting);
            this.gameObject.SetActive(false);
        }
    }

    public void onCancel()
    {
        this.gameObject.SetActive(false);
    }

    public void onRemove()
    {
        currentUI.RemoveAvatar();
        this.gameObject.SetActive(false);
    }

    public void onVRMFile()
    {
        var extensions = new[]
        {
            new ExtensionFilter( "VRM Files",  "vrm"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length != 0)
        {
            ifVRMFile.text = paths[0];
        }
    }

    public void onFBXFile()
    {
        var extensions = new[]
        {
            new ExtensionFilter( "FBX Files",  "fbx"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length != 0)
        {
            ifVRMFile.text = "";
            //ifFBXFile.text = paths[0];
        }
    }
}
