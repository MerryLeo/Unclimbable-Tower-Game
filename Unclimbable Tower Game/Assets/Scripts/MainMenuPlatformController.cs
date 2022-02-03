// Update the postion and rotation of the secondary platform in the mainmenu by interpolating between two nodes

using UnityEngine;

[SelectionBase]
public class MainMenuPlatformController : MonoBehaviour {
    [SerializeField] Transform node1;
    [SerializeField] Transform node2;
    [SerializeField] Transform cameraTrans;
    Vector3 _bottomCameraPos, _topCameraPos;
    float _targetLerp = 0, _lerpValue = 0, _waveTime = 0;
    const float _lerpSpeed = 1.25f, _verticalSpeed = 1.45f, _verticalVariation = 0.35f;
    const float _sphereRadius = 0.75f;
    MenuCameraController _cameraScript;    
    void Start() {
        _cameraScript = cameraTrans.GetComponent<MenuCameraController>();
        _bottomCameraPos = _cameraScript.BottomPos;
        _topCameraPos = _cameraScript.TopPos;
    }

    void Update() {
        _targetLerp = Vector3.Distance(cameraTrans.position, _bottomCameraPos) / Vector3.Distance(_bottomCameraPos, _topCameraPos);
        if (_lerpValue != _targetLerp) {
            if (_lerpValue < _targetLerp)
                _lerpValue += (_targetLerp - _lerpValue) * _lerpSpeed * Time.deltaTime;
            else
                _lerpValue -= (_lerpValue - _targetLerp) * _lerpSpeed * Time.deltaTime;
            
            transform.position = Vector3.Lerp(node1.position, node2.position, _lerpValue) + Vector3.up * _verticalVariation * Mathf.Sin(_waveTime);
            transform.rotation = Quaternion.Lerp(node1.rotation, node2.rotation, _lerpValue);
            _waveTime += _verticalSpeed * Time.deltaTime;
        }
    }

    void OnDrawGizmos() {
        Gizmos gizmos = new Gizmos();
        gizmos.DrawLineWithSpheres(node1.position, node2.position, new Color(0.5f, 0.5f, 0f, 0.85f), new Color(1f, 1f, 1f, 1f), _sphereRadius);
    }
}
