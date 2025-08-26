using UnityEngine;
using UnityEditor;

public class BatchRenameTool : EditorWindow
{
    string baseName = "Object";
    int startIndex = 1;

    [MenuItem("Tools/Batch Rename")]
    static void ShowWindow()
    {
        GetWindow<BatchRenameTool>("Batch Rename");
    }

    void OnGUI()
    {
        baseName = EditorGUILayout.TextField("Base Name", baseName);
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);

        if (GUILayout.Button("Rename Selected"))
        {
            RenameSelected();
        }
    }

    void RenameSelected()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        int index = startIndex;

        foreach (GameObject obj in selectedObjects)
        {
            Undo.RecordObject(obj, "Rename Object");
            obj.name = baseName + "_" + index.ToString("D2");
            index++;
        }
    }
}
