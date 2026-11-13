using UnityEditor;

[CustomEditor(typeof(UIServiceButtonHandler))]
public class UIServiceButtonHandlerEditor : Editor
{
    private UIServiceButtonHandler uIServiceButtonHandler;

    private void OnEnable()
    {
        uIServiceButtonHandler = (UIServiceButtonHandler)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        uIServiceButtonHandler.AutoSetReferenceSettings();
    }
}