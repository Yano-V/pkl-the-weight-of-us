using System;
using System.Collections.Generic;
using UnityEngine;

// ================================================================================= //
//                                                                                   //
// Class: GameService                                                                //
// Description: GameService tracks the current states of the game, including         //
//              current states of the game. These includes where the player is in    //
//              the game, the pause state, the combat state, etc.                    //
//                                                                                   //
// Purpose: GameService is the sole service to tell other scripts what the           //
//          current states of the game by exposing enums.                            //
//                                                                                   //
// ================================================================================= //

public class GameService : 
    MonoService, 
    IGameServiceManageable, 
    IGameServiceFiniteManageable
{
    private readonly Dictionary<EnumMainGameState, FiniteMainGameState> mainGameStates = new();
    private FiniteMainGameState currentMainGameState;

    [SerializeField] EnumCombatState combatState;
    [SerializeField] EnumMainGameState mainGameState;
    [SerializeField] LoadState loadState;

    [SerializeField] GamePerformanceTracker performanceTracker;
    [SerializeField] int targetFrameRate = 60;

    #region Interface Implementation Events
    public event Action<EnumMainGameState> OnEnterMainGameState;
    public event Action<EnumMainGameState> OnExitMainGameState;
    public event Action<EnumCombatState> OnEnterCombatState;
    public event Action<EnumCombatState> OnExitCombatState;
    public event Action<LoadState> OnEnterLoadState;
    public event Action<LoadState> OnExitLoadState;
    #endregion

    #region Interface Implementation Properties
    public EnumCombatState CombatState { get => combatState; set => combatState = value; }
    public EnumMainGameState MainGameState => mainGameState;
    public LoadState LoadState => loadState;

    public FiniteMainGameState CurrentFiniteMainGameState => currentMainGameState;

    public GamePerformanceTracker PerformanceTracker => performanceTracker;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (!GameServices.IsServiceRegistered<IGameService>())
        {
            GameServices.Register<IGameService>(this);
            GameServices.Register<IGameServiceManageable>(this);
            GameServices.Register<IGameServiceFiniteManageable>(this);
            serviceRegistered = true;
        }
        else
        {
            Destroy(this);
            return;
        }

        performanceTracker = new(this);

        RegisterState(EnumMainGameState.Bootstrap, new BootstrapMainGameState(this));
        RegisterState(EnumMainGameState.Intro, new IntroMainGameState(this));
        RegisterState(EnumMainGameState.Title, new TitleMainGameState(this));
        RegisterState(EnumMainGameState.MainMenu, new MainMenuMainGameState(this));
        RegisterState(EnumMainGameState.InGame, new InGameMainGameState(this));
    }

    private void Update()
    {
        performanceTracker?.Update(Time.unscaledDeltaTime);
        currentMainGameState?.Update();
    }

    private void OnDestroy()
    {
        if (serviceRegistered)
        {
            GameServices.Unregister<IGameService>();
            GameServices.Unregister<IGameServiceManageable>();
            GameServices.Unregister<IGameServiceFiniteManageable>();
            serviceRegistered = false;

            ClearStates();
        }
    }
    #endregion

    public void ChangeMainGameState(EnumMainGameState enumState)
    {
        if (mainGameStates.TryGetValue(enumState, out var state))
        {            
            currentMainGameState?.Exit();
            currentMainGameState = state;
            currentMainGameState.Enter();
        }
    }

    #region Interface Implementation Methods
    public void EnterMainGameStateEnum(EnumMainGameState state)
    {
        if (state == mainGameState) return;

        mainGameState = state;
        OnEnterMainGameState?.Invoke(mainGameState);
    }

    public void ExitMainGameStateEnum() => OnExitMainGameState?.Invoke(mainGameState);

    public void EnterCombatStateEnum(EnumCombatState state)
    {
        if (state == combatState) return;

        combatState = state;
        OnEnterCombatState?.Invoke(combatState);
    }

    public void ExitCombatStateEnum() => OnExitCombatState?.Invoke(combatState);

    public void ChangeLoadState(LoadState state)
    {
        if (state == loadState) return;

        OnExitLoadState?.Invoke(loadState);
        loadState = state;
        OnEnterLoadState?.Invoke(loadState);
    }
    #endregion

    #region Private Methods
    void RegisterState(EnumMainGameState state, FiniteMainGameState finiteState)
    {
        if (!mainGameStates.ContainsKey(state))
        {
            mainGameStates.Add(state, finiteState);
        }
    }

    void ClearStates() => mainGameStates.Clear();
    #endregion 

#if UNITY_EDITOR
    public void SetTargetFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
    }
#endif
}