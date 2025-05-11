using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public PlayerStateMachine PlayerStateMachine;
    public InputHandler InputHandler;

    public float x_movement;
    public float y_movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //x_movement = InputHandler.InputVector.x;
        //y_movement = InputHandler.InputVector.y;
    }
}
