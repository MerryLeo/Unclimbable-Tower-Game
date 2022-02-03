using UnityEngine;

public static class GizmosExtension {
    public static void DrawLineWithSpheres(this Gizmos gizmos, Vector3 start, Vector3 end, Color lineCol, Color sphereCol, float spheresRadius) {
        // Draw Line
        Gizmos.color = lineCol;
        Gizmos.DrawLine(start, end);

        // Draw Spheres
        Gizmos.color = sphereCol;
        Gizmos.DrawWireSphere(start, spheresRadius);
        Gizmos.DrawWireSphere(end, spheresRadius);
    }
}
