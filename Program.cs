using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

namespace YuchiGames.PrimitierDesktop
{
    public class Program : MelonMod
    {
        bool _initialized = false;
        GameObject _leftHandController = new GameObject();
        GameObject _rightHandController = new GameObject();
        GameObject _mainCamera = new GameObject();
        GameObject _xrOrigin = new GameObject();
        PlayerMovement _playerMovement = new PlayerMovement();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
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

            _xrOrigin = GameObject.Find("/Player/XR Origin");
            //InputActionMap inputActionMap = _xrOrigin.GetComponent<PlayerInput>()
            //    .actions.actionMaps[0];
            //// Move
            //inputActionMap.actions[0].AddBinding("<GamePad>/leftStick");
            //inputActionMap.actions[0].AddCompositeBinding("2DVector")
            //    .With("Up", "<Keyboard>/W")
            //    .With("Down", "<Keyboard>/S")
            //    .With("Left", "<Keyboard>/A")
            //    .With("Right", "<Keyboard>/D");
            //// Jump
            //inputActionMap.actions[4].AddBinding("<Keyboard>/Space");
            //// Grab
            //inputActionMap.actions[7].AddBinding("<Keyboard>/H");
            //inputActionMap.actions[8].AddBinding("<Keyboard>/J");
            //// Bond
            //inputActionMap.actions[9].AddBinding("<Keyboard>/T");
            //inputActionMap.actions[10].AddBinding("<Keyboard>/Y");
            //// Menu
            //inputActionMap.actions[11].AddBinding("<Keyboard>/N");
            //inputActionMap.actions[12].AddBinding("<Keyboard>/M");
            //// Separate
            //inputActionMap.actions[13].AddBinding("<Keyboard>/U");
            //inputActionMap.actions[14].AddBinding("<Keyboard>/I");

            //MelonLogger.Msg($"InputActionMapName: {inputActionMap.enabled}, {inputActionMap.name}");
            //foreach (InputAction action in inputActionMap.actions)
            //{
            //    MelonLogger.Msg($"InputActionName: {action.enabled}, {action.name}");
            //    foreach (InputBinding binding in action.bindings)
            //    {
            //        MelonLogger.Msg($"Binding: {binding.path}");
            //    }
            //}
            //foreach (InputDevice device in InputSystem.devices)
            //{
            //    MelonLogger.Msg($"Device: {device.name}");
            //}

            _playerMovement = _xrOrigin.GetComponent<PlayerMovement>();

            _initialized = true;
        }

        Vector3 _leftHandMove = new Vector3(-0.1f, -0.2f, 0.15f);
        Vector3 _leftHandRot = new Vector3(270f, 0f, 0f);
        Vector3 _rightHandMove = new Vector3(0.1f, -0.2f, 0.15f);
        Vector3 _rightHandRot = new Vector3(270f, 0f, 0f);
        bool _hideMouse = false;
        bool _isEscape = false;

        bool _isMovementKeyPressed = false;
        Vector2Int _moveAxis = new Vector2Int(0, 0);

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

            if (Input.GetKeyDown(KeyCode.W))
            {
                _isMovementKeyPressed = true;
                _moveAxis = Vector2Int.up;
            }
            else
            {
                _isMovementKeyPressed = false;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                _isMovementKeyPressed = true;
                _moveAxis = Vector2Int.down;
            }
            else
            {
                _isMovementKeyPressed = false;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                _isMovementKeyPressed = true;
            }
            else
            {
                _isMovementKeyPressed = false;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                _isMovementKeyPressed = true;
            }
            else
            {
                _isMovementKeyPressed = false;
            }
        }

        public override void OnFixedUpdate()
        {
            if (!_initialized)
                return;

            if (_isMovementKeyPressed)
            {
                _playerMovement.Move(_moveAxis);
            }
        }

        public void SwitchHideMouse(bool isHideButton)
        {
            _hideMouse = isHideButton;
            Cursor.lockState = _hideMouse ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_hideMouse;
        }
    }
}
