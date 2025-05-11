using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackHitbox : NetworkBehaviour
{
    //lifetime
    public float lifetime = 0.5f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            Destroy(gameObject); //destroy self
        }
    }
}
