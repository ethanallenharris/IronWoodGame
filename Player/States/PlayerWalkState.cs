using UnityEngine;
public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    private float largestAxis = 0f;

    private Vector3 initialScale;

    private float scaleFactor;

    private float playerRotationWeight;

    public override void EnterState() 
    {
        //initialScale = _currentContext.player.transform.localScale;

        // Reset the child's scale to uniform based on the largest axis of the initial scale
        //largestAxis = Mathf.Max(initialScale.x, initialScale.y, initialScale.z);

        // Adjust the other two axes to maintain the original proportions of the character
        //scaleFactor = largestAxis / Mathf.Max(initialScale.x, initialScale.y, initialScale.z);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        _currentContext.Movement();
        _currentContext.RotateCharacter();
        _currentContext.DetectDash();
        _currentContext.DetectStep();
        _currentContext.DetectAttack();


        Vector2 InputVector = _currentContext.input.InputVector;

        float targetRotation = 0f;

        if (InputVector.y > 0)
        {
            //_currentContext.animator.PlayAnimation("RunForward");
            //targetRotation = 0f;

            //if (InputVector.x < 0)
            //{
            //    targetRotation = -45f;
            //}
            //else if (InputVector.x > 0)
            //{
            //    targetRotation = 45f;
            //}
        }
        else if (InputVector.y < 0)
        {
            //_currentContext.animator.PlayAnimation("RunBack");
            targetRotation = 0;

            if (InputVector.x < 0)
            {
                targetRotation = 90f;
            }
            else if (InputVector.x > 0)
            {
                targetRotation = -90f;
            }
        }
        else if (InputVector.x > 0)
        {
            //_currentContext.animator.PlayAnimation("RunRight");
            targetRotation = 10f;
        }
        else if (InputVector.x < 0)
        {
            //_currentContext.animator.PlayAnimation("RunLeft");
            targetRotation = -10f;
        }


        // Smoothly rotate towards the target rotation
        RotatePlayer(targetRotation);
    }

    private void RotatePlayer(float targetRotation)
    {
        Quaternion playerRotation = Quaternion.Euler(-90f, _currentContext.playerController.transform.rotation.eulerAngles.y, _currentContext.playerController.transform.rotation.eulerAngles.z);

        Quaternion playerTargetAngle = Quaternion.Euler(0f, targetRotation, 0f) * playerRotation;

        _currentContext.player.transform.rotation = Quaternion.Lerp(_currentContext.player.transform.rotation, playerTargetAngle, _currentContext.PlayerRotationSpeed * Time.deltaTime);
    }



    public override void ExitState() { }



    public override void InitialiseSubState() { }

    public override void CheckSwitchState() {
        //if player is not inputting movement keys, switch to idle state
        if (_currentContext.input.InputVector.y == 0 && _currentContext.input.InputVector.x == 0)
        {
            Quaternion playerRotation = Quaternion.Euler(-90f, _currentContext.playerController.transform.rotation.eulerAngles.y, _currentContext.playerController.transform.rotation.eulerAngles.z);
            _currentContext.player.transform.rotation = Quaternion.Euler(0f, 0f, 0f) * playerRotation;//Reset player back to facing forwards
            SwitchState(_factory.Idle());
            _currentContext.currentState.EnterState();
        }

        if (_currentContext.input.InputVector.y == 1 && Input.GetKey("left shift"))
        {
            SwitchState(_factory.Run());
            _currentContext.currentState.EnterState();
        }
    }
}
