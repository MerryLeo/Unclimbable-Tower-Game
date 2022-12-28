// Goal of the game

using UnityEngine;

public class Goal : MonoBehaviour 
{
    const string PLAYER_TAG = "Player", GAME_MANAGER_NAME = "GameManager";
    GameManager _gameManager;
    void Start() 
    {
        _gameManager = GameObject.Find(GAME_MANAGER_NAME).GetComponent<GameManager>();
    }

    // Call Win if the player touches this object
    void OnTriggerEnter(Collider collider) 
    {
        if (collider.gameObject.tag == PLAYER_TAG) 
        {
            gameObject.SetActive(false);
            _gameManager.Win();
        }
    }
}
