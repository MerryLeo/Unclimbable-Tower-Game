
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    Image handPickImg;
    [SerializeField]
    Image handHoldingImg;
    [SerializeField]
    Image handRotatingImg;
    [SerializeField]
    Image defaultHandImg;

    PickUp pickupScript;

    // Start is called before the first frame update
    void Start() {
        pickupScript = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        defaultHandImg.enabled = true;
        handHoldingImg.enabled = false;
        handPickImg.enabled = false;
        handRotatingImg.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (pickupScript.PickableObjectInSight) { // Pickable Object Hand
            defaultHandImg.enabled = false;
            handHoldingImg.enabled = false;
            handPickImg.enabled = true;
            handRotatingImg.enabled = false;
        } else if (pickupScript.RotatingTheObject) { // Rotating Object Hand
            defaultHandImg.enabled = false;
            handHoldingImg.enabled = false;
            handPickImg.enabled = false;
            handRotatingImg.enabled = true;
        } else if (pickupScript.HoldingLightObject || pickupScript.HoldingHeavyObject) { // Grabing Object Hand
            defaultHandImg.enabled = false;
            handHoldingImg.enabled = true;
            handPickImg.enabled = false;
            handRotatingImg.enabled = false;
        } else { // Default Hand
            defaultHandImg.enabled = true;
            handHoldingImg.enabled = false;
            handPickImg.enabled = false;
            handRotatingImg.enabled = false;
        }
    }
}
