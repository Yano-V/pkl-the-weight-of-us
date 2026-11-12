using UnityEngine;

[CreateAssetMenu(fileName = "Limbo UI Page", menuName = "Limbo/UI/UI Page")]
public class UIPageSO : ScriptableObject
{
    [Header("Page ID Settings")]
    [SerializeField] string pageIDPrefix;
    [SerializeField] string pageID;
    [SerializeField] string pageIDSuffix;
    [SerializeField] string pageIDDelimiter = "";

    [Header("UI Page Settings")]
    [SerializeField] UIPageType uIPageType;
    [SerializeField] OpenPageType openPageType = OpenPageType.ClosePreviousPage;

    public string PageID => $"{pageIDPrefix}{pageIDDelimiter}{pageID}" + 
                            $"{(string.IsNullOrEmpty(pageIDSuffix) ? "" : pageIDDelimiter + pageIDSuffix)}";
    public UIPageType UIPageType => uIPageType;
    public OpenPageType OpenPageType => openPageType;

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        // To be used later
    }

    public void SetAssetNameToID()
    {
        if (name != pageID)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(pageID) || string.IsNullOrEmpty(assetPath)) return;

            string error = UnityEditor.AssetDatabase.RenameAsset(assetPath, pageID);

            if (!string.IsNullOrEmpty(error))
                LogUtilities.LogError($"Failed to rename asset: {error}", GetType().Name);
            else
                UnityEditor.AssetDatabase.SaveAssets();
        }        
    }
#endif
}
