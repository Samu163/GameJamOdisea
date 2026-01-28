using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridObject))]
public class GridObjectEditor : Editor
{
    private GridObject gridObject;
    private bool wasSnapEnabled = false;

    private void OnEnable()
    {
        gridObject = (GridObject)target;
        wasSnapEnabled = gridObject.snapToGrid;
    }

    private void OnSceneGUI()
    {
        if (gridObject.snapToGrid && !wasSnapEnabled)
        {
            Undo.RecordObject(gridObject.transform, "Snap to Grid");
            gridObject.SnapToGridNow();
            SceneView.RepaintAll();
        }
        wasSnapEnabled = gridObject.snapToGrid;

        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 280, 200));
        GUILayout.BeginVertical("box");
        GUILayout.Label("Grid Object Editor", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        bool newSnap = GUILayout.Toggle(gridObject.snapToGrid, "Snap to Grid", "button", GUILayout.Height(30));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gridObject, "Toggle Snap");
            gridObject.snapToGrid = newSnap;
            if (newSnap)
            {
                Undo.RecordObject(gridObject.transform, "Snap to Grid");
                gridObject.SnapToGridNow();
            }
        }

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Recalcular", GUILayout.Height(25)))
        {
            Undo.RecordObject(gridObject, "Recalculate Size");
            gridObject.CalculateGridSizeFromColliders(); // CAMBIADO
            EditorUtility.SetDirty(gridObject);
        }
        if (GUILayout.Button("Snapear", GUILayout.Height(25)))
        {
            Undo.RecordObject(gridObject.transform, "Manual Snap");
            gridObject.snapToGrid = true;
            gridObject.SnapToGridNow();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}