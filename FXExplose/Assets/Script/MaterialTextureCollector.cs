using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MaterialTextureCollector : EditorWindow
{
    private List<Material> materials = new List<Material>();
    private string targetFolder = "Assets/CollectedTextures";
    private Vector2 scrollPos;

    [MenuItem("Tools/Material Texture Collector")]
    public static void ShowWindow()
    {
        GetWindow<MaterialTextureCollector>("Material Texture Collector");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Kéo thả Materials vào đây:", EditorStyles.boldLabel);

        // Khu vực Drag & Drop
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Thả Materials vào đây");

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object dragged in DragAndDrop.objectReferences)
                    {
                        if (dragged is Material mat && !materials.Contains(mat))
                        {
                            materials.Add(mat);
                        }
                    }
                }
                Event.current.Use();
            }
        }

        // Scroll view danh sách materials
        if (materials.Count > 0)
        {
            EditorGUILayout.LabelField("Danh sách Materials:");

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            foreach (var mat in materials)
            {
                EditorGUILayout.ObjectField(mat, typeof(Material), false);
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Clear All"))
            {
                materials.Clear();
            }
        }

        EditorGUILayout.Space();

        targetFolder = EditorGUILayout.TextField("Target Folder:", targetFolder);

        if (GUILayout.Button("Di Chuyển Texture"))
        {
            MoveTextures();
        }
    }

    private void MoveTextures()
    {
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            AssetDatabase.Refresh();
        }

        HashSet<string> moved = new HashSet<string>();

        foreach (var mat in materials)
        {
            Shader shader = mat.shader;
            int count = ShaderUtil.GetPropertyCount(shader);

            for (int i = 0; i < count; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propName = ShaderUtil.GetPropertyName(shader, i);
                    Texture tex = mat.GetTexture(propName);

                    if (tex != null)
                    {
                        string path = AssetDatabase.GetAssetPath(tex);
                        if (!moved.Contains(path))
                        {
                            string fileName = Path.GetFileName(path);
                            string newPath = Path.Combine(targetFolder, fileName).Replace("\\", "/");

                            string error = AssetDatabase.MoveAsset(path, newPath);
                            if (!string.IsNullOrEmpty(error))
                            {
                                Debug.LogError($"Lỗi khi move {fileName}: {error}");
                            }
                            else
                            {
                                moved.Add(path);
                            }
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Hoàn tất", "Đã di chuyển tất cả texture vào " + targetFolder, "OK");
    }
}
