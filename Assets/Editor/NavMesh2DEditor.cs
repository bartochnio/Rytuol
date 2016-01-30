using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMesh2D))]
public class NavMesh2DEditor : Editor 
{
    private NavMesh2D navMesh;

    void OnEnable()
    {
        navMesh = target as NavMesh2D;
    }

    public override void OnInspectorGUI()
    {
        navMesh = target as NavMesh2D;

        if (GUILayout.Button("Add obstacle"))
        {
            Undo.RecordObject(navMesh, "Add Obstacle");
            navMesh.AddObstacle();
            EditorUtility.SetDirty(navMesh);
        }

        if (GUILayout.Button("Generate"))
        {
            Undo.RecordObject(navMesh, "Generate");
            navMesh.Decompose();
            EditorUtility.SetDirty(navMesh);
        }

        DrawDefaultInspector();
    }
}
