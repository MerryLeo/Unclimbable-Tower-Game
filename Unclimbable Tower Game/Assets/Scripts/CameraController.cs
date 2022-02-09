// Camera rotation using mouse inputs

using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    [Header("Mouse Settings")]
    [SerializeField] PlayerController_FSM _playerController;
    GameManager _gameManager;
    SaveSystem _saveSystem;
    float _mouseX, _mouseY;
    Camera _camera;
    const float _fovSpeed = 0.02f, _runningFOV = 80f, _maxVerticalRotation = 80f;
    const string _gameManagerName = "GameManager", _saveSystemName = "SaveSystem", _mouseXAxisName = "Mouse X", _mouseYAxisName = "Mouse Y";
    float _defaultFov, _currentFOV, _targetFOV, _count, _mouseSensitivity = 0.5f;
    bool _active;

    // Initialize Variables and Add Listeners to Game Manager
    void Start() {
        _camera = GetComponent<Camera>();
        _playerController.StateChanged += OnPlayerStateChanged;
        _count = 0;
        _defaultFov = _currentFOV = _targetFOV = _camera.fieldOfView;
        _active = true;
        Cursor.lockState = CursorLockMode.Locked;

        // Add Listeners to the Game Manager
        _gameManager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
        _gameManager.PauseEvent += OnGamePaused;
        _gameManager.GameOverEvent += OnGamePaused;
        _gameManager.WonEvent += OnGamePaused;
        _gameManager.ResumeEvent += OnGameResumed;

        // Set Mouse Sensitivity
        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _saveSystem.GameSettingLoaded += OnGameSettingUpdated;
    }

    // Modify Camera Rotation and Camera FOV
    void Update() {
        if (_active) {
            // Rotate the camera
            _mouseX = Input.GetAxis(_mouseXAxisName) * _mouseSensitivity;
            _mouseY = Input.GetAxis(_mouseYAxisName) * _mouseSensitivity;
            transform.Rotate(Vector3.up, _mouseX, Space.World);
            transform.Rotate(-transform.right, _mouseY, Space.World);

            // Lock the camera
            float verticalRotation = transform.localEulerAngles.x > 90 ? transform.localEulerAngles.x - 360f : transform.localEulerAngles.x;
            if (verticalRotation > _maxVerticalRotation)
                transform.localRotation = Quaternion.Euler(_maxVerticalRotation, transform.localEulerAngles.y, 0);
            else if (verticalRotation < -_maxVerticalRotation)
                transform.localRotation = Quaternion.Euler(-_maxVerticalRotation, transform.localEulerAngles.y, 0);
        }

        // Modify FOV using linear interpolation
        if (_currentFOV != _targetFOV) {
            _currentFOV = Mathf.Lerp(_currentFOV, _targetFOV, _count);
            _camera.fieldOfView = _currentFOV;
            _count += _fovSpeed * Time.deltaTime;
        }
    }

    // Change Player FOV when Running
    public void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e) {
        if (e.NewState is PlayerRunningState)
            SetFOV(_runningFOV);
            
        else if (e.PreviousState is PlayerRunningState)
            ResetFOV();
    }

    // Set Target FOV
    void SetFOV(float fov) {
        _targetFOV = fov;
        _count = 0;
    }

    // Reset Target FOV
    void ResetFOV() {
        _targetFOV = _defaultFov;
        _count = 0;
    }
    
    // Update Mouse Sensitivity
    void OnGameSettingUpdated(object sender, GameSettingEventArgs e) {
        _mouseSensitivity = e.Setting.MouseSensitivity;
    }

    // Unlock Mouse
    void OnGamePaused(object sender, EventArgs e) {
        _active = false;
        Cursor.lockState = CursorLockMode.None;
    }

    // Lock Mouse
    void OnGameResumed(object sender, EventArgs e) {
        _active = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
