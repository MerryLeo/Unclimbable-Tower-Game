
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

    Image[] images;

    PickUp pickupScript;

    // Start is called before the first frame update
    void Start() {
        pickupScript = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        images = new Image[4] { handPickImg, handHoldingImg, handRotatingImg, defaultHandImg };
        SetImageActive(defaultHandImg);
    }

    // Update is called once per frame
    void Update() {

        switch(pickupScript.CurrentState) 
        {
            case PickupState.HOLDINGHEAVYOBJECT:
                SetImageActive(handHoldingImg);
                break;
            case PickupState.HOLDINGLIGHTOBJECT:
                SetImageActive(handHoldingImg);
                break;
            case PickupState.OBJECTINSIGHT:
                SetImageActive(handPickImg);
                break;
            case PickupState.ROTATINGOBJECT:
                SetImageActive(handRotatingImg);
                break;
            default:
                SetImageActive(defaultHandImg);
                break;
        }
    }

    void SetImageActive(Image img)
    {
        foreach(Image image in images)
        {
            image.enabled = (img == image) ? true : false;
        }
    }
}
