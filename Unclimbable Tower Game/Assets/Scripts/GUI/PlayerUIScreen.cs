// Script to change UI panels visibility and configure a Timer

using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScreen : MonoBehaviour {
    
    [Header("UI Game Objects")]
    [SerializeField] GameObject crosshairObj;
    [SerializeField] GameObject timerObj;
    [SerializeField] GameObject pauseScreenObj;
    [SerializeField] GameObject optionScreenObj;
    [SerializeField] GameObject winScreenObj;
    [SerializeField] GameObject gameOverScreenObj;
    [SerializeField] Timer timer;
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
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(true);
        gameOverScreenObj.SetActive(false);
    }

    void OnPlayerOutOfSpawn(object sender, EventArgs e) {
        timer.InitializeTimer();
        timer.StartTimer();
    }

    void OnGamePaused(object sender, EventArgs e) {
        // Timer
        if (timer.Initialised)
            timer.StartTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(true);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameResumed(object sender, EventArgs e) {
        // Timer
        if (timer.Initialised)
            timer.StartTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(true);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameWon(object sender, EventArgs e) {
        // Timer
        if (timer.Initialised)
            timer.StartTimer();

        // UI Configuration
        winScreenObj.SetActive(true);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameOver(object sender, EventArgs e) {
        // Timer
        if (timer.Initialised)
            timer.StartTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(true);
    }
}
