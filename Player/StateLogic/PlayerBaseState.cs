public abstract class PlayerBaseState
{

    protected PlayerStateMachine _currentContext;
    protected PlayerStateFactory _factory;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _currentContext = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    public abstract void InitialiseSubState();

    void UpdateStates() { }

    protected void SwitchState(PlayerBaseState newState) 
    {
        //current state exits state
        ExitState();

        //new state enters new state
        newState.EnterState();

        //switch current state of context
        _currentContext.currentState = newState;

    }

    protected void SetSuperSate() { }

    protected void SetSubState() { }
}
