using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityInspector : Editor 
{
    private Entity entity;
    private int selectedIndex = -1;

    void OnEnable()
    {
        entity = target as Entity;
    }

    public override void OnInspectorGUI()
    {
        entity = target as Entity;

        DrawDefaultInspector();

        DrawWaypointInspector();
    }

    private void OnSceneGUI()
    {
        entity = target as Entity;

        float size = 0.2f;
        for (int i = 0; i < entity.Path.Count; ++i)
        {
            if (Handles.Button(entity.Path[i], Quaternion.identity, size, size, Handles.DotCap))
            {
                selectedIndex = i;
                Repaint();
            }

            if (i < entity.Path.Count - 1)
                Handles.DrawLine(entity.Path[i], entity.Path[i + 1]);
            else if (entity.Loop)
                Handles.DrawLine(entity.Path[i], entity.Path[0]);
        }

        //Draw position handle
        if (selectedIndex >= 0)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 point = entity.Path[selectedIndex];
            point = Handles.DoPositionHandle(point, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(entity, "Move Point");
                EditorUtility.SetDirty(entity);
                entity.Path[selectedIndex] = point;
            }
        }
    }

    private void DrawWaypointInspector()
    {
        EditorGUILayout.LabelField("Waypoint options");

        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", entity.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(entity, "Toggle Loop");
            EditorUtility.SetDirty(entity);
            entity.Loop = loop;
        }

        if (GUILayout.Button("Add Point"))
        {
            Undo.RecordObject(entity, "Add Point");
            entity.Path.Add(Vector3.zero);
            EditorUtility.SetDirty(entity);
        }

        if (selectedIndex >= 0)
        {
            if (GUILayout.Button("Remove Selected Point"))
            {
                Undo.RecordObject(entity, "Remove Point");
                entity.Path.RemoveAt(selectedIndex);
                EditorUtility.SetDirty(entity);
                selectedIndex = -1;
            }
            else
            {
                GUILayout.Label("Selected Point");
                EditorGUI.BeginChangeCheck();
                Vector3 point = EditorGUILayout.Vector3Field("Position", entity.Path[selectedIndex]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(entity, "Move Point");
                    EditorUtility.SetDirty(entity);
                    entity.Path[selectedIndex] = point;
                }
            }
        }
    }
}
