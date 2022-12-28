// Update the postion and rotation of the secondary platform in the mainmenu by interpolating between two nodes

using UnityEngine;

[SelectionBase]
public class MainMenuPlatformController : MonoBehaviour 
{
    const float LERP_SPD = 1.25f, VERTICAL_SPD = 1.45f, VERTICAL_VARIATION = 0.35f;
    const float SPHERE_RADIUS = 0.75f;
    [SerializeField] Transform node1, node2, cameraTrans;
    Vector3 _bottomCameraPos, _topCameraPos;
    float _targetLerp = 0, _lerpValue = 0, _waveTime = 0;
    MenuCameraController _cameraScript;
    void Start() 
    {
        _cameraScript = cameraTrans.GetComponent<MenuCameraController>();
        _bottomCameraPos = _cameraScript.BottomPos;
        _topCameraPos = _cameraScript.TopPos;
    }

    void Update() 
    {
        _targetLerp = Vector3.Distance(cameraTrans.position, _bottomCameraPos) / Vector3.Distance(_bottomCameraPos, _topCameraPos);
        if (_lerpValue != _targetLerp) 
        {
            if (_lerpValue < _targetLerp)
                _lerpValue += (_targetLerp - _lerpValue) * LERP_SPD * Time.deltaTime;
            else
                _lerpValue -= (_lerpValue - _targetLerp) * LERP_SPD * Time.deltaTime;
            
            transform.position = Vector3.Lerp(node1.position, node2.position, _lerpValue) + Vector3.up * VERTICAL_VARIATION * Mathf.Sin(_waveTime);
            transform.rotation = Quaternion.Lerp(node1.rotation, node2.rotation, _lerpValue);
            _waveTime += VERTICAL_SPD * Time.deltaTime;
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos gizmos = new Gizmos();
        gizmos.DrawLineWithSpheres(node1.position, node2.position, new Color(0.5f, 0.5f, 0f, 0.85f), new Color(1f, 1f, 1f, 1f), SPHERE_RADIUS);
    }
}
