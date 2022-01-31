using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MenuCameraController : MonoBehaviour {
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
    const float _minMouseYThreshold = 0.2f, _maxMouseYThreshold = 0.8f, _lerpSpeed = 0.3f, _rotationSpeed = 0.8f;
    const float _camRotation = 25f;
    Quaternion _minCamRotation, _maxCamRotation;
    void Start() {
        _minCamRotation = Quaternion.Euler(fallingRotation);
        _maxCamRotation = Quaternion.Euler(risingRotation);
    }

    void Update() {
        // Increment or decrement lerpValue with vertical mouse position
        float mouseY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        if (mouseY > _maxMouseYThreshold)
        {
            _positionLerpValue += UtilityClass.Remap(mouseY, _maxMouseYThreshold, 1f, 0, 1f) * _lerpSpeed * Time.deltaTime;
        }
        else if (mouseY < _minMouseYThreshold)
        {
            _positionLerpValue -= UtilityClass.Remap(mouseY, _minMouseYThreshold, 0, 0, 1f) * _lerpSpeed * Time.deltaTime;
        }
        _positionLerpValue = Mathf.Clamp01(_positionLerpValue);
        
        // Increment or decrement the current rotation lerp value
        _targetRotationLerp = ((mouseY < _maxMouseYThreshold && mouseY > _minMouseYThreshold) || (_positionLerpValue == 0 || _positionLerpValue == 1)) ? 0.5f : mouseY;
        if (_rotationLerpValue < _targetRotationLerp) {
            _rotationLerpValue += (_targetRotationLerp - _rotationLerpValue) * _rotationSpeed * Time.deltaTime;
        } else if (_rotationLerpValue > _targetRotationLerp) {
            _rotationLerpValue -= (_rotationLerpValue - _targetRotationLerp) * _rotationSpeed * Time.deltaTime;
        }

        // Update position and rotation using lerpValue and currentRotationLerp
        transform.position = Vector3.Lerp(bottomPosition, topPosition, _positionLerpValue);
        transform.rotation = Quaternion.Lerp(_minCamRotation, _maxCamRotation, _rotationLerpValue);
        
    }
}
