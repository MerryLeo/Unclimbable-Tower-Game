// Update the postion and rotation of the secondary platform in the mainmenu by interpolating between two nodes

using UnityEngine;

[SelectionBase]
public class MainMenuPlatformController : MonoBehaviour
{
    [SerializeField]
    Transform node1;
    [SerializeField]
    Transform node2;

    [SerializeField]
    Transform cameraTrans;

    Vector3 bottomCameraPos, topCameraPos;

    float targetLerp = 0, lerpValue = 0, waveTime = 0;
    const float lerpSpeed = 1.25f, verticalSpeed = 1.45f, verticalVariation = 0.35f;
    MenuCameraController cameraScript;

    // Start is called before the first frame
    void Start() 
    {
        cameraScript = cameraTrans.GetComponent<MenuCameraController>();
        bottomCameraPos = cameraScript.BottomPos;
        topCameraPos = cameraScript.TopPos;
    }

    // Update is called once per frame
    void Update()
    {
        targetLerp = Vector3.Distance(cameraTrans.position, bottomCameraPos) / Vector3.Distance(bottomCameraPos, topCameraPos);
        if (lerpValue != targetLerp)
        {
            if (lerpValue < targetLerp)
                lerpValue += (targetLerp - lerpValue) * lerpSpeed * Time.deltaTime;
            else
                lerpValue -= (lerpValue - targetLerp) * lerpSpeed * Time.deltaTime;
            
            transform.position = Vector3.Lerp(node1.position, node2.position, lerpValue) + Vector3.up * verticalVariation * Mathf.Sin(waveTime);
            transform.rotation = Quaternion.Lerp(node1.rotation, node2.rotation, lerpValue);
            waveTime += verticalSpeed * Time.deltaTime;
        }
    }
}
