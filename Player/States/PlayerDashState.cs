using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{

    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    bool ExitStateSwitch = false;
    float rollDuration = 0;//seconds

    public override void EnterState() 
    {
        _currentContext.RotateCharacter();
        _currentContext.StartCoroutine(Roll());
    }

    public override void UpdateState()
    {
        ExitState();

        _currentContext.player.transform.rotation = _currentContext.transform.rotation * Quaternion.Euler(-90f, 0,0);
        _currentContext.transform.Translate(new Vector3(0, 0, 1) * 0.06f);
    }

    IEnumerator Roll()
    {
        _currentContext.isRolling = true;

        // Animation stuff
        _currentContext.animator.SetBool("Rolling", true);
        _currentContext.clientNetworkAnimator.Animator.SetBool("Rolling", true);

        _currentContext.animator.Play("Roll");
        _currentContext.clientNetworkAnimator.Animator.Play("Roll");

        float rollAnimationLength = GetAnimationClipLength(_currentContext.animator, "Roll");
        yield return new WaitForSeconds(rollAnimationLength/3);


        _currentContext.animator.SetBool("Rolling", false);
        _currentContext.clientNetworkAnimator.Animator.SetBool("Rolling", false);
        _currentContext.animator.SetFloat("Horizontal", 0);
        _currentContext.animator.SetFloat("Vertical", 0);
        _currentContext.clientNetworkAnimator.Animator.SetFloat("Horizontal", 0);
        _currentContext.clientNetworkAnimator.Animator.SetFloat("Vertical", 0);
        _currentContext.animator.Play("Walking");
        _currentContext.clientNetworkAnimator.Animator.Play("Walking");

        _currentContext.EnterState("idle");
        ExitStateSwitch = true;
    }

    float GetAnimationClipLength(Animator animator, string clipName)
    {
        float clipLength = 0f;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                clipLength = clip.length;
                break;
            }
        }

        return clipLength;
    }




    public override void ExitState() 
    {
        if (ExitStateSwitch)
        {
            if (Input.GetKey("left shift"))
            {
                _currentContext.currentSpeedMultiplier = 3f;
                SwitchState(_factory.Run());
                _currentContext.currentState.EnterState();
                return;
            }
                
            _currentContext.EnterState("idle");
        }
    }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() { }
}