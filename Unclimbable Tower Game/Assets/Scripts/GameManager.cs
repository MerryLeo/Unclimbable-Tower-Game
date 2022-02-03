using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField] float levelTime = 2f;
    [SerializeField] int levelNum;
    public event EventHandler GameOverEvent;
    public event EventHandler PauseEvent;
    public event EventHandler ResumeEvent;
    public event EventHandler WonEvent;

    public float LevelTime => levelTime;
    public int CurrentLevel { get; private set; } = 0;
    GameObject _playerObj;
    const float _lowerBound = -10f;
    bool _gameOver, _playerWon;
    const string _mainMenuSceneName = "Mainmenu";

    // Start is called before the first frame update
    void Start() {
        _gameOver = _playerWon = false;
        _playerObj = GameObject.Find("Player");
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update() {
        if (!_playerWon && !_gameOver && _playerObj.transform.position.y < _lowerBound) // Game Over
            GameOver();

        else if (Input.GetButtonDown("Pause") && !_gameOver && !_playerWon) // Pause
            Pause();
    }


    public void Win() {
        EventHandler handler = WonEvent;
        handler?.Invoke(this, EventArgs.Empty);
        _playerWon = true;
        Time.timeScale = 0;

        // Save Game Data
        if (CurrentLevel < levelNum) {
            CurrentLevel++;
            SaveSystem.SaveGame(new PlayerData(levelNum));
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
        SaveSystem.SaveGame(new PlayerData(levelNum));
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
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
