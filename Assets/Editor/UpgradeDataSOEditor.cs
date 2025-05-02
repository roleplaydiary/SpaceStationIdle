#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(UpgradeDataSO))]
public class UpgradeDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpgradeDataSO script = (UpgradeDataSO)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Export to JSON"))
        {
            ExportToJson(script);
        }

        if (GUILayout.Button("Import from JSON"))
        {
            ImportFromJson(script);
        }
    }

    private void ExportToJson(UpgradeDataSO data)
    {
        string filePath = EditorUtility.SaveFilePanel("Export Upgrade Data to JSON", "", "upgrade_data.json", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string json = JsonUtility.ToJson(new Wrapper<UpgradeDataSO.UpgradeEntry> { items = data.upgrades.ToList() }, true);

            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"Upgrade data exported to: {filePath}");
                EditorUtility.RevealInFinder(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error exporting to JSON: {e.Message}");
            }
        }
    }

    private void ImportFromJson(UpgradeDataSO data)
    {
        string filePath = EditorUtility.OpenFilePanel("Import Upgrade Data from JSON", "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                Wrapper<UpgradeDataSO.UpgradeEntry> wrapper = JsonUtility.FromJson<Wrapper<UpgradeDataSO.UpgradeEntry>>(json);
                if (wrapper != null && wrapper.items != null)
                {
                    data.upgrades = wrapper.items.ToArray();
                    EditorUtility.SetDirty(data); // Mark the SO as dirty for saving
                    AssetDatabase.SaveAssets(); // Save the changes to the asset file
                    Debug.Log($"Upgrade data imported from: {filePath}");
                }
                else
                {
                    Debug.LogError("JSON file is invalid or does not contain upgrade data.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error importing from JSON: {e.Message}");
            }
        }
    }

    // Helper class to wrap the array for JsonUtility
    [System.Serializable]
    private class Wrapper<T>
    {
        public System.Collections.Generic.List<T> items;
    }
}
#endif