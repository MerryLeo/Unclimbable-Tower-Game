using System;
using UnityEngine;

public class PlayerUIScreen : MonoBehaviour
{
    // UI Elements
    [Header("UI Game Objects")]
    [SerializeField]
    GameObject crosshairObj;
    [SerializeField]
    GameObject timerObj;
    [SerializeField]
    GameObject pauseScreenObj;
    [SerializeField]
    GameObject optionScreenObj;
    [SerializeField]
    GameObject winScreenObj;
    [SerializeField]
    GameObject gameOverScreenObj;

    [SerializeField]
    Timer timer;

    GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        // Find Manager and Add Listeners
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        manager.PauseEvent += OnGamePaused;
        manager.ResumeEvent += OnGameResumed;
        manager.WonEvent += OnGameWon;
        manager.GameOverEvent += OnGameOver;

        // Initial UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(true);
        gameOverScreenObj.SetActive(false);
    }

    void OnGamePaused(object sender, EventArgs e)
    {
        // Timer
        timer.StopTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(true);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameResumed(object sender, EventArgs e)
    {
        // Timer
        timer.StartTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(true);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameWon(object sender, EventArgs e)
    {
        // Timer
        timer.StopTimer();

        // UI Configuration
        winScreenObj.SetActive(true);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(false);
    }

    void OnGameOver(object sender, EventArgs e)
    {
        // Timer
        timer.StopTimer();

        // UI Configuration
        winScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        crosshairObj.SetActive(false);
        gameOverScreenObj.SetActive(true);
    }
}
