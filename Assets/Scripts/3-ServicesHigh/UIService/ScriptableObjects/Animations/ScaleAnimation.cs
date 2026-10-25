using UnityEngine;

[CreateAssetMenu(fileName = "Limbo Scale Animation", menuName = "Limbo/UI/UI Animations/Scale Animation")]
public class ScaleAnimation : UIAnimationBase
{
    private Vector3 initialScale = Vector3.one;
    private bool hasInitializedScale = false;

    [SerializeField] bool setStartingScaleToInitialState = false;
    [SerializeField] Vector3 startingScale = Vector3.one;
    [SerializeField] Vector3 targetScale = Vector3.one;
    [SerializeField] Vector3 targetScaleExit = Vector3.one;
    [SerializeField] bool useExitScale = false;

    public override void OpenAnimation()
    {
        uIAnimDoneType = UIAnimType.Open;

        if (!hasInitializedScale)
        {
            initialScale = rectTransform.localScale;

            if (setStartingScaleToInitialState)
                startingScale = initialScale;

            hasInitializedScale = true;
        }

        rectTransform.localScale = startingScale;

        if (WaitForOpenAnimationToFinish)
        {
            uIPageHandler.OnDonePlaying += HandleWaitAnimationDone;
            return;
        }

        OpenAnimationLogic();
    }

    public override void CloseAnimation()
    {
        uIAnimDoneType = UIAnimType.Close;

        if (WaitForCloseAnimationToFinish)
        {
            uIPageHandler.OnDonePlaying += HandleWaitAnimationDone;
            return;
        }

        CloseAnimationLogic();
    }

    protected override void OpenAnimationLogic()
    {
        float delayToUse = WaitForOpenAnimationToFinish ? 0f : delay;

        LTDescr tween = LeanTween.scale
            (
                rectTransform.gameObject,
                targetScale,
                duration
            )
            .setEase(leanTweenType)
            .setDelay(delayToUse)
            .setIgnoreTimeScale(ignoreTimeScale)
            .setOnComplete(() =>
            {
                if (!isLooper)
                {
                    ScaleTweenID = -1;
                    SetAnimationDone();
                }
            });

        if (isLooper)
        {
            switch (repeatType)
            {
                case RepeatType.Repeat:
                    tween.setLoopClamp().setRepeat(-1);
                    break;
                case RepeatType.PingPong:
                    tween.setLoopPingPong(-1);
                    break;
                case RepeatType.None:
                default:
                    break;
            }
        }

        ScaleTweenID = tween.id;
    }

    protected override void CloseAnimationLogic()
    {
        if (!hasInitializedScale)
        {
            initialScale = rectTransform.localScale;

            if (setStartingScaleToInitialState)
            {
                startingScale = initialScale;
            }

            hasInitializedScale = true;
        }

        float delayToUse = WaitForCloseAnimationToFinish ? 0f : delayExit;

        if (!useExitScale)
        {
            ScaleTweenID = LeanTween.scale
                (
                    rectTransform.gameObject,
                    startingScale,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnComplete(() =>
                {
                    ScaleTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
        else
        {
            ScaleTweenID = LeanTween.scale
                (
                    rectTransform.gameObject,
                    targetScaleExit,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnComplete(() =>
                {
                    ScaleTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
    }

    public override void StopAnimation()
    {
        base.StopAnimation();

        uIPageHandler.OnDonePlaying -= HandleWaitAnimationDone;
    }
}