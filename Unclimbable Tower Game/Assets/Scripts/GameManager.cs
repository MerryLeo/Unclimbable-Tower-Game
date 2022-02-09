using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField] float levelTime = 2f;
    public event EventHandler GameOverEvent;
    public event EventHandler PauseEvent;
    public event EventHandler ResumeEvent;
    public event EventHandler WonEvent;
    public float LevelTime => levelTime;
    GameObject _playerObj;
    int? _levelNumber; // Current Level Number
    bool _gameOver, _playerWon;
    SaveSystem _saveSystem;
    const float _lowerBound = -10f;
    const string _mainMenuSceneName = "Mainmenu", _playerName = "Player", _saveSystemName = "SaveSystem";
    const string _pauseInput = "Pause";
    void Start() {
        // Level Number is found in the name of the current Scene
        // Ex: if the scene is called Level 5 then level number is 5
        _levelNumber = SceneManager.GetActiveScene().name.FetchNumber();
        if (_levelNumber == null)
            _levelNumber = 1;

        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _gameOver = _playerWon = false;
        _playerObj = GameObject.Find(_playerName);
        Time.timeScale = 1;
    }

    void Update() {
        if (!_playerWon && !_gameOver && _playerObj.transform.position.y < _lowerBound) // Game Over
            GameOver();

        else if (Input.GetButtonDown(_pauseInput) && !_gameOver && !_playerWon) // Pause
            Pause();
    }


    public void Win() {
        EventHandler handler = WonEvent;
        handler?.Invoke(this, EventArgs.Empty);
        _playerWon = true;
        Time.timeScale = 0;

        // Save Game Data
        if (_saveSystem.Data.Level <= _levelNumber) {
            _saveSystem.Data.Level++;
            SaveSystem.SaveGame(_saveSystem.Data);
        }
    }

    public void GameOver() {
        EventHandler handler = GameOverEvent;
        handler?.Invoke(this, EventArgs.Empty);
        _gameOver = true;
        Time.timeScale = 0;
    }

    public void Pause() {
        EventHandler handler = PauseEvent;
        handler?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 0;
    }

    public void Resume() {
        EventHandler handler = ResumeEvent;
        handler?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene(_mainMenuSceneName, LoadSceneMode.Single);
    }

    // Restart Current Scene
    public static void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Load Level with a number
    public static void LoadLevel(int levelNumber) {
        SceneManager.LoadScene($"Level {levelNumber}", LoadSceneMode.Single);
    }

    // Load Next Level with current level number
    public static void NextLevel(int currentLevel) {
        SceneManager.LoadScene($"Level {currentLevel + 1}", LoadSceneMode.Single);
    }

    // Quit the application
    public static void QuitGame() {
        Application.Quit();
    }
}
