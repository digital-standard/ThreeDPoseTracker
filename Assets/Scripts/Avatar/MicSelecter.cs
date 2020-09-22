using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class MicSelecter : MonoBehaviour
{
    public AudioSource audioSource = null;


    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float micInputVolume = 100;

    private int frequency = 48000;

    public string SelectedMic;

    private bool micSelected = false;
    private bool focused = true;

    void Awake()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (!audioSource)
        {
            return;
        }
    }

    void Start()
    {
        audioSource.loop = true;
        audioSource.mute = false;
    }

    /// <summary>
    /// Initializes the microphone.
    /// </summary>
    public void InitializeMic(string selectedMic)
    {
        micSelected = false;
        if (Microphone.devices.Length == 0)
        {
            return;
        }

        bool find = false;
        for (int i = 0; i < Microphone.devices.Length; ++i)
        {
            if (Microphone.devices[i].ToString() == selectedMic)
            {
                find = true;
                break;
            }
        }
        if(!find)
        {
            return;
        }

        SelectedMic = selectedMic;
        micSelected = true;
        GetMicCaps();
    }

    void Update()
    {
        if (!focused)
        {
            if (Microphone.IsRecording(SelectedMic))
            {
                StopMicrophone();
            }
            return;
        }

        if (!Application.isPlaying)
        {
            StopMicrophone();
            return;
        }

        audioSource.volume = (micInputVolume / 100);

        //Constant Speak
        if (!Microphone.IsRecording(SelectedMic))
        {
            StartMicrophone();
        }
    }

    void OnApplicationFocus(bool focus)
    {
        focused = focus;

        if (!focused)
        {
            StopMicrophone();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        focused = !pauseStatus;

        if (!focused)
        {
            StopMicrophone();
        }
    }

    void OnDisable()
    {
        StopMicrophone();
    }

    public void GetMicCaps()
    {
        int min, max;
        Microphone.GetDeviceCaps(SelectedMic, out min, out max);

        if (min == 0 && max == 0)
        {
            max = 44100;
        }

        if (frequency > max)
        {
            frequency = max;
        }
    }

    /// <summary>
    /// Starts the microphone.
    /// </summary>
    public void StartMicrophone()
    {
        //Starts recording
        audioSource.clip = Microphone.Start(SelectedMic, true, 1, frequency);

        Stopwatch timer = Stopwatch.StartNew();

        // Wait until the recording has started
        while (!(Microphone.GetPosition(SelectedMic) > 0) && timer.Elapsed.TotalMilliseconds < 1000)
        {
            Thread.Sleep(50);
        }

        if (Microphone.GetPosition(SelectedMic) <= 0)
        {
        }

        audioSource.Play();
    }

    /// <summary>
    /// Stops the microphone.
    /// </summary>
    public void StopMicrophone()
    {
        if (micSelected == false) return;

        // Overriden with a clip to play? Don't stop the audio source
        if ((audioSource != null) &&
            (audioSource.clip != null) &&
            (audioSource.clip.name == "Microphone"))
        {
            audioSource.Stop();
        }

        // Reset to stop mouth movement
        OVRLipSyncContext context = GetComponent<OVRLipSyncContext>();
        context.ResetContext();

        Microphone.End(SelectedMic);
    }
}
