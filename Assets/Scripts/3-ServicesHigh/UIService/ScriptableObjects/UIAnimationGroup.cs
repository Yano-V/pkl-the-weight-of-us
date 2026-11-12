using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Limbo UI Animation Group", menuName = "Limbo/UI/UI Animation Group")]
public class UIAnimationGroup : ScriptableObject
{
    private IUIAnimator uIAnimator;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private UIPageHandler uIPageHandler;

    private bool isLooper;

    private UIAnimType uIAnimDoneType;

    public event Action<UIAnimationGroup> OnAnimationGroupDone;

    private int doneAnimating = 0;

    [SerializeField] List<UIAnimationBase> animations = new();

    private readonly List<UIAnimationBase> animationsInstantiated = new();

    public IReadOnlyList<UIAnimationBase> Animations => animations;
    public string UIAnimationGroupID => name;

    #region Initializations
    public void Initialize(IUIAnimator uIAnimator, 
                           RectTransform rectTransform,
                           CanvasGroup canvasGroup,
                           UIPageHandler uIPageHandler,
                           bool isLooper)
    { 
        this.uIAnimator = uIAnimator;
        this.rectTransform = rectTransform;
        this.canvasGroup = canvasGroup;
        this.uIPageHandler = uIPageHandler;

        this.isLooper = isLooper;

        InstantiateAnimations();
    }
    #endregion

    #region Event Callbacks
    void HandleAnimationDone(UIAnimationBase animation)
    {
        animation.OnAnimationDone -= HandleAnimationDone;
        if (!IsAnimationCounted(animation)) return;

        doneAnimating++;

        if (doneAnimating >= AnimationCount())
        {
            OnAnimationGroupDone?.Invoke(this);
        }
    }

    #endregion

    #region Public Methods
    public void OpenAnimation()
    {
        uIAnimDoneType = UIAnimType.Open;

        if (animationsInstantiated.Count == 0)
        {
            OnAnimationGroupDone?.Invoke(this);
            return;
        }

        ResetDoneCount();

        foreach (UIAnimationBase anim in animationsInstantiated)
        {
            anim.OnAnimationDone -= HandleAnimationDone;
            if (IsAnimationCounted(anim))
                anim.OnAnimationDone += HandleAnimationDone;
            anim.OpenAnimation();
        }

        if (AnimationCount() == 0)
            OnAnimationGroupDone?.Invoke(this);

        // for (int i = 0; i < animationsInstantiated.Count; i++)
        // {
        //     animationsInstantiated[i].OpenAnimation();
        //     animationsInstantiated[i].OnAnimationDone += HandleAnimationDone;
        // }
    }

    public void CloseAnimation()
    {
        uIAnimDoneType = UIAnimType.Close;

        if (animationsInstantiated.Count == 0)
        {
            OnAnimationGroupDone?.Invoke(this);
            return;
        }
        
        ResetDoneCount();

        foreach (UIAnimationBase anim in animationsInstantiated)
        {
            anim.OnAnimationDone -= HandleAnimationDone;
            if (IsAnimationCounted(anim))
                anim.OnAnimationDone += HandleAnimationDone;
            anim.CloseAnimation();
        }

        if (AnimationCount() == 0)
            OnAnimationGroupDone?.Invoke(this);

        // for (int i = 0; i < animationsInstantiated.Count; i++)
        // {
        //     animationsInstantiated[i].CloseAnimation();
        // }
    }

    public void StopAnimation()
    {
        if (animationsInstantiated.Count == 0) return;

        ResetDoneCount();

        foreach (UIAnimationBase anim in animationsInstantiated)
        {
            anim.OnAnimationDone -= HandleAnimationDone;
            anim.StopAnimation();
        }

        // for (int i = 0; i < animationsInstantiated.Count; i++)
        // {
        //     animationsInstantiated[i].StopAnimation();
        // }
    }

    public void ReinstantiateAnimations() // For editor 
    {
        RemoveInstantiatedAnimations();
        InstantiateAnimations();
    }

    public void RemoveInstantiatedAnimations()
    {
        for (int i = animationsInstantiated.Count - 1; i >= 0; i--)
        {
            Destroy(animationsInstantiated[i]);
            animationsInstantiated.RemoveAt(i);
        }
    }
    #endregion

    #region Private Methods
    void InstantiateAnimations()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i] == null) continue;
            
            UIAnimationBase animBase = Instantiate(animations[i]);

            if (animBase is FadeAnimation)
                animBase.Initialize(this, uIAnimator, rectTransform, uIPageHandler, isLooper, canvasGroup);
            else
                animBase.Initialize(this, uIAnimator, rectTransform, uIPageHandler, isLooper);

            animationsInstantiated.Add(animBase);
        }
    }

    void ResetDoneCount() => doneAnimating = 0;
    #endregion

    #region Private Returning Methods
    int AnimationCount()
    {
        int count = 0;

        if (uIAnimDoneType == UIAnimType.Open)
        {            
            foreach (UIAnimationBase animBase in animationsInstantiated)
            {
                if (animBase.ExcludeOpenAnimationFromCount) continue;
                count++;
            }
        }
        else if (uIAnimDoneType == UIAnimType.Close)
        {
            foreach (UIAnimationBase animBase in animationsInstantiated)
            {
                if (animBase.ExcludeCloseAnimationFromCount) continue;
                count++;
            }
        }

        return count;
    }

    bool IsAnimationCounted(UIAnimationBase animation)
    {
        return uIAnimDoneType switch
        {
            UIAnimType.Open => !animation.ExcludeOpenAnimationFromCount,
            UIAnimType.Close => !animation.ExcludeCloseAnimationFromCount,
            _ => false
        };
    }
    #endregion
}