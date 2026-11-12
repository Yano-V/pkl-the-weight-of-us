using UnityEngine;

[CreateAssetMenu(fileName = "Limbo Rotate Animation", menuName = "Limbo/UI/UI Animations/Rotate Animation")]
public class RotateAnimation : UIAnimationBase
{
    private Vector3 initialRotation = Vector3.zero;
    private bool hasInitializedRotation = false;

    [SerializeField] bool setStartingRotationToInitialState = false;
    [SerializeField] Vector3 startingRotation = Vector3.zero;
    [SerializeField] Vector3 targetRotation = Vector3.zero;
    [SerializeField] Vector3 targetRotationExit = Vector3.zero;
    [SerializeField] bool useExitRotation = false;

    public override void OpenAnimation()
    {
        uIAnimDoneType = UIAnimType.Open;

        if (!hasInitializedRotation)
        {
            initialRotation = rectTransform.localEulerAngles;

            if (setStartingRotationToInitialState)
                startingRotation = initialRotation;

            hasInitializedRotation = true;
        }

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

    void TweenRotation(Vector3 value)
    {
        rectTransform.localRotation = Quaternion.Euler(value);
    }

    protected override void OpenAnimationLogic()
    {
        float delayToUse = WaitForOpenAnimationToFinish ? 0f : delay;
        // LTDescr tween = LeanTween.rotate(rectTransform.gameObject, targetRotation, duration)
        LTDescr tween = LeanTween.value(rectTransform.gameObject, startingRotation, targetRotation, duration)
            .setEase(leanTweenType)
            .setDelay(delayToUse)
            .setOnUpdateVector3(TweenRotation)
            .setIgnoreTimeScale(ignoreTimeScale)
            .setOnComplete(() =>
            {
                if (!isLooper)
                {
                    RotateTweenID = -1;
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

        RotateTweenID = tween.id;
    }

    protected override void CloseAnimationLogic()
    {
        if (!hasInitializedRotation)
        {
            initialRotation = rectTransform.localEulerAngles;

            if (setStartingRotationToInitialState)
            {
                startingRotation = initialRotation;
            }

            hasInitializedRotation = true;
        }

        float delayToUse = WaitForCloseAnimationToFinish ? 0f : delayExit;

        if (!useExitRotation)
        {
            // RotateTweenID = LeanTween.rotate
            RotateTweenID = LeanTween.value
                (
                    rectTransform.gameObject,
                    rectTransform.localEulerAngles,
                    startingRotation,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnUpdateVector3(TweenRotation)
                .setOnComplete(() =>
                {
                    RotateTweenID = -1;
                    SetAnimationDone();
                })
                .setIgnoreTimeScale(ignoreTimeScale)
                .id;
        }
        else
        {
            // RotateTweenID = LeanTween.rotate
            RotateTweenID = LeanTween.value
                (
                    rectTransform.gameObject,
                    rectTransform.localEulerAngles,
                    targetRotationExit,
                    durationExit
                )
                .setEase(leanTweenTypeExit)
                .setDelay(delayToUse)
                .setOnUpdateVector3(TweenRotation)
                .setOnComplete(() =>
                {
                    RotateTweenID = -1;
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