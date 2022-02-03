using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class MainMenuUI : MonoBehaviour {
    [Header("Main UI Screen")]
    [SerializeField] GameObject mainUIObj;
    [SerializeField] Button continueButton;
    [SerializeField] Button newGameButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button optionButton;
    [SerializeField] GameObject levelButtonObj;
    LevelButton[] buttons;
    PlayerData data;
    void Start() {

        // Load Save
        data = SaveSystem.LoadGame();
        int currentLevel;
        if (data == null) {
            SaveSystem.SaveGame(new PlayerData(1));
            currentLevel = 1;
        } else {
            currentLevel = data.Level;
        }

        // Level Buttons
        buttons = GetComponentsInChildren<LevelButton>();
        for (int i = 0; i < buttons.Length; i++) {
            if (buttons[i].LevelNum > currentLevel) 
                buttons[i].GetComponent<Button>().interactable = false;
        }

        // Main UI
        exitButton.onClick.AddListener(GameManager.QuitGame);

    }
}
