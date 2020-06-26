using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationScript : MonoBehaviour
{
    private GameObject pnlImages;
    private GameObject pnlPredict;
    private GameObject pnlRecord;

    private Toggle ShowSource;
    private Toggle ShowInput;
    private Toggle SkipOnDrop;
    private Toggle RepeatPlayback;
    private InputField ifSourceCutScale;
    private InputField ifSourceCutX;
    private InputField ifSourceCutY;
    private InputField ifLowPassFilter;
    private Dropdown trainedModel;

    private Toggle ShowBackground;
    private InputField ifBackgroundFile;
    private InputField ifBackgroundScale;
    private InputField ifBackgroundR;
    private InputField ifBackgroundG;
    private InputField ifBackgroundB;

    private Toggle UseUnityCapture;
    private Toggle MirrorUseCamera;

    private Toggle UseVMCProtocol;
    private InputField ifVMCPIP;
    private InputField ifVMCPPort;
    private Toggle VMCPRot;


    private UIScript currentUI;
    private ConfigurationSetting configurationSetting;


    public void Init()
    {
        pnlImages = GameObject.Find("pnlImages");
        pnlPredict = GameObject.Find("pnlPredict");
        pnlRecord = GameObject.Find("pnlRecord");

        pnlImages.SetActive(true);
        pnlPredict.SetActive(true);
        pnlRecord.SetActive(true);

        ShowSource = GameObject.Find("ShowSource").GetComponent<Toggle>();
        ShowInput = GameObject.Find("ShowInput").GetComponent<Toggle>();
        SkipOnDrop = GameObject.Find("SkipOnDrop").GetComponent<Toggle>();
        RepeatPlayback = GameObject.Find("RepeatPlayback").GetComponent<Toggle>();
        ifSourceCutScale = GameObject.Find("ifSourceCutScale").GetComponent<InputField>();
        ifSourceCutX = GameObject.Find("ifSourceCutX").GetComponent<InputField>();
        ifSourceCutY = GameObject.Find("ifSourceCutY").GetComponent<InputField>();
        ifLowPassFilter = GameObject.Find("ifLowPassFilter").GetComponent<InputField>();
        trainedModel = GameObject.Find("TrainedModel").GetComponent<Dropdown>();

        ShowBackground = GameObject.Find("ShowBackground").GetComponent<Toggle>();
        ifBackgroundFile = GameObject.Find("ifBackgroundFile").GetComponent<InputField>();
        ifBackgroundScale = GameObject.Find("ifBackgroundScale").GetComponent<InputField>();
        ifBackgroundR = GameObject.Find("ifBackgroundR").GetComponent<InputField>();
        ifBackgroundG = GameObject.Find("ifBackgroundG").GetComponent<InputField>();
        ifBackgroundB = GameObject.Find("ifBackgroundB").GetComponent<InputField>();

        UseUnityCapture = GameObject.Find("UseUnityCapture").GetComponent<Toggle>();
        MirrorUseCamera = GameObject.Find("MirrorUseCamera").GetComponent<Toggle>();

        UseVMCProtocol = GameObject.Find("UseVMCProtocol").GetComponent<Toggle>();
        ifVMCPIP = GameObject.Find("ifVMCPIP").GetComponent<InputField>();
        ifVMCPPort = GameObject.Find("ifVMCPPort").GetComponent<InputField>();
        VMCPRot = GameObject.Find("VMCPRot").GetComponent<Toggle>();

        pnlImages.SetActive(true);
        pnlPredict.SetActive(false);
        pnlRecord.SetActive(false);
    }


    public void ShowSetting(ConfigurationSetting config)
    {
        configurationSetting = config;

        ShowSource.isOn = config.ShowSource == 1;
        ShowInput.isOn = config.ShowInput == 1;
        SkipOnDrop.isOn = config.SkipOnDrop == 1;
        RepeatPlayback.isOn = config.RepeatPlayback == 1;

        ifSourceCutScale.text = config.SourceCutScale.ToString("0.00");
        ifSourceCutX.text = config.SourceCutX.ToString("0.00");
        ifSourceCutY.text = config.SourceCutY.ToString("0.00");
        ifLowPassFilter.text = config.LowPassFilter.ToString("0.00");
        trainedModel.value = config.TrainedModel;

        ShowBackground.isOn = config.ShowBackground == 1;
        ifBackgroundFile.text = config.BackgroundFile;
        ifBackgroundScale.text = config.BackgroundScale.ToString("0.00");
        ifBackgroundR.text = config.BackgroundR.ToString("0");
        ifBackgroundG.text = config.BackgroundG.ToString("0");
        ifBackgroundB.text = config.BackgroundB.ToString("0");

        UseUnityCapture.isOn = config.UseUnityCapture == 1;
        MirrorUseCamera.isOn = config.MirrorUseCamera == 1;

        UseVMCProtocol.isOn = config.UseVMCProtocol == 1;
        ifVMCPIP.text = config.VMCPIP;
        ifVMCPPort.text = config.VMCPPort.ToString("0"); ;
        VMCPRot.isOn = config.VMCPRot == 1;
    }

    public void Show(UIScript ui, ConfigurationSetting config)
    {
        currentUI = ui;
        ShowSetting(config);

        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    private string SetSetting()
    {
        configurationSetting.ShowSource = ShowSource.isOn ? 1 : 0;
        configurationSetting.ShowInput = ShowInput.isOn ? 1 : 0;
        configurationSetting.SkipOnDrop = SkipOnDrop.isOn ? 1 : 0;
        configurationSetting.RepeatPlayback = RepeatPlayback.isOn ? 1 : 0;

        if (!float.TryParse(ifSourceCutScale.text, out configurationSetting.SourceCutScale))
        {
            return "Source Cut Scale is required.";
        }
        if (!float.TryParse(ifSourceCutX.text, out configurationSetting.SourceCutX))
        {
            return "Source Cut Center position X is required.";
        }
        if (!float.TryParse(ifSourceCutY.text, out configurationSetting.SourceCutY))
        {
            return "Source Cut Center position Y is required.";
        }
        var f = 0f;
        if (!float.TryParse(ifLowPassFilter.text, out f))
        {
            return "Low Pass Filter is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Low Pass Filter is between 0 and 1.";
        }
        configurationSetting.LowPassFilter = f;
        configurationSetting.TrainedModel = trainedModel.value;

        configurationSetting.ShowBackground = ShowBackground.isOn ? 1 : 0;
        configurationSetting.BackgroundFile = ifBackgroundFile.text.Trim();
        if (!float.TryParse(ifBackgroundScale.text, out configurationSetting.BackgroundScale))
        {
            return "Background Scale is required.";
        }
        var i = 0;
        if (!int.TryParse(ifBackgroundR.text, out i))
        {
            return "Background Color R is required.";
        }
        if (i < 0 || i > 255)
        {
            return "Background Color R is between 0 and 255";
        }
        configurationSetting.BackgroundR = i;
        if (!int.TryParse(ifBackgroundG.text, out i))
        {
            return "Background Color G is required.";
        }
        if (i < 0 || i > 255)
        {
            return "Background Color G is between 0 and 255";
        }
        configurationSetting.BackgroundG = i;
        if (!int.TryParse(ifBackgroundB.text, out i))
        {
            return "Background Color B is required.";
        }
        if (i < 0 || i > 255)
        {
            return "Background Color B is between 0 and 255";
        }
        configurationSetting.BackgroundB = i;

        configurationSetting.UseUnityCapture = UseUnityCapture.isOn ? 1 : 0;
        configurationSetting.MirrorUseCamera = MirrorUseCamera.isOn ? 1 : 0;

        configurationSetting.UseVMCProtocol = UseVMCProtocol.isOn ? 1 : 0;
        configurationSetting.VMCPIP = ifVMCPIP.text.Trim();
        if (!int.TryParse(ifVMCPPort.text, out i))
        {
            return "Server IP is required.";
        }
        if (i < 0 || i > 100000)
        {
            return "Port num is between 0 and 100000";
        }
        configurationSetting.VMCPPort = i;
        configurationSetting.VMCPRot = VMCPRot.isOn ? 1 : 0;

        return "";
    }

    public void onOK()
    {
        var msg = SetSetting();
        if (msg != "")
        {
            currentUI.ShowMessage(msg);
        }
        else
        {
            currentUI.SetConfiguration(configurationSetting);
            Close();
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
            currentUI.SetConfiguration(configurationSetting);
        }
    }

    public void onRestoreSettings()
    {
        currentUI.RestoreSettings();
    }

    public void onCancel()
    {
        Close();
    }

    public void onTabImages()
    {
        pnlImages.SetActive(true);
        pnlPredict.SetActive(false);
        pnlRecord.SetActive(false);
    }

    public void onTabPredict()
    {
        pnlImages.SetActive(false);
        pnlPredict.SetActive(true);
        pnlRecord.SetActive(false);
    }

    public void onTabRecord()
    {
        pnlImages.SetActive(false);
        pnlPredict.SetActive(false);
        pnlRecord.SetActive(true);
    }

    public void onBackgroundFile()
    {
        var extensions = new[]
        {
                new ExtensionFilter( "Image Files", "png", "jpg", "jpeg" ),
            };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length != 0)
        {
            ifBackgroundFile.text = paths[0];
        }
    }

    public void TrainedModel_Changed(int value)
    {
    }
}
