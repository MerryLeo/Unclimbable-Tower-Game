// Camera rotation using mouse inputs

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public bool CameraEnabled;

    [Header("Mouse Settings")]
    [Range(0, 1.5f)] [SerializeField]
    float horizontalSensitivty = 0.5f;
    [Range(0, 1.5f)] [SerializeField]
    float verticalSensitivty = 0.5f;

    const float maxVerticalRotation = 80f;
    float mouseX, mouseY;
    new Camera camera;
    const float fovSpeed = 0.02f;
    float defaultFOV, currentFOV, targetFOV, count;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        count = 0;
        defaultFOV = currentFOV = targetFOV = camera.fieldOfView;
        CameraEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraEnabled)
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

    public void SetFOV(float fov)
    {
        targetFOV = fov;
        count = 0;
    }

    public void ResetFOV()
    {
        targetFOV = defaultFOV;
        count = 0;
    }
}
