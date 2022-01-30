
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {
    [SerializeField]
    Image _handPickImg;
    [SerializeField]
    Image _handHoldingImg;
    [SerializeField]
    Image _handRotationImg;
    [SerializeField]
    Image _defaultHandImg;
    Image[] _images;
    PickUp _pickupScript;

    // Start is called before the first frame update
    void Start() {
        _pickupScript = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        _pickupScript.PickupStateChanged += OnPickupStateChanged;
        _images = new Image[4] { _handPickImg, _handHoldingImg, _handRotationImg, _defaultHandImg };
        SetActiveImage(_defaultHandImg);
    }

    // Change Current Crosshair with pickup state
    void OnPickupStateChanged(object sender, PickupStateEventArgs e) {
        Debug.Log($"New Pickup State is: {e.NewState}");
        switch(e.NewState) {
            case PickupState.HOLDINGHEAVYOBJECT:
                SetActiveImage(_handHoldingImg);
                break;
            case PickupState.HOLDINGLIGHTOBJECT:
                SetActiveImage(_handHoldingImg);
                break;
            case PickupState.OBJECTINSIGHT:
                SetActiveImage(_handPickImg);
                break;
            case PickupState.ROTATINGLIGHTOBJECT:
                SetActiveImage(_handRotationImg);
                break;
            default:
                SetActiveImage(_defaultHandImg);
                break;
        }
    }

    // Set Active img and set inactive all others
    void SetActiveImage(Image img) {
        foreach(Image image in _images) {
            image.enabled = img == image;
        }
    }
}
