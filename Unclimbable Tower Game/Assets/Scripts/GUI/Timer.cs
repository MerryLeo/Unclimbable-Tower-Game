// Count down script that modifies a text component

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour {
    public bool Initialised { get; private set; }
    Text _timerText;
    GameManager _manager;
    float _startTime, _time;
    bool _active, _outOfTime;

    // Start is called before the first frame update
    void Start() {
        _timerText = GetComponent<Text>();
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _active = false;
        Initialised = false;
    }

    // Update is called once per frame
    void Update() {
        if (_active) {
            int minute = (int)(_time - (Time.time - _startTime)/60f);
            int second = (int)(60f * (_time - minute - (Time.time - _startTime)/60f));
            if (minute > 0) 
                _timerText.text = $"{minute}min{second}";
            
            else if (second > 0) 
                _timerText.text = $"{second}s";
            
            else {
                _timerText.text = "Time's up!";
                _outOfTime = true;
            }
        }

        if (_outOfTime)
            _manager?.GameOver();
    }

    public void StartTimer() {
        _active = true;
    }

    public void StopTimer() {
        _active = false;
    }

    public void InitializeTimer() {
        Initialised = true;
        _outOfTime = false;
        _startTime = Time.time;
        _time = _manager.LevelTime;
    }
}
