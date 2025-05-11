public class PlayerStateFactory
{

    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() 
    {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Walk() {
        return new PlayerWalkState(_context, this);
    }

    public PlayerBaseState Inventory()
    {
        return new PlayerInventoryState(_context, this);
    }

    public PlayerBaseState Dash()
    {
        return new PlayerDashState(_context, this);
    }

    public PlayerBaseState Run()
    {
        return new PlayerRunState(_context, this);
    }

    public PlayerStepState Step()
    {
        return new PlayerStepState(_context, this);
    }

    public PlayerAttackState Attack()
    {
        return new PlayerAttackState(_context, this);
    }

}
