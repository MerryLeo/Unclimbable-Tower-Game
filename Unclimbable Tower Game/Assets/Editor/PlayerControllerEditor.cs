using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    [MenuItem("GameObject/Create Other/Player")]
    public static void OnCreate()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.AddComponent<CapsuleCollider>();
        playerObj.AddComponent<PlayerController>();
        playerObj.tag = "Player";

        GameObject cameraObj = new GameObject("Player Camera");
        cameraObj.transform.SetParent(playerObj.transform);
        cameraObj.AddComponent<CameraController>();


        Selection.activeGameObject = playerObj;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
    static void DrawGizmoPlayerController(PlayerController controller, GizmoType gizmoType)
    {
        Gizmos.color = Color.blue;
        Vector3 position = controller.transform.position;
        Gizmos.DrawSphere(position, 2f);
    }
}