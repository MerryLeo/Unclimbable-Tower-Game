// Custom Editor Window to create UI buttons over existing objects

using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class UICustomEditor : EditorWindow
{
    Transform parentTransform = null;
    Button button;
    GameObject buttonPrefab;
    string label = "Button";
    const float offset = 0.5f;

    [MenuItem("Window/WorldSpace UI Customization")]
    public static void ShowWindow() 
    {
        EditorWindow.GetWindow(typeof(UICustomEditor));
        
    }

    void OnGUI() 
    {
        GUILayout.Label ("Button Settings", EditorStyles.boldLabel);
        buttonPrefab = (GameObject)EditorGUILayout.ObjectField(buttonPrefab, typeof(GameObject), true);
        parentTransform = (Transform)EditorGUILayout.ObjectField(parentTransform, typeof(Transform), true);
        if (GUILayout.Button("Create Button"))
        {
            if (Selection.gameObjects.Length > 0 || parentTransform != null)
            {
                foreach(GameObject obj in Selection.gameObjects)
                {
                    CreateButton(obj);
                }
            }
        }
    }

    void CreateButton(GameObject obj)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, obj.transform.position + obj.transform.forward * offset, obj.transform.rotation);
        buttonObj.GetComponentInChildren<Text>().text = label;

        // Scale
        Vector3 boundScale = obj.GetComponent<MeshRenderer>().bounds.size;
        buttonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(boundScale.x, boundScale.y);

        buttonObj.transform.SetParent(parentTransform);

        /*
        // Set up Button Object
        GameObject buttonObj = new GameObject("Button");
        buttonObj.layer = LayerMask.NameToLayer("UI");
        RectTransform buttonRectTrans = (RectTransform)buttonObj.AddComponent(typeof(RectTransform));
        buttonObj.AddComponent(typeof(CanvasRenderer));
        buttonObj.AddComponent(typeof(Image));
        buttonObj.AddComponent(typeof(Button));

        // Set up Text Object
        GameObject textObj = new GameObject("Text");
        textObj.layer = LayerMask.NameToLayer("UI");
        textObj.AddComponent(typeof(RectTransform));
        textObj.AddComponent(typeof(CanvasRenderer));
        textObj.transform.localScale = Vector3.one * 0.01f;
        Text text = (Text)textObj.AddComponent(typeof(Text));
        text.color = Color.black;
        text.text = label;
        text.alignByGeometry = true;
        textObj.transform.SetParent(buttonObj.transform);

        // Set up Button Transform
        buttonObj.transform.position = obj.transform.position + obj.transform.forward * offset;
        buttonObj.transform.rotation = obj.transform.rotation;
        buttonRectTrans.sizeDelta = new Vector2(obj.transform.localScale.x, obj.transform.localScale.y);

        // Set Button as a child of parentTransform
        buttonObj.transform.SetParent(parentTransform);
        */
    }
}
