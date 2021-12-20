

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MenuCameraController : MonoBehaviour
{
    [Header("Camera Transforms")]
    [SerializeField]
    Vector3 bottomPosition = new Vector3(0, 0.85f, -11);
    [SerializeField]
    Vector3 topPosition = new Vector3(0, 18, -7.5f);

    [SerializeField]
    Vector3 fallingRotation = new Vector3(-5, 0, 0);
    [SerializeField]
    Vector3 risingRotation = new Vector3(-35f, 0, 0);

    public Vector3 BottomPos => bottomPosition;
    public Vector3 TopPos => topPosition;
    public Vector3 FallRot => fallingRotation;
    public Vector3 RiseRot => risingRotation;

    float positionLerpValue = 0, rotationLerpValue = 0.5f;
    float targetRotationLerp = 0.5f;
    // float positionLerpValue = 0, rotationLerpValue = 0.5f;
    const float minMouseYThreshold = 0.2f, maxMouseYThreshold = 0.8f, lerpSpeed = 0.3f, rotationSpeed = 0.8f;
    const float camRotation = 25f;
    Quaternion minCamRotation, maxCamRotation;

    // Start is called before the first frame update
    void Start()
    {
        minCamRotation = Quaternion.Euler(fallingRotation);
        maxCamRotation = Quaternion.Euler(risingRotation);
    }

    // Update is called once per frame
    void Update()
    {
        // Increment or decrement lerpValue with vertical mouse position
        float mouseY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        if (mouseY > maxMouseYThreshold)
        {
            positionLerpValue += UtilityClass.Remap(mouseY, maxMouseYThreshold, 1f, 0, 1f) * lerpSpeed * Time.deltaTime;
        }
        else if (mouseY < minMouseYThreshold)
        {
            positionLerpValue -= UtilityClass.Remap(mouseY, minMouseYThreshold, 0, 0, 1f) * lerpSpeed * Time.deltaTime;
        }
        positionLerpValue = Mathf.Clamp01(positionLerpValue);
        
        // Increment or decrement the current rotation lerp value
        targetRotationLerp = ((mouseY < maxMouseYThreshold && mouseY > minMouseYThreshold) || (positionLerpValue == 0 || positionLerpValue == 1)) ? 0.5f : mouseY;
        if (rotationLerpValue < targetRotationLerp)
        {
            rotationLerpValue += (targetRotationLerp - rotationLerpValue) * rotationSpeed * Time.deltaTime;
        }
        else if (rotationLerpValue > targetRotationLerp)
        {
            rotationLerpValue -= (rotationLerpValue - targetRotationLerp) * rotationSpeed * Time.deltaTime;
        }

        // Update position and rotation using lerpValue and currentRotationLerp
        transform.position = Vector3.Lerp(bottomPosition, topPosition, positionLerpValue);
        transform.rotation = Quaternion.Lerp(minCamRotation, maxCamRotation, rotationLerpValue);
        
    }
}
