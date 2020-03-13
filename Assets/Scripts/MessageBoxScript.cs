using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxScript : MonoBehaviour
{
    private Text Message;

    public void Init()
    {
        Message = GameObject.Find("Message").GetComponent<Text>();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowMessage(string msg)
    {
        Message.text = msg;
        this.gameObject.SetActive(true);
    }

    public void onMessageOK()
    {
        this.gameObject.SetActive(false);
    }
}
