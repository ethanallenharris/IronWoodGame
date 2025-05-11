using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryState : PlayerBaseState
{

    public PlayerInventoryState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    private bool exitSwitch = false;

    public override void EnterState() { }

    public override void UpdateState()
    {
        ExitState();

        if (_currentContext.InventoryCooldown > 0)
            _currentContext.InventoryCooldown -= Time.deltaTime;
    }

    public override void ExitState() 
    {
        _currentContext.DetectInventory();
    }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() { }
}