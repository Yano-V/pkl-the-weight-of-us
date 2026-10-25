using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIAnimatorGroup : MonoBehaviour
{
    public event Action<UIAnimatorGroup> OnAnimatorGroupDone;

    [SerializeField] List<IUIAnimator> animators = new();

    [SerializeField] List<IUIAnimator> loopers = new();

    [SerializeField] UIAnimHandler uIAnimHandler;
    [SerializeField] UIAnimatorGroupID uIAnimatorGroupID;

    [Header("Debug Values")]
    [Tooltip("Number of animators that have completed their animations.")]
    [SerializeField] int doneAnimating = 0;

    public UIAnimatorGroupID UIAnimatorGroupID => uIAnimatorGroupID;

    #region Unity Methods
    private void Awake()
    {
        if (uIAnimHandler != null)
            uIAnimHandler.RegisterGroup(this);
    }

    private void OnDestroy()
    {
        if (uIAnimHandler != null)
            uIAnimHandler.UnregisterGroup(this);
    }
    #endregion

    #region Event Callbacks
    void HandleAnimatorDone(IUIAnimator animator)
    {
        doneAnimating++;

        if (doneAnimating == animators.Count)
        {
            OnAnimatorGroupDone?.Invoke(this);
        }
        animator.OnAnimatorDone -= HandleAnimatorDone;
    }
    #endregion

    public void OpenAnimation()
    {
        if (animators.Count == 0)
        {
            OnAnimatorGroupDone?.Invoke(this);
            return;
        }

        doneAnimating = 0;

        foreach (IUIAnimator anim in animators)
        {
            anim.OnAnimatorDone += HandleAnimatorDone;
            anim.OpenAnimation();
        }

        foreach (IUIAnimator looper in loopers)
        {
            looper.OpenAnimation();
        }
    }

    public void CloseAnimation()
    {
        if (animators.Count == 0)
        {
            OnAnimatorGroupDone?.Invoke(this);
            return;            
        }   

        doneAnimating = 0;
        
        foreach (IUIAnimator anim in animators)
        {
            anim.OnAnimatorDone += HandleAnimatorDone;
            anim.CloseAnimation();
        }
    }

    public void StopAnimation()
    {
        if (animators.Count == 0) return;

        doneAnimating = 0;

        foreach (IUIAnimator anim in animators)
        {
            anim.OnAnimatorDone -= HandleAnimatorDone;
            anim.StopAnimation();
        }
    }

    public void RegisterAnimator(IUIAnimator animator)
    {
        if (!animators.Contains(animator) && !animator.IsLooper)
            animators.Add(animator);
        else if (!loopers.Contains(animator) && animator.IsLooper)
            loopers.Add(animator);
    }

    public void UnregisterAnimator(IUIAnimator animator)
    {
        if (animators.Contains(animator) && !animator.IsLooper)
            animators.Remove(animator);
        else if (loopers.Contains(animator) && animator.IsLooper)
            loopers.Remove(animator);                                                                                                                       
    }

    #if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (uIAnimHandler == null)
            uIAnimHandler = GetComponentInParent<UIAnimHandler>();
    }

    public void SetGameObjectName()
    {
        string groupIDName = uIAnimatorGroupID != null ? uIAnimatorGroupID.name : "";
        string autoSetName = $"{groupIDName}AnimGroup";

        if (name != autoSetName)
            name = autoSetName;
    }
    #endif
}