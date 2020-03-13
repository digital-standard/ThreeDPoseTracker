using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationScript : MonoBehaviour
{
    private Toggle ShowSource;
    private Toggle ShowInput;
    private Toggle SkipOnDrop;
    private Toggle RepeatPlayback;
    private InputField ifSourceCutScale;
    private InputField ifSourceCutX;
    private InputField ifSourceCutY;
    private InputField ifLowPassFilter;
    private Dropdown trainedModel;

    private UIScript currentUI;
    private ConfigurationSetting configurationSetting;

    public void Init()
    {
        ShowSource = GameObject.Find("ShowSource").GetComponent<Toggle>();
        ShowInput = GameObject.Find("ShowInput").GetComponent<Toggle>();
        SkipOnDrop = GameObject.Find("SkipOnDrop").GetComponent<Toggle>();
        RepeatPlayback = GameObject.Find("RepeatPlayback").GetComponent<Toggle>();
        ifSourceCutScale = GameObject.Find("ifSourceCutScale").GetComponent<InputField>();
        ifSourceCutX = GameObject.Find("ifSourceCutX").GetComponent<InputField>();
        ifSourceCutY = GameObject.Find("ifSourceCutY").GetComponent<InputField>();
        ifLowPassFilter = GameObject.Find("ifLowPassFilter").GetComponent<InputField>();
        trainedModel = GameObject.Find("TrainedModel").GetComponent<Dropdown>();
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
    }

    public void Show(UIScript ui, ConfigurationSetting config)
    {
        currentUI = ui;
        ShowSetting(config);

        this.gameObject.SetActive(true);
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
        if (!float.TryParse(ifLowPassFilter.text, out configurationSetting.LowPassFilter))
        {
            return "Low Pass Filter is required.";
        }
        if(configurationSetting.LowPassFilter < 0f || configurationSetting.LowPassFilter > 1f)
        {
            return "Low Pass Filter is between 0 and 1.";
        }

        configurationSetting.TrainedModel = trainedModel.value;

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
            this.gameObject.SetActive(false);
        }
    }


    public void onCancel()
    {
        this.gameObject.SetActive(false);
    }

    public void TrainedModel_Changed(int value)
    {
    }
}
