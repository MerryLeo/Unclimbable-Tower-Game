// Disables this game object when the player leaves its collider

using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Material))]
public class SpawnArea : MonoBehaviour {
    public event EventHandler PlayerOutOfSpawnEvent;
    GameObject _playerObj;
    Material _mat;
    const string _playerTag = "Player";

    // Start is called before the first frame update
    void Start() {
        _playerObj = GameObject.FindGameObjectWithTag(_playerTag);
        _mat = GetComponent<Renderer>().sharedMaterial;
    }

    // Material Opacity with Player Distance from its center
    void Update() {
        float dst = Vector3.Distance(_playerObj.transform.position, gameObject.transform.position);
        _mat.SetFloat("_Opacity", dst);
    }

    // Raise PlayerOutOfSpawnEvent
    void OnTriggerExit(Collider other) {
        if (other.tag == _playerTag) {
            gameObject.SetActive(false);
            EventHandler handler = PlayerOutOfSpawnEvent;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
