using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ControlLabel : MonoBehaviour 
{
    PickUp _playerPickup;
    Text _text;
    string[] _controlTexts;

    void Awake() 
    {
        _text = GetComponent<Text>();
        _playerPickup = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        _playerPickup.PickupStateChanged += OnPickupStateChanged;
        _controlTexts = new string[4] { 
            "", 
            "Left Mouse: Pickup", 
            "Right Mouse: Throw, R Key: Rotate", 
            "Too heavy cannot lift, rotate or throw" 
            };
        _text.text = _controlTexts[0];
    }

    void OnPickupStateChanged(object sender, PickupStateEventArgs e) 
    {
        switch (e.NewState) {
            case PickupState.OBJECTINSIGHT:
                _text.text = _controlTexts[1];
                break;
            case PickupState.HOLDINGLIGHTOBJECT:
                _text.text = _controlTexts[2];
                break;
            case PickupState.HOLDINGHEAVYOBJECT:
                _text.text = _controlTexts[3];
                break;
            default:
                _text.text = _controlTexts[0];
                break;
        }
    }
}
