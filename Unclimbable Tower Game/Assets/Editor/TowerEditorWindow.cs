// Simple Script to add mesh colliders to all GameObjects selected

using UnityEngine;
using UnityEditor;

public class TowerEditorWindow : EditorWindow {
    [MenuItem("Window/Tower Editor")]
    static void Init() {
        TowerEditorWindow window = (TowerEditorWindow)EditorWindow.GetWindow(typeof(TowerEditorWindow));
        window.Show();
    }

    void OnGUI() {
        if (GUILayout.Button("Add Meshcollider to Selection")) {
            GameObject[] selectedObjects = Selection.gameObjects;
            for (int i = 0; i < selectedObjects.Length; i++) {
                if (selectedObjects[i].GetComponent<MeshCollider>() != null) { // Already has Collider
                    Debug.Log($"{selectedObjects[i].name} already has a Mesh Collider Component.");
                } else if (selectedObjects[i].GetComponent<MeshRenderer>() == null) { // No Mesh Rendererer
                    Debug.Log($"{selectedObjects[i].name} does not have a Mesh Rendererer.");
                } else { // Add Mesh Collider
                    Debug.Log($"Adding Mesh Collider to {selectedObjects[i].name}.");
                    selectedObjects[i].AddComponent<MeshCollider>();
                }
            }
        }

        if (GUILayout.Button("Remove All Mesh Colliders from Selection")) {
            GameObject[] selectedObjects = Selection.gameObjects;
            for (int i = 0; i < selectedObjects.Length; i++) {
                MeshCollider[] colliders = selectedObjects[i].GetComponents<MeshCollider>();
                if (colliders != null) {
                    foreach (Collider collider in colliders) {
                        Debug.Log("Collider Destroyed");
                        DestroyImmediate(collider);
                    }
                }
            }
        }
    }

    void AddMeshCollider() {
        
    }
}
