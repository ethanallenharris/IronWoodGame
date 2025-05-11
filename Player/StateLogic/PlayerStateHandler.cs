using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{

    public void changePlayerState(string newState)
    {
        switch (newState)
        {
            case "Normal":
                gameObject.GetComponent<InputHandler>().enabled = true;
                //gameObject.GetComponent<PlayerMovement>().enabled = true;
                // enable camera traking player
                break;
            case "Disabled":
                //gameObject.GetComponent<InputHandler>().enabled = false;
                //gameObject.GetComponent<PlayerMovement>().enabled = false;
                break;
            case "enter Inventory":

                break;
            case "Stunned":
                Debug.Log("stunned player");
                break;
            case "Dashing":
                Debug.Log("dash");
                //dash and disable player collision and damage during dash
                break;
        }
    }
}
