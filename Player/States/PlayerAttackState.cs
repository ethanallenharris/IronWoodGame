using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() 
    {
        _currentContext.animator.Play("LeftSlash");
        _currentContext.clientNetworkAnimator.Animator.Play("LeftSlash");
        _currentContext.StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        float rollAnimationLength = GetAnimationClipLength(_currentContext.animator, "LeftSlash");
        yield return new WaitForSeconds(rollAnimationLength);

        // Reset animation
        _currentContext.animator.Play("Walking");
        _currentContext.clientNetworkAnimator.Animator.Play("Walking");

        _currentContext.EnterState("idle");
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

    public override void UpdateState() {
        ExitState();
    }

    public override void ExitState() { }

    public override void InitialiseSubState() { }

    public override void CheckSwitchState() { }
}
