using System;

public interface IUIAnimator
{
    event Action<IUIAnimator> OnAnimatorDone;

    int MoveTweenID { get; set; }
    int ScaleTweenID { get; set; }
    int RotateTweenID { get; set; }
    int FadeTweenID { get; set; }

    bool IsLooper { get; }

    string AnimatorGroupID { get; }
    string animatorID { get; }

    void OpenAnimation();
    void CloseAnimation();
    void StopAnimation();
}

public interface IButtonAnimator : IUIAnimator
{
    void SelectAnimation(); // Animation for when the button is selected
    void DeselectAnimation(); // Animation for when the button is deselected
    void UninteractableAnimation(); // Animation for when the button is not interactable (interactable = false)
    void InteractableAnimation(); // Animation for when the button is interactable (interactable = true)
}