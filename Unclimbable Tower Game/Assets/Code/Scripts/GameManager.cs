using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Compilation;

public class GameManager : MonoBehaviour 
{
    const float LOWER_BOUND = -10f;
    const string MAINMENU_SCENE_NAME = "Mainmenu", PLAYER_NAME = "Player", SAVE_SYSTEM_NAME = "SaveSystem";
    const string PAUSE_INPUT = "Pause";

    [Header("Settings")] 
    [SerializeField] float levelTime = 2f;
    [SerializeField] CameraController[] playerCams;
    public event EventHandler GameOverEvent, PauseEvent, ResumeEvent, WonEvent;
    public float LevelTime => levelTime;
    GameObject _playerObj;
    int? _levelNum; // Current Level Number
    bool _gameOver, _playerWon;
    SaveSystem _saveSystem;

    void BeforeBuild(string input)
    {
        Debug.Log(input);
        playerCams = GameObject.FindObjectsOfType<CameraController>();
    }


    void Reset() 
    {
        Debug.Log("Hooked!");
        CompilationPipeline.assemblyCompilationStarted += BeforeBuild;
    }

    

    void Start() 
    {
        // Level Number is found in the name of the current Scene
        // Ex: if the scene is called Level 5 then level number is 5
        _levelNum = SceneManager.GetActiveScene().name.FetchNumber();
        if (_levelNum == null)
            _levelNum = 1;

        _saveSystem = GameObject.Find(SAVE_SYSTEM_NAME).GetComponent<SaveSystem>();
        _gameOver = _playerWon = false;
        _playerObj = GameObject.Find(PLAYER_NAME);
        Time.timeScale = 1;
    }

    void Update() 
    {
        // Game Over
        if (!_playerWon && !_gameOver && _playerObj.transform.position.y < LOWER_BOUND)
            GameOver();

        // Pause
        else if (Input.GetButtonDown(PAUSE_INPUT) && !_gameOver && !_playerWon)
            Pause();
    }


    public void Win() 
    {
        EventHandler handler = WonEvent;
        handler?.Invoke(this, EventArgs.Empty);
        _playerWon = true;
        Time.timeScale = 0;

        // Save Game Data
        if (_saveSystem.Data.Level <= _levelNum) 
        {
            _saveSystem.Data.Level++;
            SaveSystem.SaveGame(_saveSystem.Data);
        }
    }

    public void GameOver() 
    {
        EventHandler handler = GameOverEvent;
        handler?.Invoke(this, EventArgs.Empty);
        _gameOver = true;
        Time.timeScale = 0;
    }

    public void Pause() 
    {
        EventHandler handler = PauseEvent;
        handler?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 0;
    }

    public void Resume() 
    {
        EventHandler handler = ResumeEvent;
        handler?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu() 
        => SceneManager.LoadScene(MAINMENU_SCENE_NAME, LoadSceneMode.Single);
    

    // Restart Current Scene
    public static void RestartLevel() 
        => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    // Load Level with a number
    public static void LoadLevel(int levelNumber) 
        => SceneManager.LoadScene($"Level {levelNumber}", LoadSceneMode.Single);
    

    // Load Next Level with current level number
    public static void NextLevel(int currentLevel) 
        => SceneManager.LoadScene($"Level {currentLevel + 1}", LoadSceneMode.Single);
    
    // Quit the application
    public static void QuitGame() => Application.Quit();
}
