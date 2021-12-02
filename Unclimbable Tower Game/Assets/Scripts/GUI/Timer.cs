// Countdown

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour {
    Text timerText;
    GameManager gameManager;
    float startTime, time;
    bool active, outOfTime;

    // Start is called before the first frame update
    void Start() {
        timerText = GetComponent<Text>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ResetTimer();
    }

    // Update is called once per frame
    void Update() {
        if (active) {
            int minute = (int)(time - (Time.time - startTime)/60f);
            int second = (int)(60f * (time - minute - (Time.time - startTime)/60f));
            if (minute > 0) {
                timerText.text = $"{minute}min{second}";
            } else if (second > 0) {
                timerText.text = $"{second}s";
            } else {
                timerText.text = "Time's up!";
                outOfTime = true;
            }
        }

        if (outOfTime)
            gameManager.GameOver();
    }

    public void StartTimer() {
        active = true;
    }

    public void StopTimer() {
        active = false;
    }

    public void ResetTimer() {
        outOfTime = false;
        startTime = Time.time;
        time = gameManager.LevelTime;
    }
}
