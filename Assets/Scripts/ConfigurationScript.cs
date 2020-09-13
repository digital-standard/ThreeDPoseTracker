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
    private GameObject pnlLipSync;
    private GameObject pnlRoom;
    private GameObject pnlOthers;

    private Toggle ShowSource;
    private Toggle ShowInput;
    private Toggle SkipOnDrop;
    private Toggle RepeatPlayback;
    private Toggle MirrorUseCamera;
    private InputField ifSourceCutScale;
    private InputField ifSourceCutX;
    private InputField ifSourceCutY;

    private Toggle ShowBackground;
    private InputField ifBackgroundFile;
    private InputField ifBackgroundScale;
    private InputField ifBackgroundR;
    private InputField ifBackgroundG;
    private InputField ifBackgroundB;

    private InputField ifLowPassFilter;
    private InputField ifNOrderLPF;
    //private InputField ifBWBuffer;
    //private InputField ifBWCutoff;
    private InputField ifRangePathFilterBuffer;
    private InputField ifFIROrderN;
    private InputField ifFIRFromHz;
    private InputField ifFIRToHz;
    private InputField ifForwardThreshold;
    private InputField ifBackwardThreshold;
    private Toggle LockFoot;
    private Toggle LockLegs;
    private Toggle LockHand;
    private Toggle ElbowAxisTop;
    //private InputField ifHeightRatioThreshold;
    private Dropdown trainedModel;
/*
    private InputField ifShoulderRattlingCheckFrame;
    private InputField ifThighRattlingCheckFrame;
    private InputField ifFootRattlingCheckFrame;
    private InputField ifArmRattlingCheckFrame;
    private InputField ifShinThreshold;
    private InputField ifShinSmooth;
    private InputField ifShinRatio;
    private InputField ifArmThreshold;
    private InputField ifArmSmooth;
    private InputField ifArmRatio;
    private InputField ifOtherThreshold;
    private InputField ifOtherSmooth;
    private InputField ifOtherRatio;
*/
    private Toggle Blender;
    private Toggle EnforceHumanoidBones;
    private Toggle Capturing;
    private InputField ifCapturingFPS;
    private Toggle CatchUp;

    private Toggle UseLipSync;
    private Dropdown ddMicSelect;
    private InputField ifLipSyncSmoothAmount;
    private InputField ifLipSyncSensitivity;
    private Toggle UseAutoBlink;
    private InputField ifTimeBlink;
    private InputField ifAutoBlinkThreshold;
    private InputField ifAutoBlinkInterval;

    private Toggle ShowRoom;
    private InputField ifRoomX;
    private InputField ifRoomY;
    private InputField ifRoomZ;
    private InputField ifRoomRotX;
    private InputField ifRoomRotY;
    private InputField ifRoomRotZ;
    private InputField ifRoomScaleX;
    private InputField ifRoomScaleY;
    private InputField ifRoomScaleZ;
    private Toggle ReceiveShadow;
    private Toggle UseGrounderIK;
    private InputField ifIKPositionWeight;
    private InputField ifLegPositionWeight;
    private InputField ifHeightOffset;

    private Toggle UseUnityCapture;
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
        pnlLipSync = GameObject.Find("pnlLipSync");
        pnlRoom = GameObject.Find("pnlRoom");
        pnlOthers = GameObject.Find("pnlOthers");

        pnlImages.SetActive(true);
        pnlPredict.SetActive(true);
        pnlRecord.SetActive(true);
        pnlLipSync.SetActive(true);
        pnlRoom.SetActive(true);
        pnlOthers.SetActive(true);

        ShowSource = GameObject.Find("ShowSource").GetComponent<Toggle>();
        ShowInput = GameObject.Find("ShowInput").GetComponent<Toggle>();
        SkipOnDrop = GameObject.Find("SkipOnDrop").GetComponent<Toggle>();
        RepeatPlayback = GameObject.Find("RepeatPlayback").GetComponent<Toggle>();
        MirrorUseCamera = GameObject.Find("MirrorUseCamera").GetComponent<Toggle>();
        ifSourceCutScale = GameObject.Find("ifSourceCutScale").GetComponent<InputField>();
        ifSourceCutX = GameObject.Find("ifSourceCutX").GetComponent<InputField>();
        ifSourceCutY = GameObject.Find("ifSourceCutY").GetComponent<InputField>();

        ShowBackground = GameObject.Find("ShowBackground").GetComponent<Toggle>();
        ifBackgroundFile = GameObject.Find("ifBackgroundFile").GetComponent<InputField>();
        ifBackgroundScale = GameObject.Find("ifBackgroundScale").GetComponent<InputField>();
        ifBackgroundR = GameObject.Find("ifBackgroundR").GetComponent<InputField>();
        ifBackgroundG = GameObject.Find("ifBackgroundG").GetComponent<InputField>();
        ifBackgroundB = GameObject.Find("ifBackgroundB").GetComponent<InputField>();

        ifLowPassFilter = GameObject.Find("ifLowPassFilter").GetComponent<InputField>();
        ifNOrderLPF = GameObject.Find("ifNOrderLPF").GetComponent<InputField>();
        //ifBWBuffer = GameObject.Find("ifBWBuffer").GetComponent<InputField>();
        //ifBWCutoff = GameObject.Find("ifBWCutoff").GetComponent<InputField>();
        ifRangePathFilterBuffer = GameObject.Find("ifRangePathFilterBuffer").GetComponent<InputField>();
        ifFIROrderN = GameObject.Find("ifFIROrderN").GetComponent<InputField>();
        ifFIRFromHz = GameObject.Find("ifFIRFromHz").GetComponent<InputField>();
        ifFIRToHz = GameObject.Find("ifFIRToHz").GetComponent<InputField>();
        ifForwardThreshold = GameObject.Find("ifForwardThreshold").GetComponent<InputField>();
        ifBackwardThreshold = GameObject.Find("ifBackwardThreshold").GetComponent<InputField>();
        LockFoot = GameObject.Find("LockFoot").GetComponent<Toggle>();
        LockLegs = GameObject.Find("LockLegs").GetComponent<Toggle>();
        LockHand = GameObject.Find("LockHand").GetComponent<Toggle>();
        ElbowAxisTop = GameObject.Find("ElbowAxisTop").GetComponent<Toggle>();
        //ifHeightRatioThreshold = GameObject.Find("ifHeightRatioThreshold").GetComponent<InputField>();
        trainedModel = GameObject.Find("TrainedModel").GetComponent<Dropdown>();
/*
        ifShoulderRattlingCheckFrame = GameObject.Find("ifShoulderRattlingCheckFrame").GetComponent<InputField>();
        ifThighRattlingCheckFrame = GameObject.Find("ifThighRattlingCheckFrame").GetComponent<InputField>();
        ifFootRattlingCheckFrame = GameObject.Find("ifFootRattlingCheckFrame").GetComponent<InputField>();
        ifArmRattlingCheckFrame = GameObject.Find("ifArmRattlingCheckFrame").GetComponent<InputField>();
        ifShinThreshold = GameObject.Find("ifShinThreshold").GetComponent<InputField>();
        ifShinSmooth = GameObject.Find("ifShinSmooth").GetComponent<InputField>();
        ifShinRatio = GameObject.Find("ifShinRatio").GetComponent<InputField>();
        ifArmThreshold = GameObject.Find("ifArmThreshold").GetComponent<InputField>();
        ifArmSmooth = GameObject.Find("ifArmSmooth").GetComponent<InputField>();
        ifArmRatio = GameObject.Find("ifArmRatio").GetComponent<InputField>();
        ifOtherThreshold = GameObject.Find("ifOtherThreshold").GetComponent<InputField>();
        ifOtherSmooth = GameObject.Find("ifOtherSmooth").GetComponent<InputField>();
        ifOtherRatio = GameObject.Find("ifOtherRatio").GetComponent<InputField>();
*/
        Blender = GameObject.Find("Blender").GetComponent<Toggle>();
        EnforceHumanoidBones = GameObject.Find("EnforceHumanoidBones").GetComponent<Toggle>();
        Capturing = GameObject.Find("Capturing").GetComponent<Toggle>();
        ifCapturingFPS = GameObject.Find("ifCapturingFPS").GetComponent<InputField>();
        CatchUp = GameObject.Find("CatchUp").GetComponent<Toggle>();

        UseLipSync = GameObject.Find("UseLipSync").GetComponent<Toggle>();
        ddMicSelect = GameObject.Find("MicSelect").GetComponent<Dropdown>();
        ifLipSyncSmoothAmount = GameObject.Find("ifLipSyncSmoothAmount").GetComponent<InputField>();
        ifLipSyncSensitivity = GameObject.Find("ifLipSyncSensitivity").GetComponent<InputField>();
        UseAutoBlink = GameObject.Find("UseAutoBlink").GetComponent<Toggle>();
        ifTimeBlink = GameObject.Find("ifTimeBlink").GetComponent<InputField>();
        ifAutoBlinkThreshold = GameObject.Find("ifAutoBlinkThreshold").GetComponent<InputField>();
        ifAutoBlinkInterval = GameObject.Find("ifAutoBlinkInterval").GetComponent<InputField>();

        ShowRoom = GameObject.Find("ShowRoom").GetComponent<Toggle>();
        ifRoomX = GameObject.Find("ifRoomX").GetComponent<InputField>();
        ifRoomY = GameObject.Find("ifRoomY").GetComponent<InputField>();
        ifRoomZ = GameObject.Find("ifRoomZ").GetComponent<InputField>();
        ifRoomRotX = GameObject.Find("ifRoomRotX").GetComponent<InputField>();
        ifRoomRotY = GameObject.Find("ifRoomRotY").GetComponent<InputField>();
        ifRoomRotZ = GameObject.Find("ifRoomRotZ").GetComponent<InputField>();
        ifRoomScaleX = GameObject.Find("ifRoomScaleX").GetComponent<InputField>();
        ifRoomScaleY = GameObject.Find("ifRoomScaleY").GetComponent<InputField>();
        ifRoomScaleZ = GameObject.Find("ifRoomScaleZ").GetComponent<InputField>();
        ReceiveShadow = GameObject.Find("ReceiveShadow").GetComponent<Toggle>();
        UseGrounderIK = GameObject.Find("UseGrounderIK").GetComponent<Toggle>();
        ifIKPositionWeight = GameObject.Find("ifIKPositionWeight").GetComponent<InputField>();
        ifLegPositionWeight = GameObject.Find("ifLegPositionWeight").GetComponent<InputField>();
        ifHeightOffset = GameObject.Find("ifHeightOffset").GetComponent<InputField>();

        UseUnityCapture = GameObject.Find("UseUnityCapture").GetComponent<Toggle>();
        UseVMCProtocol = GameObject.Find("UseVMCProtocol").GetComponent<Toggle>();
        ifVMCPIP = GameObject.Find("ifVMCPIP").GetComponent<InputField>();
        ifVMCPPort = GameObject.Find("ifVMCPPort").GetComponent<InputField>();
        VMCPRot = GameObject.Find("VMCPRot").GetComponent<Toggle>();

        DeactivateTabPanel();
        pnlImages.SetActive(true);
    }

    public void ShowSetting(ConfigurationSetting config)
    {
        configurationSetting = config;

        ShowSource.isOn = config.ShowSource == 1;
        ShowInput.isOn = config.ShowInput == 1;
        SkipOnDrop.isOn = config.SkipOnDrop == 1;
        RepeatPlayback.isOn = config.RepeatPlayback == 1;
        MirrorUseCamera.isOn = config.MirrorUseCamera == 1;

        ifSourceCutScale.text = config.SourceCutScale.ToString("0.00");
        ifSourceCutX.text = config.SourceCutX.ToString("0.00");
        ifSourceCutY.text = config.SourceCutY.ToString("0.00");

        ShowBackground.isOn = config.ShowBackground == 1;
        ifBackgroundFile.text = config.BackgroundFile;
        ifBackgroundScale.text = config.BackgroundScale.ToString("0.00");
        ifBackgroundR.text = config.BackgroundR.ToString("0");
        ifBackgroundG.text = config.BackgroundG.ToString("0");
        ifBackgroundB.text = config.BackgroundB.ToString("0");

        ifLowPassFilter.text = config.LowPassFilter.ToString("0.00");
        ifNOrderLPF.text = config.NOrderLPF.ToString();
        //ifBWBuffer.text = config.BWBuffer.ToString();
        //ifBWCutoff.text = config.BWCutoff.ToString("0.00");
        ifRangePathFilterBuffer.text = config.RangePathFilterBuffer03.ToString("0");
        ifFIROrderN.text = config.FIROrderN03.ToString("0");
        ifFIRFromHz.text = config.FIRFromHz.ToString("0.00");
        ifFIRToHz.text = config.FIRToHz.ToString("0.00");
        ifForwardThreshold.text = config.ForwardThreshold.ToString("0.00");
        ifBackwardThreshold.text = config.BackwardThreshold.ToString("0.00");
        LockFoot.isOn = config.LockFoot == 1;
        LockLegs.isOn = config.LockLegs == 1;
        LockHand.isOn = config.LockHand == 1;
        ElbowAxisTop.isOn = config.ElbowAxisTop == 1;
        //ifHeightRatioThreshold.text = config.HeightRatioThreshold.ToString("0.00");
        trainedModel.value = config.TrainedModel;
/*
        ifShoulderRattlingCheckFrame.text = config.ShoulderRattlingCheckFrame.ToString();
        ifThighRattlingCheckFrame.text = config.ThighRattlingCheckFrame.ToString();
        ifFootRattlingCheckFrame.text = config.FootRattlingCheckFrame.ToString();
        ifArmRattlingCheckFrame.text = config.ArmRattlingCheckFrame.ToString();
        ifShinThreshold.text = config.ShinThreshold.ToString("0.00");
        ifShinSmooth.text = config.ShinSmooth.ToString("0.00");
        ifShinRatio.text = config.ShinRatio.ToString("0.00");
        ifArmThreshold.text = config.ArmThreshold.ToString("0.00");
        ifArmSmooth.text = config.ArmSmooth.ToString("0.00");
        ifArmRatio.text = config.ArmRatio.ToString("0.00");
        ifOtherThreshold.text = config.OtherThreshold.ToString("0.00");
        ifOtherSmooth.text = config.OtherSmooth.ToString("0.00");
        ifOtherRatio.text = config.OtherRatio.ToString("0.00");
*/
        Blender.isOn = config.Blender == 1;
        EnforceHumanoidBones.isOn = config.EnforceHumanoidBones == 1;
        Capturing.isOn = config.Capturing == 1;
        ifCapturingFPS.text = config.CapturingFPS.ToString("0");
        CatchUp.isOn = config.CatchUp == 1;

        UseLipSync.isOn = config.UseLipSync == 1;
        SetMicList(config.SelectedMic);
        ifLipSyncSmoothAmount.text = config.LipSyncSmoothAmount.ToString("0");
        ifLipSyncSensitivity.text = config.LipSyncSensitivity.ToString("0.00");
        UseAutoBlink.isOn = config.UseAutoBlink == 1;
        ifTimeBlink.text = config.TimeBlink.ToString("0.00");
        ifAutoBlinkThreshold.text = config.AutoBlinkThreshold.ToString("0.00");
        ifAutoBlinkInterval.text = config.AutoBlinkInterval.ToString("0.00");

        ShowRoom.isOn = config.ShowRoom == 1;
        ifRoomX.text = config.RoomX.ToString("0.00");
        ifRoomY.text = config.RoomY.ToString("0.00");
        ifRoomZ.text = config.RoomZ.ToString("0.00");
        ifRoomRotX.text = config.RoomRotX.ToString("0.00");
        ifRoomRotY.text = config.RoomRotY.ToString("0.00");
        ifRoomRotZ.text = config.RoomRotZ.ToString("0.00");
        ifRoomScaleX.text = config.RoomScaleX.ToString("0.00");
        ifRoomScaleY.text = config.RoomScaleY.ToString("0.00");
        ifRoomScaleZ.text = config.RoomScaleZ.ToString("0.00");
        ReceiveShadow.isOn = config.ReceiveShadow == 1;
        UseGrounderIK.isOn = config.UseGrounderIK == 1;
        ifIKPositionWeight.text = config.IKPositionWeight.ToString("0.00");
        ifLegPositionWeight.text = config.LegPositionWeight.ToString("0.00");
        ifHeightOffset.text = config.HeightOffset.ToString("0.00");

        UseUnityCapture.isOn = config.UseUnityCapture == 1;
        UseVMCProtocol.isOn = config.UseVMCProtocol == 1;
        ifVMCPIP.text = config.VMCPIP;
        ifVMCPPort.text = config.VMCPPort.ToString("0"); ;
        VMCPRot.isOn = config.VMCPRot == 1;
    }

    private void SetMicList(string selectedMic)
    {
        ddMicSelect.options.Clear();
        bool find = false;
        for (int i = 0; i < Microphone.devices.Length; ++i)
        {
            var name = Microphone.devices[i].ToString();
            ddMicSelect.options.Add(new Dropdown.OptionData(name));
            if(name == selectedMic)
            {
                ddMicSelect.value = i;
                find = true;
            }
        }
        if (!find && ddMicSelect.options.Count > 0)
        {
            ddMicSelect.value = 0;
        }
        ddMicSelect.RefreshShownValue();
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
        var f = 0f;

        configurationSetting.ShowSource = ShowSource.isOn ? 1 : 0;
        configurationSetting.ShowInput = ShowInput.isOn ? 1 : 0;
        configurationSetting.SkipOnDrop = SkipOnDrop.isOn ? 1 : 0;
        configurationSetting.RepeatPlayback = RepeatPlayback.isOn ? 1 : 0;
        configurationSetting.MirrorUseCamera = MirrorUseCamera.isOn ? 1 : 0;

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

        if (!float.TryParse(ifLowPassFilter.text, out f))
        {
            return "Low Pass Filter is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Low Pass Filter is between 0 and 1.";
        }
        configurationSetting.LowPassFilter = f;

        if (!int.TryParse(ifNOrderLPF.text, out i))
        {
            return "N-Order LPF is required.";
        }
        if (i < 1 || i >= 10)
        {
            return "N-Order LPF is between 1 and 10.";
        }
        configurationSetting.NOrderLPF = i;
        /*
        if (!int.TryParse(ifBWBuffer.text, out i))
        {
            return "BW Buffer is required.";
        }
        if (i < 100 || i >= 10000)
        {
            return "BW Buffer is between 100 and 10000.";
        }
        configurationSetting.BWBuffer = i;

        if (!float.TryParse(ifBWCutoff.text, out f))
        {
            return "BW Cutoff is required.";
        }
        if (f < 0f || f > 100f)
        {
            return "BW Cutoff is between 0 and 100.";
        }
        configurationSetting.BWCutoff = f;
        */
        if (!int.TryParse(ifRangePathFilterBuffer.text, out i))
        {
            return "Range Path Filter Buffer is required.";
        }
        if (i < 10 || i >= 10000)
        {
            return "Range Path Filter Buffer is between 10 and 10000.";
        }
        configurationSetting.RangePathFilterBuffer03 = i;

        if (!int.TryParse(ifFIROrderN.text, out i))
        {
            return "FIR Order N is required.";
        }
        if (i < 10 || i >= 10000)
        {
            return "FIR Order N is between 10 and 10000.";
        }
        configurationSetting.FIROrderN03 = i;

        if (!float.TryParse(ifFIRFromHz.text, out f))
        {
            return "FIR From Hz is required.";
        }
        if (f <= 0f || f > 10f)
        {
            return "FIR From Hz is between 0 and 10.";
        }
        configurationSetting.FIRFromHz = f;

        if (!float.TryParse(ifFIRToHz.text, out f))
        {
            return "FIR To Hz is required.";
        }
        if (f <= 0f || f > 10f)
        {
            return "FIR To Hz is between 0 and 10.";
        }
        configurationSetting.FIRToHz = f;

        if (!float.TryParse(ifForwardThreshold.text, out f))
        {
            return "Forward Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Forward Threshold is between 0 and 1.";
        }
        configurationSetting.ForwardThreshold = f;

        if (!float.TryParse(ifBackwardThreshold.text, out f))
        {
            return "Backward Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Backward Threshold is between 0 and 1.";
        }
        configurationSetting.BackwardThreshold = f;

        configurationSetting.LockFoot = LockFoot.isOn ? 1 : 0;
        configurationSetting.LockLegs = LockLegs.isOn ? 1 : 0;
        configurationSetting.LockHand = LockHand.isOn ? 1 : 0;
        configurationSetting.ElbowAxisTop = ElbowAxisTop.isOn ? 1 : 0;
        /*
        if (!float.TryParse(ifHeightRatioThreshold.text, out f))
        {
            return "Height Ratio Threshold is required.";
        }
        if (f < 0f || f > 100f)
        {
            return "Height Ratio Threshold is between 0 and 100.";
        }
        configurationSetting.HeightRatioThreshold = f;
        */
        configurationSetting.TrainedModel = trainedModel.value;
        /*
        if (!int.TryParse(ifShoulderRattlingCheckFrame.text, out i))
        {
            return "Shoulder Rattling Check Frame is required.";
        }
        if (i < 0 || i >= 30)
        {
            return "Shoulder Rattling Check Frame is between 0 and 30.";
        }
        configurationSetting.ShoulderRattlingCheckFrame = i;

        if (!int.TryParse(ifThighRattlingCheckFrame.text, out i))
        {
            return "Thigh Rattling Check Frame is required.";
        }
        if (i < 0 || i >= 30)
        {
            return "Thigh Rattling Check Frame is between 0 and 30.";
        }
        configurationSetting.ThighRattlingCheckFrame = i;

        if (!int.TryParse(ifFootRattlingCheckFrame.text, out i))
        {
            return "Foot Rattling Check Frame is required.";
        }
        if (i < 0 || i >= 30)
        {
            return "Foot Rattling Check Frame is between 0 and 30.";
        }
        configurationSetting.FootRattlingCheckFrame = i;

        if (!int.TryParse(ifArmRattlingCheckFrame.text, out i))
        {
            return "Arm Rattling Check Frame is required.";
        }
        if (i < 0 || i >= 30)
        {
            return "Arm Rattling Check Frame is between 0 and 30.";
        }
        configurationSetting.ArmRattlingCheckFrame = i;

        if (!float.TryParse(ifShinThreshold.text, out f))
        {
            return "Shin Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Shin Threshold is between 0 and 1.";
        }
        configurationSetting.ShinThreshold = f;

        if (!float.TryParse(ifShinSmooth.text, out f))
        {
            return "Shin Smooth is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Shin Smooth is between 0 and 1.";
        }
        configurationSetting.ShinSmooth = f;

        if (!float.TryParse(ifShinRatio.text, out f))
        {
            return "Shin Ratio is required.";
        }
        if (f < 0f || f > 10f)
        {
            return "Shin Ratio is between 0 and 10.";
        }
        configurationSetting.ShinRatio = f;

        if (!float.TryParse(ifArmThreshold.text, out f))
        {
            return "Arm Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Arm Threshold is between 0 and 1.";
        }
        configurationSetting.ArmThreshold = f;

        if (!float.TryParse(ifArmSmooth.text, out f))
        {
            return "Arm Smooth is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Arm Smooth is between 0 and 1.";
        }
        configurationSetting.ArmSmooth = f;

        if (!float.TryParse(ifArmRatio.text, out f))
        {
            return "Arm Ratio is required.";
        }
        if (f < 0f || f > 10f)
        {
            return "Arm Ratio is between 0 and 10.";
        }
        configurationSetting.ArmRatio = f;


        if (!float.TryParse(ifOtherThreshold.text, out f))
        {
            return "Other Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Other Threshold is between 0 and 1.";
        }
        configurationSetting.OtherThreshold = f;

        if (!float.TryParse(ifOtherSmooth.text, out f))
        {
            return "Other Smooth is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Other Smooth is between 0 and 1.";
        }
        configurationSetting.OtherSmooth = f;

        if (!float.TryParse(ifOtherRatio.text, out f))
        {
            return "Other Ratio is required.";
        }
        if (f < 0f || f > 10f)
        {
            return "Other Ratio is between 0 and 10.";
        }
        configurationSetting.OtherRatio = f;
        */
        configurationSetting.Blender = Blender.isOn ? 1 : 0;
        configurationSetting.EnforceHumanoidBones = EnforceHumanoidBones.isOn ? 1 : 0;
        configurationSetting.Capturing = Capturing.isOn ? 1 : 0;
        if (!float.TryParse(ifCapturingFPS.text, out f))
        {
            return "Capturing FPS is required.";
        }
        if (f < 0f || f > 120f)
        {
            return "Low Capturing FPS is between 0 and 120.";
        }
        configurationSetting.CapturingFPS = f;
        configurationSetting.CatchUp = CatchUp.isOn ? 1 : 0;

        configurationSetting.UseLipSync = UseLipSync.isOn ? 1 : 0;
        configurationSetting.SelectedMic = ddMicSelect.options[ddMicSelect.value].text;
        if (!int.TryParse(ifLipSyncSmoothAmount.text, out i))
        {
            return "Smooth Amount is required.";
        }
        if (i < 0 || i > 100)
        {
            return "Smooth Amount is between 0 and 100.";
        }
        configurationSetting.LipSyncSmoothAmount = i;
        if (!float.TryParse(ifLipSyncSensitivity.text, out f))
        {
            return "Sensitivity is required.";
        }
        if (f < 0f || f > 2f)
        {
            return "Sensitivity is between 0 and 2.";
        }
        configurationSetting.LipSyncSensitivity = f;
        configurationSetting.UseAutoBlink = UseAutoBlink.isOn ? 1 : 0;
        if (!float.TryParse(ifTimeBlink.text, out f))
        {
            return "TimeBlink is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "TimeBlink is between 0 and 1.";
        }
        configurationSetting.TimeBlink = f;
        if (!float.TryParse(ifAutoBlinkThreshold.text, out f))
        {
            return "Auto Blink Threshold is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Auto Blink Threshold is between 0 and 1.";
        }
        configurationSetting.AutoBlinkThreshold = f;
        if (!float.TryParse(ifAutoBlinkInterval.text, out f))
        {
            return "Auto Blink Interval is required.";
        }
        if (f < 0f || f > 10f)
        {
            return "Auto Blink Interval is between 0 and 10.";
        }
        configurationSetting.AutoBlinkInterval = f;

        configurationSetting.ShowRoom = ShowRoom.isOn ? 1 : 0;
        if (!float.TryParse(ifRoomX.text, out f))
        {
            return "Room Position X is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Position X is between -100 and 100.";
        }
        configurationSetting.RoomX = f;
        if (!float.TryParse(ifRoomY.text, out f))
        {
            return "Room Position Y is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Position Y is between -100 and 100.";
        }
        configurationSetting.RoomY = f;
        if (!float.TryParse(ifRoomZ.text, out f))
        {
            return "Room Position Z is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Position Z is between -100 and 100.";
        }
        configurationSetting.RoomZ = f;
        if (!float.TryParse(ifRoomRotX.text, out f))
        {
            return "Room Rotation X is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Rotation X is between -100 and 100.";
        }
        configurationSetting.RoomRotX = f;
        if (!float.TryParse(ifRoomRotY.text, out f))
        {
            return "Room Rotation Y is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Rotation Y is between -100 and 100.";
        }
        configurationSetting.RoomRotY = f;
        if (!float.TryParse(ifRoomRotZ.text, out f))
        {
            return "Room Rotation Z is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Rotation Z is between -100 and 100.";
        }
        configurationSetting.RoomRotZ = f;
        if (!float.TryParse(ifRoomScaleX.text, out f))
        {
            return "Room Scale X is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Scale X is between -100 and 100.";
        }
        configurationSetting.RoomScaleX = f;
        if (!float.TryParse(ifRoomScaleY.text, out f))
        {
            return "Room Scale Y is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Scale Y is between -100 and 100.";
        }
        configurationSetting.RoomScaleY = f;
        if (!float.TryParse(ifRoomScaleZ.text, out f))
        {
            return "Room Scale Z is required.";
        }
        if (f < -100f || f > 100f)
        {
            return "Room Scale Z is between -100 and 100.";
        }
        configurationSetting.RoomScaleZ = f;
        configurationSetting.ReceiveShadow = ReceiveShadow.isOn ? 1 : 0;
        configurationSetting.UseGrounderIK = UseGrounderIK.isOn ? 1 : 0;
        if (!float.TryParse(ifIKPositionWeight.text, out f))
        {
            return "IK Position Weight is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "IK Position Weight 0 and 1.";
        }
        configurationSetting.IKPositionWeight = f;
        if (!float.TryParse(ifLegPositionWeight.text, out f))
        {
            return "Leg Position Weight is required.";
        }
        if (f < 0f || f > 1f)
        {
            return "Leg Position Weight 0 and 1.";
        }
        configurationSetting.LegPositionWeight = f;
        if (!float.TryParse(ifHeightOffset.text, out f))
        {
            return "Height Offset is required.";
        }
        if (f < -10f || f > 10f)
        {
            return "Height Offset -10 and 10.";
        }
        configurationSetting.HeightOffset = f;

        configurationSetting.UseUnityCapture = UseUnityCapture.isOn ? 1 : 0;
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
        DeactivateTabPanel();
        pnlImages.SetActive(true);
    }

    public void onTabPredict()
    {
        DeactivateTabPanel();
        pnlPredict.SetActive(true);
    }

    public void onTabRecord()
    {
        DeactivateTabPanel();
        pnlRecord.SetActive(true);
    }

    public void onTabLipSync()
    {
        DeactivateTabPanel();
        pnlLipSync.SetActive(true);
    }

    public void onTabRoom()
    {
        DeactivateTabPanel();
        pnlRoom.SetActive(true);
    }

    public void onTabOthers()
    {
        DeactivateTabPanel();
        pnlOthers.SetActive(true);
    }

    private void DeactivateTabPanel()
    {
        if (pnlImages.activeSelf) pnlImages.SetActive(false);
        if (pnlPredict.activeSelf) pnlPredict.SetActive(false);
        if (pnlRecord.activeSelf) pnlRecord.SetActive(false);
        if (pnlLipSync.activeSelf) pnlLipSync.SetActive(false);
        if (pnlRoom.activeSelf) pnlRoom.SetActive(false);
        if (pnlOthers.activeSelf) pnlOthers.SetActive(false);
    }

    public void onOpenBrowser()
    {
        Application.OpenURL("https://digital-standard.com");
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

    public void onRoomCredit1()
    {
        Application.OpenURL("https://sketchfab.com/3d-models/the-charterhouse-great-chamber-50e692c037784347b289d7bbcb318bed");
    }

    public void onRoomCredit2()
    {
        Application.OpenURL("https://twitter.com/artfletch");
    }

    public void TrainedModel_Changed(int value)
    {
    }
}
