using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class ReserializeAllAssetsWindow : EditorWindow
{
    private string[] assetPaths = Array.Empty<string>();
    private Vector2 scrollPosition;
    private bool hasScanned;

    [MenuItem("Tools/Serialization/Reserialize All Assets")]
    private static void OpenWindow()
    {
        GetWindow<ReserializeAllAssetsWindow>("Reserialize Assets");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Scans registered assets under Assets/ and force-reserializes them using the current Unity version. This can rewrite many files. Save or commit your work first, then review the complete diff before removing FormerlySerializedAs attributes. ProjectSettings, Packages, and .meta files are not included.",
            MessageType.Warning);

        if (GUILayout.Button("Scan Assets"))
            ScanAssets();

        if (hasScanned)
        {
            EditorGUILayout.LabelField($"Assets found: {assetPaths.Length}", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(120));
            foreach (string assetPath in assetPaths)
                EditorGUILayout.LabelField(assetPath);
            EditorGUILayout.EndScrollView();
        }

        EditorGUI.BeginDisabledGroup(!hasScanned || assetPaths.Length == 0 || EditorApplication.isPlayingOrWillChangePlaymode);
        if (GUILayout.Button("Force Reserialize Scanned Assets"))
            ReserializeAssets();
        EditorGUI.EndDisabledGroup();

        if (EditorApplication.isPlayingOrWillChangePlaymode)
            EditorGUILayout.HelpBox("Stop Play Mode before reserializing assets.", MessageType.Info);
    }

    private void ScanAssets()
    {
        assetPaths = AssetDatabase.FindAssets(string.Empty, new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal) && !AssetDatabase.IsValidFolder(path))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        hasScanned = true;
    }

    private void ReserializeAssets()
    {
        if (!EditorUtility.DisplayDialog(
                "Confirm Asset Reserialization",
                $"Force Unity to rewrite {assetPaths.Length} assets under Assets/? This may create a large diff. Review it before removing any FormerlySerializedAs attributes.",
                "Reserialize",
                "Cancel"))
            return;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        AssetDatabase.SaveAssets();

        try
        {
            AssetDatabase.ForceReserializeAssets(assetPaths, ForceReserializeAssetsOptions.ReserializeAssets);
            Debug.Log($"Force-reserialized {assetPaths.Length} assets under Assets/. Review the source-control diff before removing FormerlySerializedAs attributes.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Asset Reserialization Failed", exception.Message, "OK");
        }
    }
}