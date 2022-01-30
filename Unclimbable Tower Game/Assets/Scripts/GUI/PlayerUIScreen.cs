// Script to change UI panels visibility and configure a Timer

using System;
using UnityEngine;

public class PlayerUIScreen : MonoBehaviour {
    // UI Elements
    [Header("UI Game Objects")]
    [SerializeField]
    GameObject _crosshairObj;
    [SerializeField]
    GameObject _timerObj;
    [SerializeField]
    GameObject _pauseScreenObj;
    [SerializeField]
    GameObject _optionScreenObj;
    [SerializeField]
    GameObject _winScreenObj;
    [SerializeField]
    GameObject _gameOverScreenObj;

    [SerializeField]
    Timer _timer;

    SpawnArea _spawn;
    GameManager _manager;
    const string _spawnTag = "SpawnArea", _gameManagerName = "GameManager";

    // Start is called before the first frame update
    void Start() {
        // Add Listeners to Game Manager
        _manager = GameObject.Find(_gameManagerName).GetComponent<GameManager>();
        _manager.PauseEvent += OnGamePaused;
        _manager.ResumeEvent += OnGameResumed;
        _manager.WonEvent += OnGameWon;
        _manager.GameOverEvent += OnGameOver;

        // Add Listener to Spawn Area
        _spawn = GameObject.FindGameObjectWithTag(_spawnTag).GetComponent<SpawnArea>();
        _spawn.PlayerOutOfSpawnEvent += OnPlayerOutOfSpawn;

        // Initial UI Configuration
        _winScreenObj.SetActive(false);
        _pauseScreenObj.SetActive(false);
        _crosshairObj.SetActive(true);
        _gameOverScreenObj.SetActive(false);
    }

    void OnPlayerOutOfSpawn(object sender, EventArgs e) {
        _timer.InitializeTimer();
        _timer.StartTimer();
    }

    void OnGamePaused(object sender, EventArgs e) {
        // Timer
        if (_timer.Initialised)
            _timer.StartTimer();

        // UI Configuration
        _winScreenObj.SetActive(false);
        _pauseScreenObj.SetActive(true);
        _crosshairObj.SetActive(false);
        _gameOverScreenObj.SetActive(false);
    }

    void OnGameResumed(object sender, EventArgs e) {
        // Timer
        if (_timer.Initialised)
            _timer.StartTimer();

        // UI Configuration
        _winScreenObj.SetActive(false);
        _pauseScreenObj.SetActive(false);
        _crosshairObj.SetActive(true);
        _gameOverScreenObj.SetActive(false);
    }

    void OnGameWon(object sender, EventArgs e) {
        // Timer
        if (_timer.Initialised)
            _timer.StartTimer();

        // UI Configuration
        _winScreenObj.SetActive(true);
        _pauseScreenObj.SetActive(false);
        _crosshairObj.SetActive(false);
        _gameOverScreenObj.SetActive(false);
    }

    void OnGameOver(object sender, EventArgs e) {
        // Timer
        if (_timer.Initialised)
            _timer.StartTimer();

        // UI Configuration
        _winScreenObj.SetActive(false);
        _pauseScreenObj.SetActive(false);
        _crosshairObj.SetActive(false);
        _gameOverScreenObj.SetActive(true);
    }
}
