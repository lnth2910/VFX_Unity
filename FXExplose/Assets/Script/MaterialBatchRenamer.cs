using UnityEngine;
using UnityEditor;

public class MaterialRenamer : EditorWindow
{
    private string baseName = "Material_";
    private Object[] selectedMaterials;

    [MenuItem("Tools/Material Renamer")]
    public static void ShowWindow()
    {
        GetWindow<MaterialRenamer>("Material Renamer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Chọn các Material trong Project rồi nhập tên mới:", EditorStyles.boldLabel);

        baseName = EditorGUILayout.TextField("Tên gốc:", baseName);

        if (GUILayout.Button("Lấy Material đang chọn"))
        {
            selectedMaterials = Selection.objects;
        }

        if (selectedMaterials != null && selectedMaterials.Length > 0)
        {
            EditorGUILayout.LabelField("Sẽ đổi tên cho " + selectedMaterials.Length + " material.");
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Đổi Tên và Đánh Số"))
        {
            RenameMaterials();
        }
    }

    private void RenameMaterials()
    {
        if (selectedMaterials == null || selectedMaterials.Length == 0)
        {
            EditorUtility.DisplayDialog("Lỗi", "Chưa chọn Material nào!", "OK");
            return;
        }

        for (int i = 0; i < selectedMaterials.Length; i++)
        {
            string newName = baseName + (i + 1);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedMaterials[i]), newName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}