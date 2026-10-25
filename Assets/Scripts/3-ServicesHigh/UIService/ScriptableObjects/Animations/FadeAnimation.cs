using UnityEngine;

[CreateAssetMenu(fileName = "Limbo Fade Animation", menuName = "Limbo/UI/UI Animations/Fade Animation")]
public class FadeAnimation : UIAnimationBase
{
    [SerializeField] float targetAlpha = 1f;
    [SerializeField] float targetAlphaExit = 0f;
    [SerializeField] bool useExitAlpha = false;

    public override void OpenAnimation()
    {
        uIAnimDoneType = UIAnimType.Open;

        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0;

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

        LTDescr tween = LeanTween.alphaCanvas
            (
                canvasGroup,
                targetAlpha,
                duration
            )
            .setEase(leanTweenType)
            .setDelay(delayToUse)
            .setIgnoreTimeScale(ignoreTimeScale)
            .setOnComplete(() =>
            {
                if (!isLooper)
                {
                    FadeTweenID = -1;
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

        FadeTweenID = tween.id;
    }

    protected override void CloseAnimationLogic()
    {
        if (canvasGroup == null)
            return;

        float delayToUse = WaitForCloseAnimationToFinish ? 0f : delayExit;

        if (!useExitAlpha)
        {
            FadeTweenID = LeanTween.alphaCanvas
                (
                    canvasGroup,
                    0,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnComplete(() =>
                {
                    FadeTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
        else
        {
            FadeTweenID = LeanTween.alphaCanvas
                (
                    canvasGroup,
                    targetAlphaExit,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnComplete(() =>
                {
                    FadeTweenID = -1;
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