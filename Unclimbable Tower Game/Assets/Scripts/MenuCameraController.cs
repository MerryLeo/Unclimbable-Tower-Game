using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MenuCameraController : MonoBehaviour 
{
    const float SPHERE_RADIUS = 0.75f;
    const float MIN_MOUSE_Y_THRESHOLD = 0.2f, MAX_MOUSE_Y_THRESHOLD = 0.8f, LERP_SPD = 0.3f, ROT_SPD = 0.8f;
    const float CAM_ROT = 25f;

    [Header("Camera Transforms")]
    [SerializeField] Vector3 bottomPosition = new Vector3(0, 0.85f, -11);
    [SerializeField] Vector3 topPosition = new Vector3(0, 18, -7.5f);
    [SerializeField] Vector3 fallingRotation = new Vector3(-5, 0, 0);
    [SerializeField] Vector3 risingRotation = new Vector3(-35f, 0, 0);
    public Vector3 BottomPos => bottomPosition;
    public Vector3 TopPos => topPosition;
    public Vector3 FallRot => fallingRotation;
    public Vector3 RiseRot => risingRotation;
    
    float _positionLerpValue = 0, _rotationLerpValue = 0.5f;
    float _targetRotationLerp = 0.5f;
    // float positionLerpValue = 0, rotationLerpValue = 0.5f;
    
    Quaternion _minCamRotation, _maxCamRotation;
    void Start() 
    {
        Time.timeScale = 1;
        _minCamRotation = Quaternion.Euler(fallingRotation);
        _maxCamRotation = Quaternion.Euler(risingRotation);
    }

    void Update() 
    {
        // Increment or decrement lerpValue with vertical mouse position
        float mouseY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        if (mouseY > MAX_MOUSE_Y_THRESHOLD) 
            _positionLerpValue += mouseY.Remap(MAX_MOUSE_Y_THRESHOLD, 1f, 0, 1f) * LERP_SPD * Time.deltaTime;
        
        else if (mouseY < MIN_MOUSE_Y_THRESHOLD) 
            _positionLerpValue -= mouseY.Remap(MIN_MOUSE_Y_THRESHOLD, 0, 0, 1f) * LERP_SPD * Time.deltaTime;
        
        _positionLerpValue = Mathf.Clamp01(_positionLerpValue);
        // Increment or decrement the current rotation lerp value
        _targetRotationLerp = ((mouseY < MAX_MOUSE_Y_THRESHOLD && mouseY > MIN_MOUSE_Y_THRESHOLD) || (_positionLerpValue == 0 || _positionLerpValue == 1)) ? 0.5f : mouseY;
        if (_rotationLerpValue < _targetRotationLerp)
            _rotationLerpValue += (_targetRotationLerp - _rotationLerpValue) * ROT_SPD * Time.deltaTime;

        else if (_rotationLerpValue > _targetRotationLerp)
            _rotationLerpValue -= (_rotationLerpValue - _targetRotationLerp) * ROT_SPD * Time.deltaTime;

        // Update position and rotation using lerpValue and currentRotationLerp
        transform.position = Vector3.Lerp(bottomPosition, topPosition, _positionLerpValue);
        transform.rotation = Quaternion.Lerp(_minCamRotation, _maxCamRotation, _rotationLerpValue);
    }

    void OnDrawGizmos() 
    {
        Gizmos gizmos = new Gizmos();
        gizmos.DrawLineWithSpheres(bottomPosition, topPosition, new Color(0.5f, 0.5f, 0f, 0.85f), new Color(1f, 1f, 1f, 1f), SPHERE_RADIUS);
    }
}
