// Camera rotation using mouse inputs

using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour 
{
    const float FOV_SPD = 0.02f, RUNNING_FOV = 80f, MAV_VERTICAL_ROT = 80f;
    const string GAME_MANAGER_NAME = "GameManager", SAVE_SYSTEM_NAME = "SaveSystem", MOUSE_X_NAME = "Mouse X", MOUSE_Y_NAME = "Mouse Y";

    [Header("Mouse Settings")]
    [SerializeField] PlayerController playerController;
    GameManager gameManager;
    SaveSystem saveSystem;
    float mouseX, mouseY;
    Camera playerCam;
    float defaultFov, currentFov, targetFov, count, mouseSensitivity = 0.5f;
    bool active;

    // Initialize Variables and Add Listeners to Game Manager
    void Start() 
    {
        playerCam = GetComponent<Camera>();
        playerController.StateChanged += OnPlayerStateChanged;
        count = 0;
        defaultFov = currentFov = targetFov = playerCam.fieldOfView;
        active = true;
        Cursor.lockState = CursorLockMode.Locked;

        // Add Listeners to the Game Manager
        gameManager = GameObject.Find(GAME_MANAGER_NAME).GetComponent<GameManager>();
        gameManager.PauseEvent += OnGamePaused;
        gameManager.GameOverEvent += OnGamePaused;
        gameManager.WonEvent += OnGamePaused;
        gameManager.ResumeEvent += OnGameResumed;
        

        // Set Mouse Sensitivity
        saveSystem = GameObject.Find(SAVE_SYSTEM_NAME).GetComponent<SaveSystem>();
        saveSystem.GameSettingLoaded += OnGameSettingUpdated;
    }

    // Modify Camera Rotation and Camera FOV
    void Update() 
    {
        if (active) 
        {
            // Rotate the camera
            mouseX = Input.GetAxis(MOUSE_X_NAME) * mouseSensitivity;
            mouseY = Input.GetAxis(MOUSE_Y_NAME) * mouseSensitivity;
            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(-transform.right, mouseY, Space.World);

            // Lock the camera
            float verticalRotation = transform.localEulerAngles.x > 90 ? transform.localEulerAngles.x - 360f : transform.localEulerAngles.x;
            if (verticalRotation > MAV_VERTICAL_ROT)
                transform.localRotation = Quaternion.Euler(MAV_VERTICAL_ROT, transform.localEulerAngles.y, 0);
            else if (verticalRotation < -MAV_VERTICAL_ROT)
                transform.localRotation = Quaternion.Euler(-MAV_VERTICAL_ROT, transform.localEulerAngles.y, 0);
        }

        // Modify FOV using linear interpolation
        if (currentFov != targetFov) 
        {
            currentFov = Mathf.Lerp(currentFov, targetFov, count);
            playerCam.fieldOfView = currentFov;
            count += FOV_SPD * Time.deltaTime;
        }
    }

    // Change Player FOV when Running
    public void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e) 
    {
        if (e.NewState is PlayerRunningState)
            SetFOV(RUNNING_FOV);
            
        else if (e.PreviousState is PlayerRunningState)
            ResetFOV();
    }

    // Set Target FOV
    void SetFOV(float fov) 
    {
        targetFov = fov;
        count = 0;
    }

    // Reset Target FOV
    void ResetFOV() 
    {
        targetFov = defaultFov;
        count = 0;
    }
    
    // Update Mouse Sensitivity
    void OnGameSettingUpdated(object sender, GameSettingEventArgs e) 
    {
        mouseSensitivity = e.Setting.MouseSensitivity;
    }

    // Unlock Mouse
    public void OnGamePaused(object sender, EventArgs e) 
    {
        active = false;
        Cursor.lockState = CursorLockMode.None;
    }

    // Lock Mouse
    public void OnGameResumed(object sender, EventArgs e) 
    {
        active = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
