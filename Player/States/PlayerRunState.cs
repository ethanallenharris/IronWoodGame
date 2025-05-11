using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{

    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public float speedMultiplier = 3.0f;
    public override void EnterState() 
    {
        _currentContext.StartCoroutine(IncreaseSpeedCoroutine());
        _currentContext.animator.SetFloat("Sprint", 0);
    }

    public override void UpdateState()
    {
        ExitState();
        CheckSwitchState();
        _currentContext.RotateCharacter();

        //Slowly rotate player forwards
        Quaternion playerRotation = Quaternion.Euler(-90f, _currentContext.playerController.transform.rotation.eulerAngles.y, _currentContext.playerController.transform.rotation.eulerAngles.z);
        _currentContext.player.transform.rotation = Quaternion.Lerp(_currentContext.player.transform.rotation, playerRotation, _currentContext.PlayerRotationSpeed * Time.deltaTime);

        float playerSpeed = _currentContext.playerMovespeed * (_currentContext.currentSpeedMultiplier * 0.75f);
        float RunMultFloat = (_currentContext.currentSpeedMultiplier - 1) / 2f;
        _currentContext.animator.SetFloat("Sprint", RunMultFloat);

        _currentContext.playerController.transform.Translate(new Vector3(0, 0, _currentContext.input.InputVector.y) * playerSpeed);
    }

    private IEnumerator IncreaseSpeedCoroutine()
    {
        while (_currentContext.currentSpeedMultiplier < speedMultiplier)
        {
            if (!Input.GetKey("left shift"))
            {
                _currentContext.currentSpeedMultiplier = 1;
                yield break;
            }
            _currentContext.currentSpeedMultiplier += Time.deltaTime;
            yield return null;
        }
        _currentContext.currentSpeedMultiplier = speedMultiplier;
    }

    public override void ExitState() { }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() 
    {
        if (_currentContext.input.InputVector.y == 0 || !Input.GetKey("left shift"))
        {
            //_currentContext.animator.PlayAnimation("Empty");
            _currentContext.currentSpeedMultiplier = 1f;
            SwitchState(_factory.Walk());
            _currentContext.currentState.EnterState();
        }


        //For now just reset back to walking
        if (_currentContext.DetectDash())
        {
            _currentContext.currentSpeedMultiplier = 1f;
        }

        if (
             _currentContext.DetectStep() ||
             _currentContext.DetectAttack()
            )
        {
            _currentContext.currentSpeedMultiplier = 1f;
        }
    }
}