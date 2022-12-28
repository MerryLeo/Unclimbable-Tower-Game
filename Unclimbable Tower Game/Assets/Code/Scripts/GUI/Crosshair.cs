using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour 
{
    [SerializeField] Image HandPickImg;
    [SerializeField] Image HandHoldingImg;
    [SerializeField] Image HandRotationImg;
    [SerializeField] Image DefaultHandImg;
    Image[] _images;
    PickUp _pickupScript;

    void Start() 
    {
        _pickupScript = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        _pickupScript.PickupStateChanged += OnPickupStateChanged;
        _images = new Image[4] { HandPickImg, HandHoldingImg, HandRotationImg, DefaultHandImg };
        SetActiveImage(DefaultHandImg);
    }

    // Change Current Crosshair with pickup state
    void OnPickupStateChanged(object sender, PickupStateEventArgs e) 
    {
        switch(e.NewState) {
            case PickupState.HOLDINGHEAVYOBJECT:
                SetActiveImage(HandHoldingImg);
                break;
            case PickupState.HOLDINGLIGHTOBJECT:
                SetActiveImage(HandHoldingImg);
                break;
            case PickupState.OBJECTINSIGHT:
                SetActiveImage(HandPickImg);
                break;
            case PickupState.ROTATINGLIGHTOBJECT:
                SetActiveImage(HandRotationImg);
                break;
            default:
                SetActiveImage(DefaultHandImg);
                break;
        }
    }

    // Set Active img and set inactive all others
    void SetActiveImage(Image img) 
    {
        foreach(Image image in _images) 
            image.enabled = img == image;
        
    }
}
