using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{

    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    private bool exitSwitch = false;

    public override void EnterState() 
    {
        //_currentContext.animator.Play("Walking2");
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        _currentContext.DetectDash();
        _currentContext.DetectStep();
        _currentContext.DetectAttack();
        _currentContext.ResetCharacterRotation();
        _currentContext.DetectInventory();

        if (Input.GetMouseButton(1)) 
        {
            //I want this to eventually slowly rotate towards the mouse point
            _currentContext.RotateCharacter();
        }

        ////Slowly rotate player forwards
        //Quaternion playerRotation = Quaternion.Euler(-90f, _currentContext.playerController.transform.rotation.eulerAngles.y, _currentContext.playerController.transform.rotation.eulerAngles.z);
        //_currentContext.player.transform.rotation = Quaternion.Lerp(_currentContext.player.transform.rotation, playerRotation, _currentContext.PlayerRotationSpeed * Time.deltaTime);
    }

    public override void ExitState() { }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() {

        //if player is inputting movement keys, switch to walk state
        if (_currentContext.input.InputVector.y != 0 | _currentContext.input.InputVector.x != 0)
        {
            SwitchState(_factory.Walk());
        }
    }
}