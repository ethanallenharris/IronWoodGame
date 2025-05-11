using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour, INetworkSerializable
{
    public float Stamina = 0;
    public GameManager gameManager;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        var gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObject != null)
            gameManager = gameManagerObject.GetComponent<GameManager>();

        if (gameManager != null)
        {
            if (gameManager.IsSpawned)
                gameManager.PlayerCountUpdateServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
