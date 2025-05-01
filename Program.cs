using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

namespace YuchiGames.PrimitierDesktop
{
    public class Program : MelonMod
    {
        bool _initialized = false;
        GameObject? _leftHandController;
        GameObject? _rightHandController;
        GameObject? _mainCamera;
        GameObject? _xrOrigin;
        PlayerMovement? _playerMovement;
        SphereCollider? _footCollider;

        float _airMoveMaxHeight;
        float _footFrictionOnStop;
        float _footFrictionOnMove;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Camera
            GameObject mainCanvas = GameObject.Find("/Player/XR Origin/Camera Offset/LeftHand Controller/RealLeftHand/MenuWindowL/Windows/MainCanvas");
            mainCanvas.transform.Find("CameraTab").gameObject.SetActive(false);
            mainCanvas.transform.Find("CameraTabButton").gameObject.SetActive(false);

            GameObject.Find("/HandyCamera/CameraBody").SetActive(false);
            foreach (Canvas canvas in GameObject.FindObjectsOfType<Canvas>(true))
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            _mainCamera = GameObject.Find("/Player/XR Origin/Camera Offset/Main Camera");
            _mainCamera.GetComponent<Camera>().fieldOfView = 90f;
            _mainCamera.GetComponent<TrackedPoseDriver>().enabled = false;
            _mainCamera.transform.localPosition = new Vector3(0f, 1.8f, 0f);

            // HandController
            _leftHandController = GameObject.Find("/Player/XR Origin/Camera Offset/LeftHand Controller");
            Grabber leftGrabber = GameObject.Find("/Player/LeftHand").GetComponent<Grabber>();
            Hand leftHand = leftGrabber.GetComponent<Hand>();
            leftHand.maximumForce = float.PositiveInfinity;
            leftHand.maximumTorque = float.PositiveInfinity;
            leftHand.positionSpring = 1000000f;
            leftHand.rotationSpring = 1000000f;

            _rightHandController = GameObject.Find("/Player/XR Origin/Camera Offset/RightHand Controller");
            Grabber rightGrabber = GameObject.Find("/Player/RightHand").GetComponent<Grabber>();
            Hand rightHand = rightGrabber.GetComponent<Hand>();
            rightHand.maximumForce = float.PositiveInfinity;
            rightHand.maximumTorque = float.PositiveInfinity;
            rightHand.positionSpring = 1000000f;
            rightHand.rotationSpring = 1000000f;

            // PlayerMovement
            _xrOrigin = GameObject.Find("/Player/XR Origin");
            _playerMovement = _xrOrigin.GetComponent<PlayerMovement>();
            _footCollider = _playerMovement.footCol;
            _airMoveMaxHeight = Traverse.Create(typeof(PlayerMovement)).Field("airMoveMaxHeight").GetValue<float>();
            _footFrictionOnStop = Traverse.Create(typeof(PlayerMovement)).Field("footFrictionOnStop").GetValue<float>();
            _footFrictionOnMove = Traverse.Create(typeof(PlayerMovement)).Field("footFrictionOnMove").GetValue<float>();

            _initialized = true;
        }

        Vector3 _leftHandMove = new Vector3(-0.1f, -0.2f, 0.15f);
        Vector3 _leftHandRot = new Vector3(270f, 0f, 0f);
        Vector3 _rightHandMove = new Vector3(0.1f, -0.2f, 0.15f);
        Vector3 _rightHandRot = new Vector3(270f, 0f, 0f);
        bool _hideMouse = false;
        bool _isEscape = false;

        Vector2Int _moveAxis = Vector2Int.zero;
        bool _isPressingWKey = false;
        bool _isPressingSKey = false;
        bool _isPressingAKey = false;
        bool _isPressingDKey = false;

        public override void OnUpdate()
        {
            if (!_initialized)
                return;

            _leftHandController.transform.position = _mainCamera.transform.TransformPoint(_leftHandMove);
            _leftHandController.transform.rotation = _mainCamera.transform.rotation * Quaternion.Euler(_leftHandRot);

            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButton(2))
                {
                    _leftHandRot += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));
                    return;
                }

                _leftHandMove += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.mouseScrollDelta.y) * 0.05f;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _leftHandMove = new Vector3(-0.1f, -0.2f, 0.15f);
                _leftHandRot = new Vector3(270f, 0f, 0f);
            }

            _rightHandController.transform.position = _mainCamera.transform.TransformPoint(_rightHandMove);
            _rightHandController.transform.rotation = _mainCamera.transform.rotation * Quaternion.Euler(_rightHandRot);

            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButton(2))
                {
                    _rightHandRot += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));
                    return;
                }

                _rightHandMove += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.mouseScrollDelta.y) * 0.05f;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                _rightHandMove = new Vector3(0.1f, -0.2f, 0.15f);
                _rightHandRot = new Vector3(270f, 0f, 0f);
            }

            if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)) && _hideMouse)
            {
                _xrOrigin.transform.rotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") + _xrOrigin.transform.rotation.eulerAngles.y, 0f);
            }
            if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)) && _hideMouse)
            {
                _mainCamera.transform.localRotation = Quaternion.Euler(-Input.GetAxis("Mouse Y") + _mainCamera.transform.localRotation.eulerAngles.x, 0f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isEscape = !_isEscape;
                SwitchHideMouse(!_hideMouse);
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (_isEscape)
                    SwitchHideMouse(true);
            }
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                if (!_isEscape)
                    SwitchHideMouse(false);
            }

            _isPressingWKey = Input.GetKey(KeyCode.W);
            _isPressingSKey = Input.GetKey(KeyCode.S);
            _isPressingAKey = Input.GetKey(KeyCode.A);
            _isPressingDKey = Input.GetKey(KeyCode.D);

            if (_isPressingWKey && _isPressingSKey)
            {
                _moveAxis.y = 0;
            }
            else if (_isPressingWKey)
            {
                _moveAxis.y = 1;
            }
            else if (_isPressingSKey)
            {
                _moveAxis.y = -1;
            }
            else
            {
                _moveAxis.y = 0;
            }

            if (_isPressingAKey && _isPressingDKey)
            {
                _moveAxis.x = 0;
            }
            else if (_isPressingAKey)
            {
                _moveAxis.x = -1;
            }
            else if (_isPressingDKey)
            {
                _moveAxis.x = 1;
            }
            else
            {
                _moveAxis.x = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float maxDistance = _airMoveMaxHeight + _footCollider.radius;
                Vector3 origin = _footCollider.transform.TransformPoint(_footCollider.center);
                bool isGrounded = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance);
                _playerMovement.Jump(isGrounded, hit);
            }
        }

        public override void OnFixedUpdate()
        {
            if (!_initialized)
                return;

            if (_playerMovement.isMoving)
            {
                _footCollider.material.dynamicFriction = _footFrictionOnMove;
                _footCollider.material.staticFriction = _footFrictionOnMove;
            }
            else
            {
                _footCollider.material.dynamicFriction = _footFrictionOnStop;
                _footCollider.material.staticFriction = _footFrictionOnStop;
            }

            _playerMovement.Move(_moveAxis);
            _playerMovement.MoveCollider();
            _playerMovement.PlayWaterSound();
            _playerMovement.SwitchGravity();
            _playerMovement.LimitHeight();
        }

        public void SwitchHideMouse(bool isHideButton)
        {
            _hideMouse = isHideButton;
            Cursor.lockState = _hideMouse ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_hideMouse;
        }
    }
}
