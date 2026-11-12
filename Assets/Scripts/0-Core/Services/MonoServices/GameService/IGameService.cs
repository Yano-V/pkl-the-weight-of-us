using System;

public interface IGameService : IBaseService
{
    event Action<EnumMainGameState> OnEnterMainGameState;
    event Action<EnumMainGameState> OnExitMainGameState;
    event Action<EnumCombatState> OnEnterCombatState;
    event Action<EnumCombatState> OnExitCombatState;
    event Action<LoadState> OnEnterLoadState;
    event Action<LoadState> OnExitLoadState;

    EnumCombatState CombatState { get; }
    EnumMainGameState MainGameState { get; }
    LoadState LoadState { get; }

    FiniteMainGameState CurrentFiniteMainGameState { get; }

    GamePerformanceTracker PerformanceTracker { get; }
}

public interface IGameServiceManageable : IGameService
{
    void ChangeMainGameState(EnumMainGameState enumState);

    void ChangeLoadState(LoadState state);
}

public interface IGameServiceFiniteManageable : IGameService
{
    void EnterMainGameStateEnum(EnumMainGameState state);
    void ExitMainGameStateEnum();

    void EnterCombatStateEnum(EnumCombatState state);
    void ExitCombatStateEnum();
}