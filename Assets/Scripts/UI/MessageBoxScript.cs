using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxScript : MonoBehaviour
{
    private Text Message;
    private Button BtnOk;
    private Button BtnCancel;

    public delegate void MessageBoxCallback(bool ok);
    private Action<bool> callback;

    public void Init()
    {
        Message = GameObject.Find("Message").GetComponent<Text>();
        BtnOk = GameObject.Find("btnMessageOK").GetComponent<Button>();
        BtnCancel = GameObject.Find("btnMessageCancel").GetComponent<Button>();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowMessage(string msg)
    {
        Message.text = msg;
        BtnOk.gameObject.SetActive(true);
        BtnOk.transform.localPosition = new Vector3(0, -41, 0);
        BtnCancel.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        callback = null;
    }

    public void ShowMessage(string msg, Action<bool> messageBoxCallback)
    {
        Message.text = msg;
        BtnOk.gameObject.SetActive(true);
        BtnOk.transform.localPosition = new Vector3(-90, -41, 0);
        BtnCancel.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
        callback = null;
        callback = messageBoxCallback;
    }

    public void onMessageOK()
    {
        this.gameObject.SetActive(false);
        callback?.Invoke(true);
    }

    public void onMessageCancel()
    {
        this.gameObject.SetActive(false);
        callback?.Invoke(false);
    }
}
