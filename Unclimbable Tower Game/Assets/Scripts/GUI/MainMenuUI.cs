using UnityEngine;
using UnityEngine.UI;
using System;

[SelectionBase]
public class MainMenuUI : MonoBehaviour {
    [Header("Main UI Screen")]
    [SerializeField] GameObject mainUIObj;
    [SerializeField] Button continueButton;
    [SerializeField] Button newGameButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button optionButton;
    [SerializeField] GameObject levelButtonObj;
    [SerializeField] GameObject secondMenuCanvas;
    [SerializeField] GameObject settingPanel;
    LevelButton[] _buttons;
    SaveSystem _saveSystem;
    const string _saveSystemName = "SaveSystem";
    void Start() {
        _saveSystem = GameObject.Find(_saveSystemName).GetComponent<SaveSystem>();
        _saveSystem.GameLoaded += OnGameLoaded;

        // Main UI
        exitButton.onClick.AddListener(GameManager.QuitGame);
    }

    public void SetSettingPanel(bool active) {
        secondMenuCanvas.SetActive(!active);
        levelButtonObj.SetActive(!active);
        settingPanel.SetActive(active);
    }

    void OnGameLoaded(object sender, EventArgs e) {
        
        // Level Buttons
        _buttons = GetComponentsInChildren<LevelButton>();
        for (int i = 0; i < _buttons.Length; i++) {
            if (_buttons[i].LevelNum <= _saveSystem.Data.Level) 
                _buttons[i].GetComponent<Button>().interactable = true;
            else
                _buttons[i].GetComponent<Button>().interactable = false;
        }
        
        // Continue Button
        if (_saveSystem.Data.Level < 2 || _saveSystem.Data.Level >= 8) {
            continueButton.interactable = false;
        } else {
            continueButton.onClick.AddListener(delegate{ GameManager.LoadLevel(_saveSystem.Data.Level ); });
        }
        
        // Exit Button
        exitButton.onClick.AddListener(delegate{ GameManager.QuitGame(); });
        newGameButton.onClick.AddListener(delegate{ _saveSystem.NewGame(); });
    }
}
