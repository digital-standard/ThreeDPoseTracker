using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using System.Linq;

public class VRMLipSyncContextMorphTarget : MonoBehaviour
{
    public bool LipSync = false;

    // Set the blendshape index to go to (-1 means there is not one assigned)
    private int[] visemeToBlendTargets = { -1, 4, 4, 5, 3, 3, 3, 3, -1, 3, 2, 5, 3, 6, 4 }; //AIUEOへの振り分けテーブル

    // smoothing amount
    [Range(1, 100)]
    [Tooltip("Smoothing of 1 will yield only the current predicted viseme, 100 will yield an extremely smooth viseme response.")]
    public int smoothAmount = 70;

    [Range(0.0f, 2.0f)]
    public float LipSyncSensitivity = 1.0f;

    public bool AutoBlink = true;
    private bool isBlink = false;
    public float timeBlink = 0.3f;

    // Look for a lip-sync Context (should be set at the same level as this component)
    private OVRLipSyncContextBase lipsyncContext = null;

    private VRMBlendShapeProxy proxy;

    enum AutoBlinkStatus
    {
        Close,
        HalfClose,
        Open,
    }
    private AutoBlinkStatus eyeStatus;
    private float timeRemining = 0.0f;          //タイマー残り時間
    private bool timerStarted = false;			//タイマースタート管理用

    public float threshold = 0.3f;              // ランダム判定の閾値
    public float interval = 2.0f;				// ランダム判定のインターバル

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        // make sure there is a phoneme context assigned to this object
        lipsyncContext = GetComponent<OVRLipSyncContextBase>();
        if (lipsyncContext == null)
        {
            Debug.LogError("LipSyncContextMorphTarget.Start Error: " +
                "No OVRLipSyncContext component on this object!");
        }
        else
        {
            // Send smoothing amount to context
            lipsyncContext.Smoothing = smoothAmount;
        }

        ResetTimer();
        // ランダム判定用関数をスタートする
        StartCoroutine("RandomChange");
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (LipSync)
        {
            if (lipsyncContext != null)
            {
                // get the current viseme frame
                OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
                if (frame != null)
                {
                    SetVisemeToMorphTarget(frame);
                }

                // Update smoothing value
                if (smoothAmount != lipsyncContext.Smoothing)
                {
                    lipsyncContext.Smoothing = smoothAmount;
                }
            }
        }

        if (AutoBlink)
        {
            // Blink
            if (!timerStarted)
            {
                eyeStatus = AutoBlinkStatus.Close;
                timerStarted = true;
            }
            if (timerStarted)
            {
                timeRemining -= Time.deltaTime;
                if (timeRemining <= 0.0f)
                {
                    eyeStatus = AutoBlinkStatus.Open;
                    ResetTimer();
                }
                else if (timeRemining <= timeBlink * 0.3f)
                {
                    eyeStatus = AutoBlinkStatus.HalfClose;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (AutoBlink)
        {
            if (isBlink)
            {
                switch (eyeStatus)
                {
                    case AutoBlinkStatus.Close:
                        SetCloseEyes();
                        break;
                    case AutoBlinkStatus.HalfClose:
                        SetHalfCloseEyes();
                        break;
                    case AutoBlinkStatus.Open:
                        SetOpenEyes();
                        isBlink = false;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Sets the viseme to morph target.
    /// </summary>
    void SetVisemeToMorphTarget(OVRLipSync.Frame frame)
    {
        if (!proxy)
        {
            return;
        }

        for (int i = 0; i < visemeToBlendTargets.Length; i++)
        {
            if (visemeToBlendTargets[i] != -1)
            {
                proxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset((BlendShapePreset)visemeToBlendTargets[i]), Mathf.Min(1.0f, frame.Visemes[i] * LipSyncSensitivity));
            }
        }
    }

    // VRMBlendShapeProxyを探す
    public void SetVRMBlendShapeProxy(bool flag)
    {
        if (flag)
        {
            LipSync = true;

            proxy = FindObjectOfType<VRMBlendShapeProxy>();
            if(proxy == null)
            {
                LipSync = false;
                AutoBlink = false;
            }
        }
        else
        {
            LipSync = false;

            if(!AutoBlink && !LipSync)
            {
                proxy = null;
            }
        }
    }

    public void SetAutoBlink(bool flag)
    {
        if (flag)
        {
            AutoBlink = true;
            proxy = FindObjectOfType<VRMBlendShapeProxy>();
            if (proxy == null)
            {
                LipSync = false;
                AutoBlink = false;
            }
        }
        else
        {
            AutoBlink = false;

            if (!AutoBlink && !LipSync)
            {
                proxy = null;
            }
        }
    }

    void SetCloseEyes()
    {
        proxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1f);
    }

    void SetHalfCloseEyes()
    {
        proxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0.7f);
    }

    void SetOpenEyes()
    {
        proxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0f);
    }

    //タイマーリセット
    void ResetTimer()
    {
        timeRemining = timeBlink;
        timerStarted = false;
    }

    // ランダム判定用関数
    IEnumerator RandomChange()
    {
        // 無限ループ開始
        while (true)
        {
            //ランダム判定用シード発生
            float _seed = UnityEngine.Random.Range(0.0f, 1.0f);
            if (!isBlink)
            {
                if (_seed > threshold)
                {
                    isBlink = true;
                }
            }
            // 次の判定までインターバルを置く
            //yield return new WaitForSeconds(interval);
            yield return new WaitForSeconds(interval * (UnityEngine.Random.Range(0.0f, 1.0f) + 0.5f) );
        }
    }
}
