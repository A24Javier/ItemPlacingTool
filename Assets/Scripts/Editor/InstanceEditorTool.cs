using UnityEditor;
using UnityEngine;

public class InstanceEditorTool : EditorWindow
{
    GameObject[] prefabs;
    int selectedPrefab = 0;
    Transform parentObject;

    GameObject lastInstance;

    [MenuItem("Tools/Instance Obj Tool")]
    public static void OpenWindow()
    {
        GetWindow<InstanceEditorTool>("Instance Objects Tool");
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);

        int size = prefabs != null ? prefabs.Length : 0;
        int newSize = EditorGUILayout.IntField("Size", size);

        if (prefabs == null || newSize != prefabs.Length)
        {
            System.Array.Resize(ref prefabs, newSize);
        }

        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i}", prefabs[i], typeof(GameObject), false);
        }

        if (prefabs != null && prefabs.Length > 0)
        {
            selectedPrefab = EditorGUILayout.IntSlider("Active Prefab", selectedPrefab, 0, prefabs.Length - 1);
        }
        else
        {
            EditorGUILayout.HelpBox("Add at least one prefab", MessageType.Warning);
        }

        parentObject = (Transform)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(Transform), true);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Event ev = Event.current;

        if (prefabs == null || prefabs.Length == 0) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (ev.type == EventType.MouseDown && ev.button == 0)
            {
                CreateObject(hit.point);
                ev.Use();
            }

            if (ev.type == EventType.MouseDrag && lastInstance != null)
            {
                RotateObject(hit.point);
                ev.Use();
            }

            if (ev.type == EventType.MouseUp)
            {
                lastInstance = null;
            }
        }
    }

    void CreateObject(Vector3 position)
    {
        GameObject prefab = prefabs[selectedPrefab];

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        Undo.RegisterCreatedObjectUndo(instance, "Create Object");

        instance.transform.position = position;

        if (parentObject != null)
            instance.transform.SetParent(parentObject);

        lastInstance = instance;
    }

    void RotateObject(Vector3 mousePosition)
    {
        Vector3 direction = mousePosition - lastInstance.transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            lastInstance.transform.rotation = rot;
        }
    }
}
