using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour 
{
    [SerializeField] int levelNum = 1;
    public int LevelNum => levelNum;
    Button _button;

    void Start() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate{OnClickLoadLevel(LevelNum);});

    }

    void OnClickLoadLevel(int levelToLoad) {
        GameManager.LoadLevel(levelToLoad);
    }
}
