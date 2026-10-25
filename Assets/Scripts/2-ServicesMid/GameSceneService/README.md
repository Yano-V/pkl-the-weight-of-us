# GameSceneService

`GameSceneService` implements `ISceneServiceManageable` and coordinates scene changes with Unity's `SceneManager`, the game-state service, the data service, and the UI page system. The implemented transition path is asynchronous single-scene loading. It emits lifecycle events, waits for current UI pages and the loading page animations, gates scene activation on frame-time stabilization, and then signals completion.

## Related scripts

| Script | Responsibility |
| --- | --- |
| [`GameSceneService.cs`](GameSceneService.cs) | Registers the scene-service interfaces, tracks current/loading scene data, emits events, and runs the single-scene load coroutine. |
| [`ISceneService.cs`](../../0-Core/Services/MonoServices/SceneService/ISceneService.cs) | Read-only scene-service event/property contract and manageable load/completion methods. |
| [`LoadState.cs`](../../0-Core/Services/MonoServices/SceneService/Enums/LoadState.cs) | Load lifecycle states: `Done`, `PreLoading`, `Loading`, and `PostLoading`. |
| [`SceneData.cs`](ScriptableObjects/SceneData.cs) | ScriptableObject mapping a Unity scene name to a default UI page and default main-game state. |
| [`SceneServiceSettingsSO.cs`](ScriptableObjects/Settings/SceneServiceSettingsSO.cs) | Configures the maximum wait for stable frame time. |
| [`WaitUntilLoadingPageDone.cs`](CustomYieldInstructions/WaitUntilLoadingPageDone.cs) | Yield instruction that waits until the reported UI animation direction matches the expected direction. |
| [`WaitUntilFrameTimeStabilize.cs`](CustomYieldInstructions/WaitUntilFrameTimeStabilize.cs) | Yield instruction that waits for ten stable frame-time samples or until the configured timeout elapses. |
| [`IDataService.cs`](../../1-ServicesLow/GameDataService/IDataService.cs) | Contract used to resolve scene metadata by Unity scene name. |
| [`GameDataService.cs`](../../1-ServicesLow/GameDataService/GameDataService.cs) | Loads labeled `SceneData` assets and returns the asset whose `SceneName` exactly matches a name. |
| [`UIPageHandler.cs`](../../3-ServicesHigh/UIService/UIPageHandler.cs) | UI integration for closing pages, managing the loading page, and showing the target scene's default page. |
| [`UIPage.cs`](../../3-ServicesHigh/UIService/UIPage.cs) | Reports loading-page open/close animation completion to the manageable scene service. |

## Setup and dependencies

1. Add `GameSceneService` to the persistent service/bootstrap scene. It registers both `ISceneService` and `ISceneServiceManageable` if no `ISceneService` is already registered.
2. Assign a `SceneServiceSettingsSO` asset. Its `Frame Time Stable Check Timeout` controls how long post-load frame stabilization waits before continuing.
3. Configure `GameDataService` with the Addressables label for scene metadata. Each scene that should be resolved needs a `SceneData` asset under that label.
4. In each `SceneData`, set `Scene Name` to exactly the Unity scene name, and assign its scene default page if the UI flow requires one. The asset also holds a `Default Main Game State`; this service exposes the scene metadata but does not itself apply that state.
5. Ensure `GameDataService` and `GameSceneService` can resolve their service dependencies from `GameServices` during `Awake`. `GameSceneService` requires `IDataService` and `IGameServiceManageable`; the frame-time wait also requires the game service's performance tracker and load state.
6. For transitions with UI, ensure `UIService` is registered and scene pages are configured. A caller should prepare the loading-page asset through `UIPageHandler.PrepareUIPageSO(...)` before requesting a load if a loading page is to be shown. The loading page reports completion through `UIPage.UIAnimatorFinished`.

`GameDataService` loads the scene data assets through Addressables and waits for that load to complete during its own `Awake`. `GetSceneDataByName` uses an exact string comparison and logs/returns null if a matching asset is not present.

## Runtime state

`GameSceneService` stores three scene-data references:

| Field | Runtime role |
| --- | --- |
| `previousSceneData` | Serialized current/previous-scene slot; it is not read or written elsewhere in this implementation. |
| `currentSceneData` | Scene metadata currently considered loaded. Updated after the transition waits complete, or during initial scene discovery. |
| `loadingSceneData` | Target metadata while the single-scene coroutine is in progress. Cleared after close-animation gating and before final completion events. |

The other important internal state is:

| Field | Role |
| --- | --- |
| `loadCoroutine` | Current single-scene coroutine. A new single-scene request stops the prior coroutine before starting another. |
| `allPagesClosed` | Set by `SetAllPagesClosed`; gates progress past the preloading UI close phase. Reset at the start of a new transition. |
| `closeAnimationDoneFlag` | Set by `SetCloseAnimationDoneFlag`; gates progress after the `OnFrameTimeStabilized` event. Reset at the start of a new transition. |
| `waitUntilLoadingPageDone` | Shared open/close completion waiter. Each phase resets its expected `UIAnimType` and current value. |
| `waitUntilFrameTimeStabilize` | Shared frame-time waiter updated from `IGameServiceManageable.PerformanceTracker.AverageFrameTime` in `Update`. |

## Public API

`ISceneService` exposes `SceneServiceSettingsSO` and the lifecycle events below. `ISceneServiceManageable` adds:

| Method | Behavior |
| --- | --- |
| `LoadSceneAsync(SceneData sceneToLoad, LoadSceneMode loadSceneMode, bool isAsync = true)` | Starts the asynchronous single-scene coroutine for `LoadSceneMode.Single`. Additive mode currently does nothing. `isAsync: false` throws `NotImplementedException`. |
| `LoadingPageDone(UIAnimType type)` | Updates the loading-page waiter's current value. The coroutine waits for `Open` before loading and `Close` after frame stabilization. |
| `SetAllPagesClosed()` | Releases the wait for all current pages to close during preloading. |
| `SetCloseAnimationDoneFlag(bool value)` | Changes the close-animation gate used after `OnFrameTimeStabilized`. UI animation completion currently sets it to true when configured to do so. |

`LoadSceneAsync` does not validate a null `SceneData`. If the target scene is already loaded according to `SceneData.IsSceneLoaded`, the coroutine logs a warning, clears its coroutine reference, and exits without changing the scene.

## Lifecycle events

All scene lifecycle events carry the relevant `SceneData` instance. `OnUpdateLoadProgress` carries Unity's asynchronous operation progress value.

| Event | When it is emitted by the normal single-scene flow |
| --- | --- |
| `OnScenePreLoading` | Before waiting for UI pages to close. Sets game load state to `PreLoading`. |
| `OnSceneLoading` | After requesting Unity's asynchronous load and setting load state to `Loading`. |
| `OnUpdateLoadProgress` | While `AsyncOperation.progress < 0.9`; passes the progress value. It is not emitted after the threshold or after activation. |
| `OnScenePostLoading` | After Unity reports the operation done and load state changes to `PostLoading`. |
| `OnFrameTimeStabilized` | After the frame-time wait (including its timeout fallback) completes. The UI handler closes the loading page in response. |
| `OnSceneCompleteLoad` | After the close-animation flag is set and the target is made `currentSceneData`; game load state changes to `Done` first. |
| `OnSceneCompleteLoadPageDone` | After the coroutine receives the loading waiter's close completion following `OnSceneCompleteLoad`. The UI handler displays the target scene's default page when it receives this event. |
| `OnOpenLoadingPage`, `OnCloseLoadingPage` | Declared in the interface and service, but not invoked by `GameSceneService` in the current implementation. |

`UIPageHandler` subscribes to `OnScenePreLoading`, `OnFrameTimeStabilized`, `OnSceneCompleteLoad`, and `OnSceneCompleteLoadPageDone`. It hides/interacts with pages during preloading, closes the loading page when frame time is stabilized, installs the scene default page in its stack at scene completion, and displays that page on `OnSceneCompleteLoadPageDone`.

## Single-scene asynchronous flow

`LoadSceneSingleAsync` stops any previous scene-load coroutine and starts `LoadSceneSingleAsyncCor`:

1. **Validate target:** If `SceneData.IsSceneLoaded` is true, log a warning and exit.
2. **Prepare preloading:** Reset both boolean gates, initialize the frame-time waiter from the current average frame time, assign `loadingSceneData`, change game load state to `PreLoading`, emit `OnScenePreLoading`, and set the loading-page waiter to expect `Open`.
3. **Close current UI:** Wait frame-by-frame until a consumer calls `SetAllPagesClosed()`. Then wait until `LoadingPageDone(Open)` has been reported. The UI page handler uses page close completion to set the all-pages-closed gate and can open a page previously passed to `PrepareUIPageSO`.
4. **Load without immediate activation:** Call `SceneManager.LoadSceneAsync`, change state to `Loading`, emit `OnSceneLoading`, and set `allowSceneActivation = false`. While progress is below `0.9`, emit `OnUpdateLoadProgress` each loop. At `0.9`, allow activation and wait for `isDone`.
5. **Post-load stabilization:** Change state to `PostLoading`, emit `OnScenePostLoading`, reset the stabilization timeout, and yield until frame time is stable for ten consecutive samples or the configured timeout expires. Frame samples and timeout are updated in `Update`; timeout time accumulates only while load state is `PostLoading`.
6. **Close loading UI:** Set the loading-page waiter to expect `Close`, emit `OnFrameTimeStabilized`, then wait for `closeAnimationDoneFlag` to become true.
7. **Complete scene:** Promote `loadingSceneData` to `currentSceneData`, clear the loading reference, change game load state to `Done`, emit `OnSceneCompleteLoad`, and wait for `LoadingPageDone(Close)`. Finally emit `OnSceneCompleteLoadPageDone` and clear `loadCoroutine`.

The two UI completion signals are separate: `LoadingPageDone` satisfies the direction-specific yield instruction, while `SetCloseAnimationDoneFlag(true)` satisfies the boolean gate. The current UI integration calls `LoadingPageDone` when a loading page's animation finishes. A close animation must also have `sendCloseAnimationDoneFlag` enabled so `UIPageHandler` sends the separate close flag. A transition can otherwise remain waiting indefinitely at either gate.

## Frame-time stabilization

[`WaitUntilFrameTimeStabilize`](CustomYieldInstructions/WaitUntilFrameTimeStabilize.cs) snapshots the current average frame time and tracks subsequent samples. A sample is considered stable if its absolute difference from the cached average is no more than 10% of that average. Ten consecutive stable samples finish the wait. Invalid/non-positive frame times reset the stable-frame count; if the cached value is invalid, the first valid sample initializes it.

The timeout is a fallback: the wait ends when either ten consecutive stable samples are collected or `FrameTimeStableCheckTimeout` elapses. The timeout timer uses `Time.unscaledDeltaTime` and advances only while game load state is `PostLoading`. The default timeout in `SceneServiceSettingsSO` is 2 seconds.

## Initial scene bootstrap

On `Awake`, the service subscribes to `SceneManager.sceneLoaded`. The first scene-loaded callback received before `serviceInitialized` resolves a `SceneData` by the loaded Unity scene name. If metadata is found, it assigns `currentSceneData`, emits `OnScenePostLoading`, `OnSceneCompleteLoad`, and `OnSceneCompleteLoadPageDone` immediately, then marks the service initialized. This bootstrap path does not run the normal load coroutine, does not emit preloading/loading events, and does not wait for UI or frame-time stabilization.

If metadata is missing, `GameDataService` logs an error and returns null; the service does not set `serviceInitialized` in that case. Since the handler only processes the first scene-loaded callback while uninitialized, metadata for the boot scene should be available at startup.

## Yield instruction details

[`WaitUntilLoadingPageDone`](CustomYieldInstructions/WaitUntilLoadingPageDone.cs) stores `TypeToCompare` and `CurrentType`. `PrepareConditions` sets the expected direction and resets the current direction to `None`. Its `keepWaiting` remains true until they match. `GameSceneService.LoadingPageDone` only updates `CurrentType`; it does not verify which page or transition reported the value.

`WaitUntilFrameTimeStabilize` is prepared before preloading and reset after scene activation. Its timeout is tied to `LoadState.PostLoading`, so it does not time out the earlier page-close/open waits.

## Work in progress

The following features are incomplete or declared without runtime behavior in the current scripts:

- **Additive scene loading:** `LoadSceneMode.Additive` has an empty switch branch, and the private `LoadSceneAdditiveAsync` method has no implementation. Additive loads currently do nothing.
- **Synchronous scene loading:** `LoadSceneAsync(..., isAsync: false)` always throws `NotImplementedException`; the commented-out switch is only a placeholder.
- **Loading-page open/close events:** `OnOpenLoadingPage` and `OnCloseLoadingPage` are part of `ISceneService` and are declared by `GameSceneService`, but the service never invokes them. The current UI flow is connected through scene lifecycle events and direct calls to `UIPageHandler`.
- **Previous-scene tracking:** `previousSceneData` is serialized but is not assigned or read. The service does not currently preserve the outgoing scene metadata in this field.
- **Applying the scene's default game state:** `SceneData` exposes `DefaultMainGameState`, but `GameSceneService` does not apply it to the game-state service during bootstrap or transitions.

## Current implementation limitations and caveats

- Only asynchronous `LoadSceneMode.Single` is implemented. Additive mode is a no-op, and synchronous loading throws `NotImplementedException`.
- Calling a new single-scene load stops the previous coroutine without explicitly resetting its state or notifying event subscribers that it was cancelled. The new coroutine resets its two boolean gates and re-prepares wait conditions.
- Preloading waits have no timeout: missing calls to `SetAllPagesClosed()` or `LoadingPageDone(Open)` stall the transition.
- The post-stabilization close phase has two independent waits: the close-animation flag and the loading waiter's `Close` value. Both must be reported.
- Scene activation is held at Unity's `0.9` progress threshold until the loading-page open wait completes; the progress event only covers values below `0.9`.
- The initial bootstrap event path is not equivalent to a regular coroutine load and may notify subscribers before scene pages or other listeners have registered, depending on Unity service initialization order.
- `previousSceneData` is currently unused. `SceneData.DefaultMainGameState` is also not applied by `GameSceneService` itself.
- `OnOpenLoadingPage` and `OnCloseLoadingPage` are presently declarations only; use the UI handler's API and scene lifecycle events for current behavior.
- `SceneServiceSettingsSO` and service references are assumed to be assigned/available; null configuration is not guarded throughout the loading flow.

## Shutdown

When this instance registered the services, `OnDestroy` unregisters both interfaces and unsubscribes from `SceneManager.sceneLoaded`. If it did not register because another `ISceneService` already existed, it does not unregister that other service. The coroutine is not explicitly stopped in `OnDestroy` in this implementation.
