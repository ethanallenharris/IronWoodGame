using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{

    public Vector2 InputVector { get; private set; }

    public Vector3 Mouseposition { get; private set; }

    //public GameManager gameManager;

    public bool _inInventory;

    private float _inventoryNavTimerTime;

    private float _inventoryNavTimer;

    private void Start()
    {
        //_inventoryNavTimerTime = 
       // inventoryNavTimer = inventoryNavTimerTime;
        //inInventory = false;
    }



    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        InputVector = new Vector2(h, v);

        Mouseposition = Input.mousePosition;

    }

}
