using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelPainter : EditorWindow
{
    [SerializeField] private List<GameObject> tilePrefabs = new List<GameObject>();
    private Vector2 scrollPosition;
    private int selectedPrefabIndex = -1;
    private bool isPainting = false;
    private bool snapToSurface = true;
    private GameObject previewObject;

    private GameObject lastPreviewedPrefab;

    private Vector3 lastValidPosition;
    private bool hasLastValidPosition = false;

    [MenuItem("Window/Level Painter")]
    public static void ShowWindow()
    {
        LevelPainter window = GetWindow<LevelPainter>("Level Painter");
        window.minSize = new Vector2(300, 400);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        DestroyPreview();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Painter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Configuración:", EditorStyles.miniBoldLabel);

        isPainting = EditorGUILayout.Toggle("Paint", isPainting);
        snapToSurface = EditorGUILayout.Toggle("Snap To Surface", snapToSurface);

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label("Prefabs:", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(300));

        if (GUILayout.Button("+ Add Prefab", GUILayout.Height(30)))
        {
            tilePrefabs.Add(null);
        }

        GUILayout.Space(5);

        for (int i = 0; i < tilePrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            if (selectedPrefabIndex == i)
            {
                GUI.backgroundColor = Color.green;
            }

            string buttonText = i < 9 ? $"[{i + 1}]" : $"#{i + 1}";
            if (GUILayout.Button(buttonText, GUILayout.Width(45)))
            {
                selectedPrefabIndex = i;
                DestroyPreview();
                hasLastValidPosition = false;
                SceneView.RepaintAll();
            }

            GUI.backgroundColor = Color.white;

            EditorGUI.BeginChangeCheck();
            tilePrefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                tilePrefabs[i], typeof(GameObject), false);

            if (EditorGUI.EndChangeCheck() && i == selectedPrefabIndex)
            {
                DestroyPreview();
                hasLastValidPosition = false;
            }

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tilePrefabs.RemoveAt(i);
                if (selectedPrefabIndex == i) selectedPrefabIndex = -1;
                if (selectedPrefabIndex > i) selectedPrefabIndex--;
                DestroyPreview();
                hasLastValidPosition = false;
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // Instrucciones
        EditorGUILayout.HelpBox(
            "CONTROLES:\n" +
            " Click Izquierdo: Colocar prefab\n" +
            " Shift + Click: Eliminar objeto\n" +
            " Teclas 1-9: Cambio rápido de prefab"+
            " Nota: A veces va un poco raro el tema de pintar y cambiar objeto, sino funciona, clica en el prefab 1 y pinta 1 vez, a partir de ahi podras cambiar todo lo que quieras",
            MessageType.Info);

        // Info del prefab seleccionado
        if (selectedPrefabIndex >= 0 && selectedPrefabIndex < tilePrefabs.Count &&
            tilePrefabs[selectedPrefabIndex] != null)
        {
            GameObject prefab = tilePrefabs[selectedPrefabIndex];
            GridObject gridObj = prefab.GetComponent<GridObject>();

            if (gridObj != null)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label($"Prefab: {prefab.name}", EditorStyles.boldLabel);
                GUILayout.Label($"Dimensiones: {gridObj.gridSizeX}x{gridObj.gridSizeY}x{gridObj.gridSizeZ}");
                EditorGUILayout.EndVertical();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPainting)
        {
            DestroyPreview();
            hasLastValidPosition = false;
            return;
        }

        Event e = Event.current;

        // Atajos de teclado - cambio instantáneo
        if (e.type == EventType.KeyDown && !e.alt && !e.control)
        {
            if (e.keyCode >= KeyCode.Alpha1 && e.keyCode <= KeyCode.Alpha9)
            {
                int index = e.keyCode - KeyCode.Alpha1;
                if (index < tilePrefabs.Count)
                {
                    selectedPrefabIndex = index;
                    DestroyPreview();
                    hasLastValidPosition = false;
                    e.Use();
                    Repaint();
                    SceneView.RepaintAll();
                }
            }
        }

        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= tilePrefabs.Count)
        {
            DestroyPreview();
            hasLastValidPosition = false;
            return;
        }

        GameObject selectedPrefab = tilePrefabs[selectedPrefabIndex];
        if (selectedPrefab == null)
        {
            DestroyPreview();
            hasLastValidPosition = false;
            return;
        }

        GridObject gridObj = selectedPrefab.GetComponent<GridObject>();
        if (gridObj == null)
        {
            DestroyPreview();
            hasLastValidPosition = false;
            return;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;

        if (snapToSurface)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                spawnPosition = CalculateSnapPosition(hit.point, gridObj);
                validPosition = true;
            }
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                spawnPosition = SnapToGrid(hitPoint, gridObj);
                validPosition = true;
            }
        }

        if (validPosition)
        {
            lastValidPosition = spawnPosition;
            hasLastValidPosition = true;

            UpdatePreview(selectedPrefab, spawnPosition);
            DrawPlacementGuides(spawnPosition, gridObj);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                if (e.shift)
                {
                    DeleteObjectAtPosition(spawnPosition);
                }
                else
                {
                    PlaceObject(selectedPrefab, spawnPosition);
                }
                e.Use();
            }
        }
        else if (hasLastValidPosition)
        {
            UpdatePreview(selectedPrefab, lastValidPosition);
            DrawPlacementGuides(lastValidPosition, gridObj);
        }
        else
        {
            DestroyPreview();
        }

        SceneView.RepaintAll();
    }

    private Vector3 CalculateSnapPosition(Vector3 hitPoint, GridObject gridObj)
    {
        Vector3 snappedHit = new Vector3(
            Mathf.Round(hitPoint.x),
            Mathf.Round(hitPoint.y),
            Mathf.Round(hitPoint.z)
        );

        Vector3 pivotOffset = new Vector3(
            gridObj.gridSizeX * 0.5f,
            gridObj.gridSizeY * 0.5f,
            gridObj.gridSizeZ * 0.5f
        );
        return snappedHit + pivotOffset;
    }

    private Vector3 SnapToGrid(Vector3 worldPos, GridObject gridObj)
    {
        Vector3 snapped = new Vector3(
            Mathf.Round(worldPos.x),
            Mathf.Round(worldPos.y),
            Mathf.Round(worldPos.z)
        );

        Vector3 pivotOffset = new Vector3(
            gridObj.gridSizeX * 0.5f,
            gridObj.gridSizeY * 0.5f,
            gridObj.gridSizeZ * 0.5f
        );

        return snapped + pivotOffset;
    }


    private void DrawPlacementGuides(Vector3 position, GridObject gridObj)
    {
        Vector3 size = new Vector3(gridObj.gridSizeX, gridObj.gridSizeY, gridObj.gridSizeZ);
        Handles.color = new Color(0, 1, 1, 0.8f);
        Handles.DrawWireCube(position, size);
        Handles.color = Color.yellow;
        Handles.SphereHandleCap(0, position, Quaternion.identity, 0.15f, EventType.Repaint);
    }

    private void UpdatePreview(GameObject prefab, Vector3 position)
    {
        if (previewObject != null && lastPreviewedPrefab != prefab)
        {
            DestroyPreview();
        }

        if (previewObject == null)
        {
            previewObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            previewObject.name = "[PREVIEW]";
            previewObject.hideFlags = HideFlags.HideAndDontSave;
            lastPreviewedPrefab = prefab;

            foreach (Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] != null)
                    {
                        Material tempMat = new Material(mats[i]);

                        if (tempMat.HasProperty("_Color"))
                        {
                            Color col = tempMat.color;
                            col.a = 0.5f;
                            tempMat.color = col;
                        }

                        tempMat.SetFloat("_Mode", 3);
                        tempMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        tempMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        tempMat.SetInt("_ZWrite", 0);
                        tempMat.DisableKeyword("_ALPHATEST_ON");
                        tempMat.EnableKeyword("_ALPHABLEND_ON");
                        tempMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        tempMat.renderQueue = 3000;

                        mats[i] = tempMat;
                    }
                }
                renderer.sharedMaterials = mats;
            }

            foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        previewObject.transform.position = position;
    }

    private void DestroyPreview()
    {
        if (previewObject != null)
        {
            DestroyImmediate(previewObject);
            previewObject = null;
            lastPreviewedPrefab = null;
        }
    }

    private void PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = position;

        GridObject gridObj = instance.GetComponent<GridObject>();
        if (gridObj != null && gridObj.snapToGrid)
        {
            gridObj.SnapToGridNow();
        }

        Undo.RegisterCreatedObjectUndo(instance, "Paint Object");
        Selection.activeGameObject = instance;
    }

    private void DeleteObjectAtPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);

        foreach (Collider col in colliders)
        {
            GridObject gridObj = col.GetComponent<GridObject>();
            if (gridObj != null)
            {
                Undo.DestroyObjectImmediate(gridObj.gameObject);
                break;
            }
        }
    }
}