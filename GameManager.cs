using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<GameObject> players;

    public override void OnNetworkSpawn()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void PlayerCountUpdateServerRpc() 
    {
        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
        players = playersArray.ToList();
    }
}
