using UnityEditor;
using UnityEngine;

public class UIAnimationBaseEditor<T> : Editor
{
    protected MonoScript monoScript;

    protected UIAnimationBase uIAnimationBase;

    protected SerializedProperty animationID;
    protected SerializedProperty duration, durationExit;
    protected SerializedProperty delay, delayExit;
    protected SerializedProperty leanTweenType, leanTweenTypeExit;
    protected SerializedProperty ignoreTimeScale;
    protected SerializedProperty repeatType;
    protected SerializedProperty waitForOpenAnimationToFinish, waitForCloseAnimationToFinish;
    protected SerializedProperty checkAnimatorGroupID, checkAnimatorID, checkAnimationGroupID;
    protected SerializedProperty signalsSceneTransitionCloseAnimationDone;
    protected SerializedProperty excludeOpenAnimationFromCount, excludeCloseAnimationFromCount;

    protected TAnim GetReference<TAnim>() where TAnim : UIAnimationBase => (TAnim)GetReference();

    protected UIAnimationBase GetReference()
    {
        if (uIAnimationBase == null)
            uIAnimationBase = (UIAnimationBase)target;

        return uIAnimationBase != null ? uIAnimationBase : null;
    }

    protected void FindBaseProperties()
    {
        animationID = serializedObject.FindProperty("animationID");

        duration = serializedObject.FindProperty("duration");
        durationExit = serializedObject.FindProperty("durationExit");

        delay = serializedObject.FindProperty("delay");
        delayExit = serializedObject.FindProperty("delayExit");

        leanTweenType = serializedObject.FindProperty("leanTweenType");
        leanTweenTypeExit = serializedObject.FindProperty("leanTweenTypeExit");

        ignoreTimeScale = serializedObject.FindProperty("ignoreTimeScale");
        repeatType = serializedObject.FindProperty("repeatType");
        waitForOpenAnimationToFinish = serializedObject.FindProperty("waitForOpenAnimationToFinish");
        waitForCloseAnimationToFinish = serializedObject.FindProperty("waitForCloseAnimationToFinish");
        checkAnimatorGroupID = serializedObject.FindProperty("checkAnimatorGroupID");
        checkAnimatorID = serializedObject.FindProperty("checkAnimatorID");
        checkAnimationGroupID = serializedObject.FindProperty("checkAnimationGroupID");
        signalsSceneTransitionCloseAnimationDone = serializedObject.FindProperty("signalsSceneTransitionCloseAnimationDone");
        excludeOpenAnimationFromCount = serializedObject.FindProperty("excludeOpenAnimationFromCount");
        excludeCloseAnimationFromCount = serializedObject.FindProperty("excludeCloseAnimationFromCount");
    }

    protected void DrawMonoScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", monoScript, typeof(T), true);
        EditorGUI.EndDisabledGroup();
    }

    protected void DrawButtonHelpers()
    {
        if (GUILayout.Button("Set Animation ID by Asset"))
        {
            uIAnimationBase.SetAnimationID();
        }
    }

    protected void DrawBaseProperties()
    {
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(animationID);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(durationExit);

        EditorGUI.BeginDisabledGroup(uIAnimationBase.WaitForOpenAnimationToFinish);
        EditorGUILayout.PropertyField(delay);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(uIAnimationBase.WaitForCloseAnimationToFinish);
        EditorGUILayout.PropertyField(delayExit);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Easing", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(leanTweenType);
        EditorGUILayout.PropertyField(leanTweenTypeExit);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(ignoreTimeScale);
        EditorGUILayout.PropertyField(repeatType);
        EditorGUILayout.PropertyField(waitForOpenAnimationToFinish);
        EditorGUILayout.PropertyField(waitForCloseAnimationToFinish);
        EditorGUILayout.PropertyField(checkAnimatorGroupID);
        EditorGUILayout.PropertyField(checkAnimatorID);
        EditorGUILayout.PropertyField(checkAnimationGroupID);
        EditorGUILayout.PropertyField(signalsSceneTransitionCloseAnimationDone);
        EditorGUILayout.PropertyField(excludeOpenAnimationFromCount);
        EditorGUILayout.PropertyField(excludeCloseAnimationFromCount);
    }
}