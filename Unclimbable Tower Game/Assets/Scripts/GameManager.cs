using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    [Header("Settings")] 
    [SerializeField]
    float levelTime = 2f;
    [SerializeField]
    string nextSceneName = null;

    public event EventHandler GameOverEvent;
    public event EventHandler PauseEvent;
    public event EventHandler ResumeEvent;
    public event EventHandler WonEvent;

    public float LevelTime => levelTime;

    GameObject playerObj;
    // CameraController cameraControllerScript;
    const float lowerBound = -10f;
    bool gameOver, playerWon;

    // Start is called before the first frame update
    void Start() 
    {
        gameOver = playerWon = false;
        playerObj = GameObject.Find("Player");
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update() 
    {
        if (!playerWon && !gameOver && playerObj.transform.position.y < lowerBound) // Game Over
            GameOver();

        else if (Input.GetButtonDown("Pause") && !gameOver && !playerWon) // Pause
            Pause();
    }

    public void RestartLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Win() 
    {
        // Raise Won Event
        EventHandler handler = WonEvent;
        handler?.Invoke(this, EventArgs.Empty);

        playerWon = true;
        Time.timeScale = 0;
    }

    public void GameOver() 
    {
        // Raise GameOver Event
        EventHandler handler = GameOverEvent;
        handler?.Invoke(this, EventArgs.Empty);

        gameOver = true;
        Time.timeScale = 0;
    }

    public void Pause() 
    {
        // Raise Pause Event
        EventHandler handler = PauseEvent;
        handler?.Invoke(this, EventArgs.Empty);

        Time.timeScale = 0;
    }

    public void Resume() 
    {
        // Raise Resume Event
        EventHandler handler = ResumeEvent;
        handler?.Invoke(this, EventArgs.Empty);

        Time.timeScale = 1;
    }

    public void NextLevel() 
    {
        if (nextSceneName != null)
            SceneManager.LoadScene(nextSceneName);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
