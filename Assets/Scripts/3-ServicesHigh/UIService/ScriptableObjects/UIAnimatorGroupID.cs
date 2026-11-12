using UnityEngine;

[CreateAssetMenu(fileName = "Limbo UI Animator Group ID", menuName = "Limbo/UI/UI Animator Group ID")]
public class UIAnimatorGroupID : ScriptableObject
{
    [SerializeField] string animatorGroupID;

    public string AnimGroupID => animatorGroupID;

    public void SetAnimatorGroupID()
    {
        if (animatorGroupID != name)
            animatorGroupID = name;
    }

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (string.IsNullOrEmpty(animatorGroupID))
        {
            animatorGroupID = name;
        }
    }
#endif
}