using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimatorLooper : UIAnimator
{
    public override bool IsLooper => true;

    private void OnDisable()
    {
        if (instantiatedAnimGroups.Count == 0) return;

        ResetDoneCount();
        isAnimating = false;

        foreach (UIAnimationGroup animGroup in instantiatedAnimGroups)
        {
            animGroup.StopAnimation();
        }
    }

    public override void OpenAnimation()
    {
        if (instantiatedAnimGroups.Count == 0) return;

        foreach (UIAnimationGroup animGroup in instantiatedAnimGroups)
        {
            animGroup.OpenAnimation();
        }
    }

    public override void CloseAnimation() {} // No Logic. Looper does not use the close logic
    public override void StopAnimation() {} // No Lugic. Looper does not use the stop logic
}