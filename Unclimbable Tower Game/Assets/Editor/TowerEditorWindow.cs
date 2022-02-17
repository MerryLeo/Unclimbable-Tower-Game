// Simple Script to Select GameObjects and modify components

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TowerEditorWindow : EditorWindow {

    [MenuItem("Window/Tower Editor")]
    static void Init() {
        TowerEditorWindow window = (TowerEditorWindow)EditorWindow.GetWindow(typeof(TowerEditorWindow));
        window.Show();
    }

    void OnGUI() {
        
        // Selection Menu
        GUILayout.Label("Selection Options");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All")) {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null) {
                Debug.Log("No object selected");
            } else {
                Transform[] allTransforms = FetchAllChildrenAndParent(selectedObject.transform);
                GameObject[] gameObjects = FetchGameObjectsFromTransforms(allTransforms);
                Selection.objects = gameObjects;
            }
        }
        if (GUILayout.Button("Select Children")) {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null) {
                Debug.Log("No object selected");
            } else {
                Transform[] children = FetchAllChildren(selectedObject.transform);
                Selection.objects = FetchGameObjectsFromTransforms(children);
            }
        }
        GUILayout.EndHorizontal();

        // Component Modification Menu
        GUILayout.Label("Component Modification Options");
        

        if (GUILayout.Button("Select Mesh Collider from Selection")) {
            if (Selection.activeObject == null)
                Debug.Log("No object selected");

            else
            {
                List<MeshCollider> colliders = new List<MeshCollider>();
                GameObject[] gameObjects = Selection.gameObjects;
                for (int i = 0; i < gameObjects.Length; i++) {
                    MeshCollider objectCol = gameObjects[i].GetComponent<MeshCollider>();
                    if (objectCol != null)
                        colliders.Add(objectCol);
                }

                if (colliders.Count > 0)
                    Selection.objects = colliders.ToArray();
                
                else
                    Debug.Log("No colliders were found");
            }
        }
        

        /*
        if (GUILayout.Button("Add Meshcollider to Selection")) {
            
        }
        if (GUILayout.Button("Remove All Mesh Colliders from Selection")) {
            
        }
        */
        
    }

    // Get GameObjects array from transform array
    GameObject[] FetchGameObjectsFromTransforms(Transform[] transforms) {
        GameObject[] gameObjects = new GameObject[transforms.Length];
        for (int i = 0; i < transforms.Length; i++) {
            gameObjects[i] = transforms[i].gameObject;
        }
        return gameObjects;
    }

    // Select all children and the objectTrans itself
    Transform[] FetchAllChildrenAndParent(Transform objectTrans) {
        List<Transform> allTrans = new List<Transform>();
        allTrans.Add(objectTrans);
        allTrans.AddRange(FetchAllChildren(objectTrans));
        return allTrans.ToArray();
    }

    // Select all children of transform using recurssive
    Transform[] FetchAllChildren(Transform objectTrans) {

        // Fetch children
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < objectTrans.childCount; i++) {
            Transform childTransform = objectTrans.GetChild(i);
            children.Add(childTransform);

            // Get children of each children transform
            if (childTransform.childCount > 0) {
                Transform[] childs = FetchAllChildren(childTransform); 
                for (int j = 0; j < childs.Length; j++) {
                    children.Add(childs[j]);
                }
            }
        }
        return children.ToArray();
    }
}
