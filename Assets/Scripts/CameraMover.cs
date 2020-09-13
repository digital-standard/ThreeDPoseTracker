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

    private bool rightRot = false;
    private bool leftRot = false;

    bool KetDownMarginFlag = false;
    float KetDownMargin = 0;

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

        if (KetDownMarginFlag)
        {
            KetDownMargin += Time.deltaTime;

            if (KetDownMargin > 0.25f)
            {
                KetDownMarginFlag = false;
                KetDownMargin = 0f;
            }
        }

        // マウスホイールの回転値を変数 scroll に渡す
        scroll = Input.GetAxis("Mouse ScrollWheel") ;
        if (scroll != 0)
        {
            camTransform.position += camTransform.forward * scroll;
            //mainCamera.fieldOfView += scroll;
        }

        CamControlIsActive(); //カメラ操作の有効無効

        if (CameraMoveActive)
        {
            CameraRotationMouseControl(); //カメラの回転 マウス
            CameraRotation2MouseControl(); //カメラの回転 マウス
            CameraSlideMouseControl(); //カメラの縦横移動 マウス
            CameraPositionKeyControl(); //カメラのローカル移動 キー
        }

        if (rightRot)
        {
            CameraRotationAuto(1f);
        }
        if (leftRot)
        {
            CameraRotationAuto(-1f);
        }
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

    //カメラの回転 マウス
    private void CameraRotationMouseControl()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startMousePos = Input.mousePosition;
            _presentCamRotation.x = camTransform.transform.eulerAngles.x;
            _presentCamRotation.y = camTransform.transform.eulerAngles.y;
        }

        if (Input.GetMouseButton(1))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (startMousePos.y - Input.mousePosition.y) / Screen.height;

            //回転開始角度 ＋ マウスの変化量 * マウス感度
            float eulerX = _presentCamRotation.x + y * _mouseSensitive;
            float eulerY = _presentCamRotation.y - x * _mouseSensitive;

            camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
        }
    }

    //カメラの回転 マウス
    private void CameraRotation2MouseControl()
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
            var headpos = barracudaRunner.GetHeadPosition();
            camTransform.RotateAround(headpos, Vector3.up, -x * 10);
            camTransform.RotateAround(headpos, camTransform.right, y * 10);

        }
    }

    //カメラの移動 マウス
    private void CameraSlideMouseControl()
    {
        if (Input.GetMouseButtonDown(2))
        {
            startMousePos = Input.mousePosition;
            _presentCamPos = camTransform.position;
        }

        if (Input.GetMouseButton(2))
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


    //カメラの回転 マウス
    private void CameraRotationKey(float f)
    {
        var headpos = barracudaRunner.GetHeadPosition();
        camTransform.RotateAround(headpos, Vector3.up, f * Time.deltaTime * 25);
    }

    float ang = 0f;

    //カメラの回転 自動
    private void CameraRotationAuto(float f)
    {
        var headpos = barracudaRunner.GetHeadPosition();
        ang += Time.deltaTime * 25;
        camTransform.RotateAround(headpos, Vector3.up, f * Time.deltaTime * 25);
        if (ang > 360)
        {
            ang = 0;
            rightRot = false;
            leftRot = false;
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
        if (Input.GetKey(KeyCode.Z)) { CameraRotationKey(1f);return; }
        if (Input.GetKey(KeyCode.C)) { CameraRotationKey(-1f); return; }
        camTransform.position = campos;

        if (!KetDownMarginFlag)
        {
            if (Input.GetKey(KeyCode.P)) { uiScript.NextAvatar(); KetDownMarginFlag = true; KetDownMargin = 0f; }

            var headpos = barracudaRunner.GetHeadPosition();
            if (Input.GetKey(KeyCode.Alpha1))
            {
                ang = 0;
                rightRot = false;
                leftRot = false;
                mainCamera.fieldOfView = 60;
                campos = initialCamposition;
                camTransform.rotation = initialCamRotation;
                camTransform.position = campos;
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                mainCamera.fieldOfView = 60;
                this.transform.LookAt(headpos);
                camTransform.position = new Vector3(headpos.x - 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                mainCamera.fieldOfView = 60;
                this.transform.LookAt(headpos);
                camTransform.position = new Vector3(headpos.x + 0.5f, headpos.y, headpos.z - 0.5f);
                this.transform.LookAt(headpos);
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                mainCamera.fieldOfView = 60;
                this.transform.LookAt(headpos);
                camTransform.position = new Vector3(headpos.x, headpos.y, headpos.z - 0.8f);
                this.transform.LookAt(headpos);
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
            else if(Input.GetKey(KeyCode.Alpha5))
            {
                ang = 0;
                rightRot = !rightRot;
                leftRot = false;
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                ang = 0;
                leftRot = !leftRot;
                rightRot = false;
                KetDownMarginFlag = true;
                KetDownMargin = 0f;
            }
        }

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