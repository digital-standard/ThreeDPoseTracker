using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraMover : MonoBehaviour
{
    //カメラのtransform  
    private Transform camTransform;
    //初期状態
    private Vector3 initialCamposition;
    private Quaternion initialCamRotation;
    //マウスの始点 
    private Vector3 startMousePos;

    //カメラ操作の有効無効
    public bool CameraMoveActive = false;

    private int CameraAngleCnt = 0;

    private VNectBarracudaRunner barracudaRunner;
    private UIScript uiScript;

    // マウスホイールの回転値を格納する変数
    private float scroll;


    // WASD：前後左右の移動
    // QE：上昇・降下
    // 右ドラッグ：カメラの回転
    // 左ドラッグ：前後左右の移動
    // スペース：カメラ操作の有効・無効の切り替え
    // P：回転を実行時の状態に初期化する

    //カメラの移動量
    [SerializeField, Range(0.1f, 10.0f)]
    private float _positionStep = 2.0f;

    //マウス感度
    [SerializeField, Range(30.0f, 150.0f)]
    private float _mouseSensitive = 90.0f;
    //カメラ回転の始点情報
    private Vector3 _presentCamRotation;
    private Vector3 _presentCamPos;
    //UIメッセージの表示
    private bool _uiMessageActiv;

    private Camera mainCamera;

    void Start()
    {
        camTransform = this.gameObject.transform;
        initialCamposition = this.gameObject.transform.position;
        initialCamRotation = this.gameObject.transform.rotation;

        barracudaRunner = GameObject.Find("BarracudaRunner").GetComponent<VNectBarracudaRunner>();
        uiScript = GameObject.Find("UICanvas").GetComponent<UIScript>();
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if(!CameraMoveActive)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            CameraAngleCnt++;
            var headpos = barracudaRunner.GetHeadPosition();

            // マウスホイールの回転値を変数 scroll に渡す
            scroll = Input.GetAxis("Mouse ScrollWheel");

            if (CameraAngleCnt == 1)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x - 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
            }
            else if(CameraAngleCnt == 2)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x + 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
            }
            else if (CameraAngleCnt == 3)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x, headpos.y, headpos.z - 0.8f);
                this.transform.LookAt(headpos);
            }
            else if (CameraAngleCnt == 4)
            {
                mainCamera.fieldOfView = 60;
                CameraAngleCnt = 0;
                camTransform.position = initialCamposition;
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            uiScript.NextAvatar();
            var headpos = barracudaRunner.GetHeadPosition();
            if (CameraAngleCnt == 1)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x - 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
            }
            else if (CameraAngleCnt == 2)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x + 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
            }
            else if (CameraAngleCnt == 3)
            {
                mainCamera.fieldOfView = 60;
                camTransform.position = new Vector3(headpos.x, headpos.y, headpos.z - 0.8f);
                this.transform.LookAt(headpos);
            }
            else if (CameraAngleCnt == 4)
            {
                mainCamera.fieldOfView = 60;
                CameraAngleCnt = 0;
                camTransform.position = initialCamposition;
            }
        }

        // マウスホイールの回転値を変数 scroll に渡す
        scroll = Input.GetAxis("Mouse ScrollWheel") * 50;
        mainCamera.fieldOfView += scroll;
        /*
        if (CameraAngleCnt == 1)
        {
            camera.fieldOfView += scroll;
        }
        else if (CameraAngleCnt == 2)
        {
            camera.fieldOfView += scroll;
        }
        else if (CameraAngleCnt == 3)
        {
            camera.fieldOfView += scroll;
        }
        else if (CameraAngleCnt == 4)
        {
            camera.fieldOfView += scroll;
        }
        */
        /**/
        CamControlIsActive(); //カメラ操作の有効無効

        if (CameraMoveActive)
        {
            ResetCameraRotation(); //回転角度のみリセット
            CameraRotationMouseControl(); //カメラの回転 マウス
            CameraSlideMouseControl(); //カメラの縦横移動 マウス
            CameraPositionKeyControl(); //カメラのローカル移動 キー
        }
        /**/
    }

    //カメラ操作の有効無効
    public void CamControlIsActive()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CameraMoveActive = !CameraMoveActive;

            if (_uiMessageActiv == false)
            {
                StartCoroutine(DisplayUiMessage());
            }
            Debug.Log("CamControl : " + CameraMoveActive);
        }
    }

    //回転を初期状態にする
    private void ResetCameraRotation()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            this.gameObject.transform.rotation = initialCamRotation;
            Debug.Log("Cam Rotate : " + initialCamRotation.ToString());
        }
    }

    //カメラの回転 マウス
    private void CameraRotationMouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            _presentCamRotation.x = camTransform.transform.eulerAngles.x;
            _presentCamRotation.y = camTransform.transform.eulerAngles.y;
        }

        if (Input.GetMouseButton(0))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (startMousePos.y - Input.mousePosition.y) / Screen.height;

            //回転開始角度 ＋ マウスの変化量 * マウス感度
            float eulerX = _presentCamRotation.x + y * _mouseSensitive;
            float eulerY = _presentCamRotation.y + x * _mouseSensitive;

            camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
        }
    }

    //カメラの移動 マウス
    private void CameraSlideMouseControl()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startMousePos = Input.mousePosition;
            _presentCamPos = camTransform.position;
        }

        if (Input.GetMouseButton(1))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (startMousePos.y - Input.mousePosition.y) / Screen.height;

            x = x * _positionStep;
            y = y * _positionStep;

            Vector3 velocity = camTransform.rotation * new Vector3(x, y, 0);
            velocity = velocity + _presentCamPos;
            camTransform.position = velocity;
        }
    }

    //カメラのローカル移動 キー
    private void CameraPositionKeyControl()
    {
        Vector3 campos = camTransform.position;

        if (Input.GetKey(KeyCode.D)) { campos += camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.A)) { campos -= camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.E)) { campos += camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.Q)) { campos -= camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.W)) { campos += camTransform.forward * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.S)) { campos -= camTransform.forward * Time.deltaTime * _positionStep; }

        camTransform.position = campos;
    }

    //UIメッセージの表示
    private IEnumerator DisplayUiMessage()
    {
        _uiMessageActiv = true;
        float time = 0;
        while (time < 2)
        {
            time = time + Time.deltaTime;
            yield return null;
        }
        _uiMessageActiv = false;
    }

    void OnGUI()
    {
        if (_uiMessageActiv == false) { return; }
        GUI.color = Color.black;
        if (CameraMoveActive == true)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 30, 100, 20), "カメラ操作 有効");
        }

        if (CameraMoveActive == false)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 30, 100, 20), "カメラ操作 無効");
        }
    }

}