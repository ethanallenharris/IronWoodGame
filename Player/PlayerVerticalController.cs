using Unity.Netcode;
using UnityEngine;

public class PlayerVerticalController : NetworkBehaviour
{
    public float raycastDistance = 5f;
    public GameObject player;

    public float yOffset;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        RayCastPositionVertical();
    }

    private void RayCastPositionVertical()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // Check if the hit object is the floor
            if (hit.collider.CompareTag("Floor"))
            {
                // Set the player's Y position to the collision point
                player.transform.position = new Vector3(player.transform.position.x, hit.point.y + yOffset, player.transform.position.z);
            }
        }
    }


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        RayCastPositionVertical();
    }
}
