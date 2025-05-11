using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepState : PlayerBaseState
{

    public PlayerStepState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    private string Direction;
    private float zValue;
    private float xValue;

    bool ExitStateSwitch = false;

    public override void EnterState() {
        //Cloud poof at feet and step sound
        Direction = _currentContext.playerDashDirection;

        switch (Direction)
        {
            case "Forwards":
                zValue = 1;
                xValue = 0;
                break;
            case "Right":
                zValue = 0;
                xValue = 1;
                break;
            case "Left":
                zValue = 0;
                xValue = -1;
                break;
            case "Backwards":
                zValue = -1;
                xValue = 0;
                break;
        }
        _currentContext.StartCoroutine(Step());
       

    }

    IEnumerator Step()
    {
        string Direction = _currentContext.playerDashDirection;

        // Animation stuff
        switch (_currentContext.playerDashDirection)
        {
            case "Forwards":
                _currentContext.animator.Play("StepForwards");
                _currentContext.clientNetworkAnimator.Animator.Play("StepForwards");
                break;
            case "Right":
                _currentContext.animator.Play("StepRight");
                _currentContext.clientNetworkAnimator.Animator.Play("StepRight");
                break;
            case "Left":
                _currentContext.animator.Play("StepLeft");
                _currentContext.clientNetworkAnimator.Animator.Play("StepLeft");
                break;
            case "Backwards":
                _currentContext.animator.Play("StepBackwards");
                _currentContext.clientNetworkAnimator.Animator.Play("StepBackwards");
                break;
        }


        // Wait for the length of the "Roll" animation
        //float rollAnimationLength = GetAnimationClipLength(_currentContext.animator, $"Step{Direction}");
        yield return new WaitForSeconds(0.2f);

        // Reset animation parameters and play the "Walking" animation
        _currentContext.animator.Play("Walking");
        _currentContext.clientNetworkAnimator.Animator.Play("Walking");

        // Perform state transition or other logic
        _currentContext.EnterState("idle");
        ExitStateSwitch = true;
    }

    public override void UpdateState() {
        _currentContext.player.transform.rotation = _currentContext.transform.rotation * Quaternion.Euler(-90f, 0, 0);
        switch (_currentContext.playerDashDirection)
        {
            case "Forwards":
                zValue = 1;
                xValue = 0;
                break;
            case "Right":
                zValue = 0;
                xValue = 1;
                break;
            case "Left":
                zValue = 0;
                xValue = -1;
                break;
            case "Backwards":
                zValue = -1;
                xValue = 0;
                break;
        }
        _currentContext.transform.Translate(new Vector3(xValue, 0, zValue) * 0.08f);

        //_currentContext.transform.Translate(new Vector3(0, 0, 1) * 0.06f);
    }

    public override void ExitState() 
    {
        if (ExitStateSwitch)
        {
            //if holding down go to run and keep player movement speed mult
            _currentContext.EnterState("idle");
        }
    }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() { }
}
