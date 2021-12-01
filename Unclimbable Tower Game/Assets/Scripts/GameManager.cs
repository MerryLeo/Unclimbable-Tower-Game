using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField]
    float levelTime = 2f;
    [SerializeField]
    string nextSceneName = null;

    [Header("GUI")]
    [SerializeField] 
    GameObject pauseScreen = null;
    [SerializeField]
    GameObject winScreen = null;
    [SerializeField]
    GameObject gameOverScreen = null;
    [SerializeField]
    GameObject crosshair = null;

    public float LevelTime => levelTime;

    Timer timer;
    GameObject playerObj;
    CameraController cameraControllerScript;
    const float lowerBound = -10f;
    bool gameOver, playerWon;

    // Start is called before the first frame update
    void Start() {
        gameOver = playerWon = false;

        timer = GameObject.Find("Timer").GetComponentInChildren<Timer>();
        playerObj = GameObject.Find("Player");
        cameraControllerScript = playerObj.GetComponentInChildren<CameraController>();

        winScreen.SetActive(false);
        pauseScreen.SetActive(false);
        crosshair.SetActive(true);
        gameOverScreen.SetActive(false);

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update() {

        if (!playerWon && !gameOver && playerObj.transform.position.y < lowerBound) // Game Over
            GameOver();

        else if (Input.GetButtonDown("Pause") && !gameOver && !playerWon) // Pause
            Pause();
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Win() {

        // Trigger the game won condition
        playerWon = true;

        // Stop the time
        Time.timeScale = 0;

        // Turn off the camera
        cameraControllerScript.CameraEnabled = false;
        Cursor.lockState = CursorLockMode.None;

        // Configure the UI
        winScreen?.SetActive(true);
        crosshair?.SetActive(false);
        gameOverScreen?.SetActive(false);
        pauseScreen?.SetActive(false);
    }

    public void GameOver() {

        // Trigger the game over condition
        gameOver = true;

        // Stop the time
        Time.timeScale = 0;

        // Turn off the camera
        cameraControllerScript.CameraEnabled = false;
        Cursor.lockState = CursorLockMode.None;

        // Configure the UI
        gameOverScreen?.SetActive(true);
        winScreen?.SetActive(false);
        pauseScreen?.SetActive(false);
        crosshair?.SetActive(false);
    }

    public void Pause() {
        // Stop the time
        timer.StopTimer();
        Time.timeScale = 0;

        // Turn off the camera
        cameraControllerScript.CameraEnabled = false;
        Cursor.lockState = CursorLockMode.None;

        // Configure the UI
        pauseScreen.SetActive(true);
        crosshair.SetActive(false);
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    public void Resume() {

        // Resume the Time
        timer.StartTimer();
        Time.timeScale = 1;

        // Turn on the camera
        cameraControllerScript.CameraEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        // Configure the UI
        crosshair?.SetActive(true);
        pauseScreen?.SetActive(false);
        gameOverScreen?.SetActive(false);
        winScreen?.SetActive(false);
    }

    public void NextLevel() {
        if (nextSceneName != null)
            SceneManager.LoadScene(nextSceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
