using UnityEngine;
using UnityEditor;

public class ParticleMatConverter : EditorWindow
{
    string folderPath = "Assets/"; // Thư mục cần convert

    [MenuItem("Tools/Convert Particle Materials")]
    static void ShowWindow()
    {
        GetWindow<ParticleMatConverter>("Material Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("Convert Legacy Particle Materials to URP", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Folder:", folderPath);

        if (GUILayout.Button("Convert"))
        {
            ConvertMaterials(folderPath);
        }
    }

    static void ConvertMaterials(string folder)
    {
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            if (mat.shader.name.Contains("Legacy Shaders/Particles"))
            {
                Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                Color tint = mat.HasProperty("_TintColor") ? mat.GetColor("_TintColor") : Color.white;

                mat.shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");

                if (mainTex != null) mat.SetTexture("_BaseMap", mainTex);
                mat.SetColor("_BaseColor", tint);
                mat.SetFloat("_Surface", 1); // Transparent

                Debug.Log($"Converted: {path}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Done converting materials!");
    }
}
