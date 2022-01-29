// Camera rotation using mouse inputs

using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Mouse Settings")]
    [Range(0, 1.5f)] [SerializeField]
    float horizontalSensitivty = 0.5f;
    [Range(0, 1.5f)] [SerializeField]
    float verticalSensitivty = 0.5f;

    [SerializeField]
    PlayerController_FSM controller;
    GameManager manager;

    const float maxVerticalRotation = 80f;
    float mouseX, mouseY;
    new Camera camera;
    const float fovSpeed = 0.02f;
    float defaultFOV, currentFOV, targetFOV, count;
    const float runningFov = 80f;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        controller.StateChanged += OnPlayerStateChanged;
        count = 0;
        defaultFOV = currentFOV = targetFOV = camera.fieldOfView;
        active = true;
        Cursor.lockState = CursorLockMode.Locked;

        // Add Listeners to the Game Manager
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        manager.PauseEvent += OnGamePaused;
        manager.GameOverEvent += OnGamePaused;
        manager.WonEvent += OnGamePaused;
        manager.ResumeEvent += OnGameResumed;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Rotate the camera
            mouseX = Input.GetAxis("Mouse X") * horizontalSensitivty;
            mouseY = Input.GetAxis("Mouse Y") * verticalSensitivty;
            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(-transform.right, mouseY, Space.World);

            // Lock the camera
            float verticalRotation = transform.localEulerAngles.x > 90 ? transform.localEulerAngles.x - 360f : transform.localEulerAngles.x;
            if (verticalRotation > maxVerticalRotation)
                transform.localRotation = Quaternion.Euler(maxVerticalRotation, transform.localEulerAngles.y, 0);
            else if (verticalRotation < -maxVerticalRotation)
                transform.localRotation = Quaternion.Euler(-maxVerticalRotation, transform.localEulerAngles.y, 0);
        }

        // Modify FOV using linear interpolation
        if (currentFOV != targetFOV)
        {
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, count);
            camera.fieldOfView = currentFOV;
            count += fovSpeed * Time.deltaTime;
        }
    }

    // Change Player FOV when Running
    public void OnPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
    {
        if (e.NewState is PlayerRunningState)
            SetFOV(runningFov);
            
        else if (e.PreviousState is PlayerRunningState)
            ResetFOV();
    }

    void SetFOV(float fov)
    {
        targetFOV = fov;
        count = 0;
    }

    void ResetFOV()
    {
        targetFOV = defaultFOV;
        count = 0;
    }

    void OnGamePaused(object sender, EventArgs e)
    {
        active = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnGameResumed(object sender, EventArgs e)
    {
        active = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
