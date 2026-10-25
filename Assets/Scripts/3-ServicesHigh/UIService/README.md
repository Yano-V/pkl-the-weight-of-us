# UIService

`UIService` is the central runtime registry and navigation helper for UI pages. It registers page instances by `UIPageSO.PageID`, maintains an opened-page stack, hosts loading pages on a dedicated canvas, and coordinates page and loading animations with the scene service.

## Components and responsibilities

| Type | Responsibility |
| --- | --- |
| [`UIService`](UIService.cs) | Registers the `IUIService`, creates the page handler, and instantiates configured loading-page prefabs. |
| [`IUIService`](../../0-Core/Services/MonoServices/UIService/IUIService.cs) | Service contract exposing settings, the page handler, and loading canvas. |
| [`UIPageHandler`](UIPageHandler.cs) | Page/loading registries, opened-page stack, page navigation, scene-event integration, and animation completion events. |
| [`UIPage`](UIPage.cs) | Component on each page root; registers itself and controls its content, selectables, and optional page animations. |
| [`UIPageSO`](../../0-Core/Services/MonoServices/UIService/UIPage/UIPageSO.cs) | Asset defining a page ID, page category, and navigation behavior. |
| [`UIServiceSettingsSO`](ScriptableObjects/Settings/UIServiceSettingsSO.cs) | Service settings and loading-page prefab list. |
| [`UIAnimHandler`](UIAnimHandler.cs) | Coordinates animation groups attached to one page and reports when all groups finish. |
| [`UIAnimatorGroup`](UIAnimatorGroup.cs) | Groups regular animators and loopers, and aggregates completion from regular animators. |
| [`UIAnimator`](UIAnimator.cs) / [`UIAnimatorLooper`](UIAnimatorLooper.cs) | Runs configured animation groups; the looper variant runs repeating open animations without participating in completion counts. |
| [`UIAnimationGroup`](ScriptableObjects/UIAnimationGroup.cs) | Instantiates and coordinates animation assets for a UI animator. |
| [`UIAnimationBase`](ScriptableObjects/UIAnimationBase.cs) | Base asset API for animation configuration, sequencing, cancellation, and completion reporting. |
| [`FadeAnimation`](ScriptableObjects/Animations/FadeAnimation.cs), [`MoveAnimationOffset`](ScriptableObjects/Animations/MoveAnimationOffset.cs), [`RotateAnimation`](ScriptableObjects/Animations/RotateAnimation.cs), [`ScaleAnimation`](ScriptableObjects/Animations/ScaleAnimation.cs) | Built-in LeanTween animation assets. |
| [`IUIServiceSelectable`](Interfaces/IUIServiceSelectable.cs) | Contract for controls whose interactability is enabled/disabled by a page. |
| [`IUIAnimator`](Interfaces/IUIAnimator.cs) | Contract used by animator groups to start, stop, and track animators. |
| [`AnimationDoneData`](Structs/AnimationDoneData.cs) | Identifies a completed animation and carries its close-flag and animation-type metadata. |
| [`UIAnimatorGroupID`](ScriptableObjects/UIAnimatorGroupID.cs) | Stable ID asset used to identify animator groups in completion data. |

Related scene-transition contracts are [`ISceneServiceManageable`](../../0-Core/Services/MonoServices/SceneService/ISceneService.cs) and [`GameSceneService`](../../2-ServicesMid/GameSceneService/GameSceneService.cs). `GameSceneService` uses [`WaitUntilLoadingPageDone`](../../2-ServicesMid/GameSceneService/CustomYieldInstructions/WaitUntilLoadingPageDone.cs) while waiting for loading-page animation completion.

## Setup

1. Add `UIService` to the scene/service bootstrap and assign its `UIServiceSettingsSO` and loading `Canvas` references. The `ISceneServiceManageable` must already be registered when `UIService.Awake` runs. Only the first registered `IUIService` is kept; a duplicate service GameObject destroys itself.
2. Create a `UIServiceSettingsSO` asset (`Limbo > UI > UI Service Settings`). Add loading-page prefabs to **Loading Page Prefabs**. They are instantiated by the service during `Awake`; null entries are ignored.
3. Create a `UIPageSO` for each page (`Limbo > UI > UI Page`). Its computed `PageID` is the runtime registry key and must be unique among live page instances. Set its `UIPageType` and `OpenPageType`.
4. Add `UIPage` to each page root and assign its `UIPageSO` and content GameObject. The content reference is used directly by `SetActive`, so it must be assigned. For loading pages, the service also requires the loading Canvas and reparents the registered page root under that canvas.
5. Set each scene's default page on its scene data to a matching `UIPageSO`. The page must be registered by the time the scene service delivers its completion event.
6. For animated pages, enable `useAnimator` on `UIPage`, assign an enabled `UIAnimHandler`, and configure one or more `UIAnimatorGroup`s. Assign group IDs and animator assets as required by their references. Fade animations require a `CanvasGroup` on the animator; all animators require a `RectTransform`.

The page component obtains both services from `GameServices` in `Awake`, so service initialization/order is part of the scene setup contract. A missing settings asset, canvas, page asset, or content reference is not handled as an optional configuration.

## UIPage GameObject anatomy example

The following tree is based on `Project-LCG-RPG/Assets/Assets/Prefabs/UI/LoadingPages/Loading_LoadingCommonRoot.prefab`. It illustrates the separation between the page-management root, the assigned content root, and the visual/animated UI beneath it. The prefab's `UIPageSO` is `LoadingCommon` (`PageID: Loading_LoadingCommon`, type `Loading`).

```text
Loading_LoadingCommonRoot  [RectTransform, UIPage, UIAnimHandler]
|  UIPage
|    UIPageSO       -> LoadingCommon
|    contentObj     -> Loading_LoadingCommonContents
|    useAnimator    -> true
|    uIAnimHandler  -> UIAnimHandler on this root
|
+-- Loading_LoadingCommonContents  [RectTransform, tag: UIContent]
	|
	+-- ID-Loading-LoadingCommonAnimGroup
	|   [RectTransform, UIAnimatorGroup, Image, UIAnimator, CanvasGroup]
	|   |  Group ID -> ID-Loading-LoadingCommon
	|   |
	|   +-- LoadingCircleRoot  [RectTransform, UIAnimator]
	|   |   +-- LoadingCircleOutLayer  [RectTransform, UIAnimatorLooper]
	|   |       +-- LoadingCircleInLayer  [RectTransform, Image, UIAnimatorLooper]
	|   |
	|   +-- TipsRoot  [RectTransform]
	|       +-- Line       [RectTransform, Image, UIAnimator]
	|       +-- TipHeader  [RectTransform, TextMeshProUGUI, UIAnimator, CanvasGroup]
	|       +-- TipContent  [RectTransform, TextMeshProUGUI, UIAnimator, CanvasGroup]
```

**What this demonstrates:**

- The root `UIPage` owns page identity and lifecycle. Its `contentObj` reference points to the tagged content child; the page component toggles that child when displaying or hiding the page.
- The `UIAnimHandler` is colocated with the root page. `UIAnimatorGroup` is a child within the content hierarchy and registers itself with the handler at runtime, coordinating the animators assigned to the group.
- `UIAnimator` components make visual elements participate in open/close animation completion. `UIAnimatorLooper` is used for the two rotating loading-circle layers, whose looping animations are not awaited as regular animator completions.
- The root visual group contains a black `Image` and a `CanvasGroup`, while the tips region is assembled from a line image and separate header/body TMP text objects. Individual visual objects can have their own animator and CanvasGroup where needed.
- The prefab root itself does not provide the service loading Canvas. When registered as a loading page, `UIPageHandler` reparents the page root under `UIService.LoadingUICanvas`.

This is a concrete example rather than a mandatory layout. A minimal non-animated page can contain just a root with `UIPage` and one content child; add `UIAnimHandler`, groups, animators, and visual subtrees only when the page needs them.

## Page IDs and page types

`UIPageSO.PageID` is composed as:

```text
prefix + delimiter + pageID + (suffix is empty ? "" : delimiter + suffix)
```

`UIPageType` values are `Page`, `NotifConfirmPopUp`, `Loading`, and `Debug`. The handler's special behavior is based on `Loading`: it adds those pages to the loading-page registry and reparents them to `LoadingUICanvas`.

`OpenPageType` values:

| Value | Navigation behavior |
| --- | --- |
| `ClosePreviousPage` | When opening this page, hide the previous stack page. When closing this page, display the previous stack page again. |
| `OverlayPage` | Leave the previous page visible beneath this page. |

The navigation type is read from the page being opened/closed. Opening a page disables the previous page's registered selectables; closing the top page re-enables the previous page's selectables. Re-enabling a page raises its `OnRefreshPage` event.

## Runtime page lifecycle

When `UIService.Awake` runs, it registers itself, resolves `ISceneServiceManageable`, constructs `UIPageHandler`, subscribes the handler to scene-service events, and instantiates loading prefabs. When a `UIPage.Awake` runs, it resolves both services and registers its instance with the handler.

On registration, the handler adds a page to `RegisteredPages`, marks it registered, briefly enables then disables its content, and logs the registration. A duplicate `PageID` destroys the duplicate page GameObject. Loading pages are additionally stored in `LoadingPages` and parented to the loading canvas. `OnDestroy` unregisters a page; destroying the service unsubscribes the handler from scene events.

The handler exposes:

| API | Behavior |
| --- | --- |
| `OpenPage(UIPageSO data)` | Looks up the registered page by ID, disables the current top page's selectables, optionally hides it according to the target page's `OpenPageType`, displays/enables the new page, and pushes it on the stack. If no page is registered yet, logs an error. If the stack is empty, returns without opening. |
| `CloseTopPage()` | Hides the top page, optionally redisplays the previous page based on the closing page's type, enables the previous page, and removes the top entry. It requires at least two stack entries; the current guard only rejects counts below one, so calling it with exactly one entry will index before the start of the list. |
| `PrepareUIPageSO(UIPageSO page)` | Stores a loading page to open once all current pages have closed during a scene transition. |
| `OpenLoadingPage(UIPageSO page)` | Looks up the page in `LoadingPages` and displays it. The supplied asset must identify a registered loading page. |
| `CloseLoadingPage()` | Hides the current loading page and clears the reference. It assumes a loading page is currently open. |
| `SceneDefaultPage` | Returns the first entry of the opened-page list; it assumes that list is non-empty. |
| `RegisteredPages`, `LoadingPages` | Read-only views of the corresponding runtime registries. |

The initial scene default page is inserted into the opened-page stack when `OnSceneCompleteLoad` fires. `OnSceneCompleteLoadPageDone` then displays it. The stack's first entry is retained as the scene default while later entries are pushed/popped as navigation occurs.

`UIPage` sets `IsOpen` when displayed/hidden, toggles its content, and optionally starts open/close animations. Selectables register with `RegisterSelectable` and should unregister when their lifetime ends. `SetInteractable(true)` invokes `OnRefreshPage` after updating registered selectables; it does not itself alter the page's visibility.

## Animation pipeline

The completion chain is:

```text
UIPage -> UIAnimHandler -> UIAnimatorGroup -> UIAnimator -> UIAnimationGroup -> UIAnimationBase
```

`UIAnimHandler` waits for every configured group. Each `UIAnimatorGroup` waits for its regular animators and starts loopers on open; loopers do not count toward group completion. Each `UIAnimator` instantiates its configured `UIAnimationGroup` assets. Those groups instantiate their `UIAnimationBase` assets and report completion after all animations included in the current open/close count finish.

### `UIAnimationBase` reference

[`UIAnimationBase`](ScriptableObjects/UIAnimationBase.cs) is the abstract ScriptableObject base for the concrete animation assets. It does not create a tween on its own. `UIAnimationGroup` instantiates each configured asset at runtime and calls `Initialize` with its owning group, animator, target `RectTransform`, page handler, and looper status. A subclass implements `OpenAnimation` and `CloseAnimation`; these typically choose whether to wait for another animation and then run `OpenAnimationLogic` or `CloseAnimationLogic`.

When a concrete animation finishes, it calls `SetAnimationDone()`. This publishes an `AnimationDoneData` through `UIPageHandler.OnDonePlaying`, then raises the asset instance's `OnAnimationDone` event. The containing `UIAnimationGroup` counts that event to determine group completion. Thus completion data serves both as a sequencing signal for dependent animations and as a completion signal for the animator/page pipeline.

#### Serialized asset settings

| Field | Meaning in this codebase |
| --- | --- |
| `animationID` | Logical identifier sent in `AnimationDoneData` and used to match dependencies. The editor helper `SetAnimationID()` and `AutoSetReferenceSettings()` can set it from the asset name. IDs need to match exactly for a dependency to release. |
| `duration` | Open tween duration in seconds for the built-in fade, move, rotate, and scale assets. |
| `durationExit` | Close tween duration in seconds for those built-in assets. |
| `delay` | Delay before the open tween starts, unless the animation waits for another open animation; in that case built-in subclasses use zero delay after the dependency is met. |
| `delayExit` | Delay before the close tween starts, unless waiting for another close animation; then built-in subclasses use zero delay after the dependency is met. |
| `leanTweenType` | LeanTween easing curve used by built-in open tweens. |
| `leanTweenTypeExit` | LeanTween easing curve used by built-in close tweens. |
| `ignoreTimeScale` | Passed to the built-in tween's `setIgnoreTimeScale`. When true, the tween proceeds while `Time.timeScale` is paused or changed. |
| `repeatType` | Repeat mode used by built-in animations when the owning `UIAnimator` is a `UIAnimatorLooper`: `Repeat` loops from end to start, `PingPong` alternates direction, and `None` does not repeat. Looper animations do not send their ordinary completion callback. |
| `waitForOpenAnimationToFinish` | Optional animation asset whose open completion must be observed before this asset runs its open logic. The assigned asset supplies the expected `animationID`; it is not invoked by this field. |
| `waitForCloseAnimationToFinish` | Equivalent dependency for close logic. |
| `checkAnimatorGroupID` | If enabled, a dependency event must also have the same animator-group ID as this animation's initialized owner. |
| `checkAnimatorID` | If enabled, a dependency event must also have the same animator ID as this animation's initialized owner. |
| `checkAnimationGroupID` | If enabled, a dependency event must also have the same animation-group ID as this animation's initialized owner. |
| `signalsSceneTransitionCloseAnimationDone` | Enable on a loading page's close animation to set `ISceneServiceManageable.SetCloseAnimationDoneFlag(true)` and release the scene-transition close-animation gate. This is separate from `LoadingPageDone(UIAnimType.Close)`; both signals are required. It has no effect on open completions. |
| `excludeOpenAnimationFromCount` | The open animation still runs and can emit completion data, but `UIAnimationGroup` does not count its `OnAnimationDone` event toward open group completion. |
| `excludeCloseAnimationFromCount` | Same exclusion behavior for close group completion. |

#### Runtime and protected members

| Member | Role |
| --- | --- |
| `uIAnimationGroup` | The runtime group that instantiated this asset; used to identify the animation-group name. |
| `uIAnimator` | The runtime animator that owns this animation; exposes identity and the tracked LeanTween IDs. |
| `rectTransform` | UI transform animated by movement, rotation, and scale assets. |
| `uIPageHandler` | Publishes completion events, releases dependent animations, and forwards the close-animation flag to the scene service. |
| `canvasGroup` | Optional canvas group used by `FadeAnimation`; only that built-in passes/uses it. |
| `uIAnimDoneType` | Current direction (`Open` or `Close`); used to filter dependency events and label completion data. |
| `isLooper` | Set from the owning `UIAnimator`. Built-in subclasses configure repeats only for loopers and do not signal normal completion for looping open tweens. |
| `repeatValue`, `pingPongValue` | Protected counters available to subclasses for repeat logic. The base resets both when stopping; built-in subclasses shown here delegate repetition to LeanTween and do not otherwise use these counters. |
| `animatorGroupID`, `animatorID`, `animationGroupID` | Identity captured by `Initialize`; included in completion data and optionally checked when matching a dependency. The animation-group ID is based on the `UIAnimationGroup` asset name. |
| `MoveTweenID`, `ScaleTweenID`, `RotateTweenID`, `FadeTweenID` | Proxies to the owning animator's tracked tween IDs. Base `StopAnimation()` cancels each non-`-1` ID and resets it. These IDs are stored on the animator, so stopping an animation can cancel the currently tracked tween of that property on that animator. |
| `WaitForOpenAnimationToFinish`, `WaitForCloseAnimationToFinish` | Read-only indicators that the corresponding wait asset is assigned. |
| `ExcludeOpenAnimationFromCount`, `ExcludeCloseAnimationFromCount` | Read-only accessors consumed by `UIAnimationGroup` when aggregating completion. |
| `AnimationID`, `UIAnimDoneType` | Read-only identity/current-direction properties. |
| `OnAnimationDone` | Raised after `SetAnimationDone` has sent the handler event. `UIAnimationGroup` subscribes to this event for its completion count. |

`Initialize(...)` sets all runtime references and snapshots the three IDs from the owner. `OpenAnimation()` and `CloseAnimation()` are abstract entry points; subclasses set `uIAnimDoneType` before starting work. `OpenAnimationLogic()` and `CloseAnimationLogic()` are abstract tween/action implementations. `StopAnimation()` cancels tracked tweens and resets repeat counters. `HandleWaitAnimationDone(...)` ignores events from the wrong direction, checks the configured animation ID and any enabled owner-ID filters, unsubscribes once matched, then starts the relevant logic. `SetAnimationDone()` is the common completion path. `SetAnimationID()` and `AutoSetReferenceSettings()` are editor conveniences, not required runtime initialization.

Dependency matching uses the dependent animation's own owner IDs and the assigned wait asset's `animationID`. With the three `check...ID` options disabled (the default), any completion of the correct direction and matching animation ID can release the dependency, regardless of which animator/group emitted it. Enable the checks when IDs are reused across owners. A wait must refer to an animation that actually publishes completion; a looping open animation does not, and an animation excluded from group counting can still publish its event if it completes normally.

One implementation detail: `FadeAnimation.OpenAnimation()` returns immediately if no `CanvasGroup` was provided, without publishing completion. Such an asset cannot release another animation waiting on its open completion and is not suitable for a completion-counted fade unless the reference is fixed.

The built-in assets animate as follows:

| Asset | Open behavior | Close behavior |
| --- | --- | --- |
| `FadeAnimation` | Fades a `CanvasGroup` from alpha 0 to the configured target. | Fades to alpha 0 or the configured exit alpha. |
| `MoveAnimationOffset` | Starts at the captured position plus offset, then moves to the captured position. | Moves to the initial offset or configured exit offset. |
| `RotateAnimation` | Rotates from configured starting rotation to target rotation. | Returns to the start rotation or configured exit rotation. |
| `ScaleAnimation` | Scales from configured starting scale to target scale. | Returns to the start scale or configured exit scale. |

The open/close exclusion flags omit an animation from that direction's completion count; the animation can still run. A looper's repeating open animation does not report ordinary completion. `StopAnimation` cancels the animator's tracked LeanTween IDs and removes dependency listeners in the built-in animation implementations.

## Scene loading handshake

For a single-scene asynchronous load, `GameSceneService` performs this sequence:

1. Emits `OnScenePreLoading`; the UI handler disables interaction on opened pages and hides them.
2. Waits for `SetAllPagesClosed()`. Closing page animations call `UIPageHandler.PingAnimationDone()`, which checks `IsOpen` for the stack and signals the scene service once every non-null page is closed. If a prepared loading page exists, the handler opens it after this close check.
3. Waits for the loading page's `UIAnimType.Open` completion via `LoadingPageDone`, then loads and activates the target scene.
4. Waits for frame time to stabilize, emits `OnFrameTimeStabilized`, and the handler closes the loading page.
5. Waits for the loading page's `UIAnimType.Close` completion, then emits `OnSceneCompleteLoad` and waits for the target scene default page to finish displaying before emitting `OnSceneCompleteLoadPageDone`.

For the close wait to finish, a close animation must report `LoadingPageDone(UIAnimType.Close)`. Separately, the scene service's `closeAnimationDoneFlag` is set only when a close animation completion has `signalsSceneTransitionCloseAnimationDone` enabled; configure this on the loading page's close animation asset. The settings option `DontSendLoadingPageDoneEvent` is effective only when both `debugMode` and `dontSendLoadingPageDoneEvent` are true, and suppresses the loading-page completion notifications/close-flag signal.

`PrepareUIPageSO` is not called by `UIPageHandler` itself. The caller initiating a transition must prepare the loading-page asset before the scene service starts loading if that loading page should be shown.

## Editor helpers

Several components expose editor-only `AutoSetReferenceSettings` helpers for assigning expected nearby references. `UIPage` can find a child tagged `UIContent`, add/find its `UIAnimHandler` when animation is enabled, and optionally rename the root/content objects from the page ID. `UIAnimHandler` and `UIAnimatorGroup` can locate their neighboring components; animator scripts and ID assets can populate IDs from their asset/object names. These are editor conveniences, not runtime validation.

## Current implementation caveats

- `CloseTopPage()` is safe only when at least two entries exist, despite its current `< 1` guard.
- `OpenLoadingPage()` and `CloseLoadingPage()` dereference the current loading page without validating lookup success or whether one is open. Pass a registered loading-page asset and respect the scene transition order.
- `SceneDefaultPage` assumes at least one entry. Do not read it before a scene default has been registered.
- Page and animation setup depends on valid serialized references. Missing references can fail at runtime rather than producing a configuration error.
- Scene completion depends on animation callbacks to report page/loading completion. In particular, a non-animated page's `HidePage()` deactivates content directly and does not call `PingAnimationDone()`; verify that transition configurations provide the expected completion signals.
- Additive scene loading is not implemented in `GameSceneService`; synchronous scene loading throws `NotImplementedException`.

## Related scene-service API

`ISceneServiceManageable` provides `LoadSceneAsync`, `LoadingPageDone`, `SetAllPagesClosed`, and `SetCloseAnimationDoneFlag`. The UI handler subscribes to `OnScenePreLoading`, `OnFrameTimeStabilized`, `OnSceneCompleteLoad`, and `OnSceneCompleteLoadPageDone`, and unsubscribes when destroyed. The scene service also declares loading-page and progress events, but the UI handler does not subscribe to those events directly.