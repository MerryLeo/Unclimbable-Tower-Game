// Goal of the game

using UnityEngine;

public class Goal : MonoBehaviour {
    GameManager _gameManager;
    const string _playerTag = "Player", _gameManagerName = "GameManager";
    void Start() {
        _gameManager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
    }

    // Call Win if the player touches this object
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == _playerTag) {
            gameObject.SetActive(false);
            _gameManager.Win();
        }
    }
}
