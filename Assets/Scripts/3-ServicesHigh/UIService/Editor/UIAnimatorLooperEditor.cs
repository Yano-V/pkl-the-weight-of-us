using UnityEditor;

[CustomEditor(typeof(UIAnimatorLooper))]
[CanEditMultipleObjects]
public class UIAnimatorLooperEditor : Editor
{
    private UIAnimatorLooper uIAnimator;

    private void OnEnable()
    {
        uIAnimator = (UIAnimatorLooper)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        uIAnimator.AutoSetReferenceSettings();
    }


}