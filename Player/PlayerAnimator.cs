using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{


    Animator animator;
    private string currentState;
    private bool isClient;
    private PlayerStateMachine playerStateMachine;


    void Start()
    {
        FetchAnimator();
        //base.OnStartClient();

        //    if (base.IsOwner)
        //    {
        //        CmdUpdateAnimationState(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        //    }
        //    else
        //    {

        //    }
    }

    public override void OnNetworkSpawn()
    {
        FetchAnimator();
        FetchStateMachine();
    }

    public void FetchAnimator()
    {
        animator = GetComponent<Animator>();
    }

    public void FetchStateMachine()
    {
        playerStateMachine = gameObject.GetComponentInParent<PlayerStateMachine>();
    }

    public void PlayAnimation(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;


    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                return ac.animationClips[i].length;
            }
        }
        Debug.LogWarning("Animation '" + animationName + "' not found!");
        return 0f;
    }

    public void EnterIdle()
    {
        playerStateMachine.currentState = playerStateMachine.states.Idle();
        playerStateMachine.currentState.EnterState();
    }

    public void Attack(GameObject Attack)
    {
        playerStateMachine.SpawnWeaponAttack(Attack);
        playerStateMachine.ResetCharacterRotation();
        playerStateMachine.animator.SetFloat("Horizontal", 0);
        playerStateMachine.animator.SetFloat("Vertical", 0);
        playerStateMachine.clientNetworkAnimator.Animator.SetFloat("Horizontal", 0);
        playerStateMachine.clientNetworkAnimator.Animator.SetFloat("Vertical", 0);
    }

    private void MoveDirection(Direction direction, float velocity, float duration)
    {
        if (!IsOwner) return;

        Vector3 velocityVector = new Vector3();

        switch (direction)
        {
            case Direction.Forwards:
                velocityVector = new Vector3(0, 0, velocity);
                break;
            case Direction.Right:
                velocityVector = new Vector3(velocity, 0, 0);
                break;
            case Direction.Left:
                velocityVector = new Vector3(-velocity, 0, 0);
                break;
            case Direction.Back:
                velocityVector = new Vector3(0, 0, -velocity);
                break;
        }

        playerStateMachine.StartCoroutine(playerStateMachine.Move(velocityVector, duration));
    }

    public void MoveForwadsSmall()
    {
        MoveDirection(Direction.Forwards, 0.8f, 0.08f); // Adjust parameters as needed
    }


    //private void Update()
    //{

    //}

    //[Command]
    //private void CmdUpdateAnimationState(int stateHash)
    //{
    //    syncState = Animator.StringToHash(Animator.GetAnimatorTransitionInfo(0).ToString()).ToString();
    //}

    //private void OnAnimationStateChanged(string newState)
    //{
    //    if (currentState == newState) return;

    //    animator.Play(newState);

    //    currentState = newState;
    //}

    //private void OnValidate()
    //{
    //    syncState = Animator.StringToHash(Animator.GetAnimatorTransitionInfo(0, 0).ToString()).ToString();
    //}

}
