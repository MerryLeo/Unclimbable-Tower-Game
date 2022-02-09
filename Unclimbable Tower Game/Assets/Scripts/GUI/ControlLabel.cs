using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class ControlLabel : MonoBehaviour {
    PickUp playerPickup;
    Text text;
    string[] controlTexts;

    void Awake() {
        text = GetComponent<Text>();
        playerPickup = GameObject.Find("Player").GetComponentInChildren<PickUp>();
        playerPickup.PickupStateChanged += OnPickupStateChanged;
        controlTexts = new string[4] { "", "Left Mouse: Pickup", "Right Mouse: Throw, R Key: Rotate", "Too heavy cannot lift, rotate or throw" };
        text.text = controlTexts[0];
    }

    void OnPickupStateChanged(object sender, PickupStateEventArgs e) {
        switch (e.NewState) {
            case PickupState.OBJECTINSIGHT:
                text.text = controlTexts[1];
                break;
            case PickupState.HOLDINGLIGHTOBJECT:
                text.text = controlTexts[2];
                break;
            case PickupState.HOLDINGHEAVYOBJECT:
                text.text = controlTexts[3];
                break;
            default:
                text.text = controlTexts[0];
                break;
        }
    }
}
