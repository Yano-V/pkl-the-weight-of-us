using System;
using UnityEngine;

public abstract class UIAnimationBase : ScriptableObject
{
    protected UIAnimationGroup uIAnimationGroup;
    protected IUIAnimator uIAnimator;
    protected RectTransform rectTransform;
    protected UIPageHandler uIPageHandler;
    protected CanvasGroup canvasGroup;

    protected UIAnimType uIAnimDoneType;

    [SerializeField] protected string animationID;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected float durationExit = 0.5f;
    [SerializeField] protected float delay = 0f;
    [SerializeField] protected float delayExit = 0f;
    [SerializeField] protected LeanTweenType leanTweenType = LeanTweenType.easeOutQuad;
    [SerializeField] protected LeanTweenType leanTweenTypeExit = LeanTweenType.easeOutQuad;
    [SerializeField] protected bool ignoreTimeScale = true;
    [SerializeField] protected RepeatType repeatType;
    [SerializeField] protected UIAnimationBase waitForOpenAnimationToFinish;
    [SerializeField] protected UIAnimationBase waitForCloseAnimationToFinish;
    [SerializeField] protected bool checkAnimatorGroupID = false;
    [SerializeField] protected bool checkAnimatorID = false;
    [SerializeField] protected bool checkAnimationGroupID = false;
    [SerializeField] protected bool sendCloseAnimationDoneFlag = false;
    [SerializeField] protected bool excludeOpenAnimationFromCount = false;
    [SerializeField] protected bool excludeCloseAnimationFromCount = false;

    protected bool isLooper = false;
    protected int repeatValue;
    protected int pingPongValue;

    protected string animatorGroupID;
    protected string animatorID;
    protected string animationGroupID;

    protected int MoveTweenID { get => uIAnimator.MoveTweenID; set => uIAnimator.MoveTweenID = value; }
    protected int ScaleTweenID { get => uIAnimator.ScaleTweenID; set => uIAnimator.ScaleTweenID = value; }
    protected int RotateTweenID { get => uIAnimator.RotateTweenID; set => uIAnimator.RotateTweenID = value; }
    protected int FadeTweenID { get => uIAnimator.FadeTweenID; set => uIAnimator.FadeTweenID = value; }

    public bool WaitForOpenAnimationToFinish => waitForOpenAnimationToFinish != null;
    public bool WaitForCloseAnimationToFinish => waitForCloseAnimationToFinish != null;
    public bool ExcludeOpenAnimationFromCount => excludeOpenAnimationFromCount;
    public bool ExcludeCloseAnimationFromCount => excludeCloseAnimationFromCount;
    public string AnimationID => animationID;

    public UIAnimType UIAnimDoneType => uIAnimDoneType;

    public event Action<UIAnimationBase> OnAnimationDone;

    public void Initialize(UIAnimationGroup uIAnimationGroup,
                           IUIAnimator uIAnimator,
                           RectTransform rectTransform,
                           UIPageHandler uIPageHandler,
                           bool isLooper,
                           CanvasGroup canvasGroup = null)
    {
        this.uIAnimationGroup = uIAnimationGroup;
        this.uIAnimator = uIAnimator;
        this.rectTransform = rectTransform;
        this.uIPageHandler = uIPageHandler;

        this.isLooper = isLooper;

        this.canvasGroup = canvasGroup;

        animatorGroupID = this.uIAnimator.AnimatorGroupID;
        animatorID = this.uIAnimator.animatorID;
        animationGroupID = this.uIAnimationGroup.UIAnimationGroupID;
    }

    public abstract void OpenAnimation();
    public abstract void CloseAnimation();
    public virtual void StopAnimation()
    {
        if (MoveTweenID != -1)
        {
            LeanTween.cancel(MoveTweenID);
            MoveTweenID = -1;
        }

        if (ScaleTweenID != -1)
        {
            LeanTween.cancel(ScaleTweenID);
            ScaleTweenID = -1;
        }

        if (RotateTweenID != -1)
        {
            LeanTween.cancel(RotateTweenID);
            RotateTweenID = -1;
        }

        if (FadeTweenID != -1)
        {
            LeanTween.cancel(FadeTweenID);
            FadeTweenID = -1;
        }

        repeatValue = 0;
        pingPongValue = 0;
    }

    protected abstract void OpenAnimationLogic();
    protected abstract void CloseAnimationLogic();
    
    protected void HandleWaitAnimationDone(AnimationDoneData animationData)
    {
        if (animationData.UIAnimType != uIAnimDoneType) return;

        switch (uIAnimDoneType)
        {
            case UIAnimType.Open:
                {
                    if (!IsWaitAnimationDone(waitForOpenAnimationToFinish, animationData)) return;

                    uIPageHandler.OnDonePlaying -= HandleWaitAnimationDone;
                    OpenAnimationLogic();
                    break;
                }
            case UIAnimType.Close:
                {
                    if (!IsWaitAnimationDone(waitForCloseAnimationToFinish, animationData)) return;

                    uIPageHandler.OnDonePlaying -= HandleWaitAnimationDone;
                    CloseAnimationLogic();
                    break;
                }
            default: break;
        }
    }

    bool IsWaitAnimationDone(UIAnimationBase waitAnimation, AnimationDoneData animationData)
    {
        return waitAnimation != null
            && waitAnimation.animationID == animationData.AnimationID
            && (!checkAnimatorGroupID || animatorGroupID == animationData.AnimatorGroupID)
            && (!checkAnimatorID || animatorID == animationData.AnimatorID)
            && (!checkAnimationGroupID || animationGroupID == animationData.AnimationGroupID);
    }

    protected void SetAnimationDone()
    {
        AnimationDoneData data = new(animatorGroupID,
                                     animatorID,
                                     animationID,
                                     animationGroupID,
                                     sendCloseAnimationDoneFlag,
                                     uIAnimDoneType);

        uIPageHandler.SendDonePlayingEvent(data);
        OnAnimationDone?.Invoke(this);
    }

    public void SetAnimationID()
    {
        if (animationID != name)
            animationID = name;
    }

#if UNITY_EDITOR
    public void AutoSetReferenceSettings()
    {
        if (string.IsNullOrEmpty(animationID))
        {
            animationID = name;
        }
    }
#endif
}