using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPage))]
[DisallowMultipleComponent]
public class UIAnimHandler : MonoBehaviour
{
    [SerializeField] UIPage uIPage;
    
    [SerializeField] List<UIAnimatorGroup> animatorGroups = new();

    [Header("Debug Values")]
    [SerializeField] bool isAnimating = false;
    [Tooltip("Number of animator groups that have completed their animations.")]
    [SerializeField] int doneAnimating = 0;

    private UIAnimType uIAnimDoneType;

    public bool HasAnAnimator => animatorGroups.Count > 0;

    #region Event Callbacks
    void HandleAnimatorGroupDone(UIAnimatorGroup group)
    {
        doneAnimating++;

        if (doneAnimating == animatorGroups.Count)
        {
            isAnimating = false;

            uIPage.UIAnimatorFinished(uIAnimDoneType);

            LogUtilities.Log($"All Animations Completed for {uIPage.name}", GetType().Name, gameObject);
        }
        group.OnAnimatorGroupDone -= HandleAnimatorGroupDone;
    }
    #endregion

    public void PlayOpenAnimation()
    {
        if (!HasAnAnimator) return;

        uIAnimDoneType = UIAnimType.Open;

        if (isAnimating)
        {
            foreach (UIAnimatorGroup animator in animatorGroups)
            {
                animator.StopAnimation();
                animator.OnAnimatorGroupDone -= HandleAnimatorGroupDone;
            }
        }

        isAnimating = true;
        ResetDoneCount();

        foreach (UIAnimatorGroup animator in animatorGroups)
        {
            animator.OnAnimatorGroupDone += HandleAnimatorGroupDone;
            animator.OpenAnimation();
        }
    }

    public void PlayCloseAnimation()
    {
        if (!HasAnAnimator) return;

        uIAnimDoneType = UIAnimType.Close;

        foreach (UIAnimatorGroup animator in animatorGroups)
        {
            animator.StopAnimation();
            animator.OnAnimatorGroupDone -= HandleAnimatorGroupDone;
        }

        isAnimating = true;
        ResetDoneCount();

        foreach (UIAnimatorGroup animator in animatorGroups)
        {
            animator.OnAnimatorGroupDone += HandleAnimatorGroupDone;
            animator.CloseAnimation();
        }
    }

    public void RegisterGroup(UIAnimatorGroup animator)
    {
        if (!animatorGroups.Contains(animator))
        {
            animatorGroups.Add(animator);
            LogUtilities.Log($"Added group to UI Animation Handler: {animator.name}", GetType().Name, gameObject);
        }
    }

    public void UnregisterGroup(UIAnimatorGroup animator)
    {
        if (animatorGroups.Contains(animator))
        {
            animatorGroups.Remove(animator);
            LogUtilities.Log($"Removed group to UI Animation Handler: {animator.name}", GetType().Name, gameObject);
        }
    }

    void ResetDoneCount() => doneAnimating = 0;

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (uIPage == null)
            uIPage = GetComponent<UIPage>();
    }
#endif
}