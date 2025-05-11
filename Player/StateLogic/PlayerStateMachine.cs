using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using System.Linq;
using Unity.Collections;

public class PlayerStateMachine : NetworkBehaviour
{

    //Player state machine acts like the brain of the player.
    //This is responsible for allowing player movement, inventory access, spell casting etc
    //This is done by putting players into states with thier each exit case

    #region variables

    private HelperClass helper = new HelperClass();

    //References
    public Camera camera;
    public GameObject cameraGameObject;
    public GameObject playerController;
    public GameObject player;
    public PlayerAnimator playerAnimator;
    public ClientNetworkAnimator clientNetworkAnimator;
    public Animator animator;
    [HideInInspector]
    public GameObject HUD;
    public List<GameObject> playerBodyNodes;
    [HideInInspector]
    public InventoryManager inventory;
    [HideInInspector]
    public ItemObject currentWeapon;
    [HideInInspector]
    public Item[] armor = new Item[3];
    public ItemObject itemToGive;

    //Player Scripts
    public InputHandler input;

    //player Stats
    public float playerMovespeed;
    public float currentSpeedMultiplier;
    public ItemObject[] equipment = new ItemObject[11];

    //player bools
    [HideInInspector]
    public bool isRolling = false;

    //States
    public PlayerBaseState currentState;
    [HideInInspector]
    public PlayerStateFactory states;
    private string playerCurrentState;
    [HideInInspector]
    public string playerDashDirection;
    [HideInInspector]
    public string playerAttack;


    public List<GameObject> playersList = new List<GameObject>();

    //Network variables
    public NetworkVariable<FixedString64Bytes> playerId;

    #endregion

    #region RPCs

    [ServerRpc]
    public void UpdatePlayerCountServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (!playersList.Contains(p))
            {
                p.GetComponent<PlayerStateMachine>().playerId = new NetworkVariable<FixedString64Bytes>(Guid.NewGuid().ToString(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
                playersList.Add(p);
            }
        }
    }

    [ServerRpc]
    public void EquipBookServerRpc(ulong senderClientId, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var client = NetworkManager.ConnectedClients[OwnerClientId];
        var clientStateMachine = client.PlayerObject.GetComponent<PlayerStateMachine>();

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            if (clientStateMachine.IsClient)
            {
                clientStateMachine.ToggleInventoryClientRpc(senderClientId);
            }

            //if (clientStateMachine.IsServer)
            //{
            //    clientStateMachine.ToggleInventoryClientRpc(senderClientId);
            //}
        }

        //Debug.Log(indx);

        //Debug.Log($"number of players {playersList.Count()}");
    }


    [ClientRpc]
    public void ToggleInventoryClientRpc(ulong senderClientId)
    {
        bool isLocalClient = senderClientId == NetworkManager.Singleton.LocalClientId;

        //Debug.Log($"client side playerID: {playerId.Value}");

        if (currentState is PlayerInventoryState)
        {
            if (isLocalClient)
                HUD.transform.Find("AdventurersBook").gameObject.SetActive(false);

            currentState = states.Idle();
            states.Idle().EnterState();
            animator.SetBool("OpenedInventory", false);
            clientNetworkAnimator.Animator.SetBool("OpenedInventory", false);
            //ClearHand("left");

            var targetHand = GetHand("left");
            var book = FindChildByNameContains(targetHand, "OpenedBook(Clone)");

            Debug.Log($"Result of trying to find book: {book != null}");
            NetworkManager.Destroy(book);
            Destroy(book);
        }
        else
        {
            if (IsHost)
                Debug.Log("rpc called on the host");

            currentState = states.Inventory();
            states.Inventory().EnterState();

            animator.SetBool("OpenedInventory", true);
            clientNetworkAnimator.Animator.SetBool("OpenedInventory", true);

            if (isLocalClient)
                HUD.transform.Find("AdventurersBook").gameObject.SetActive(true);

            GameObject book = Resources.Load<GameObject>("Prefabs/OpenedBook");
            GameObject targetHand = GetHand("left");

            //foreach (Transform child in targetHand.transform)
            //{
            //    Destroy(child.gameObject);
            //    NetworkManager.Destroy(child.gameObject);
            //}

            // Instantiate new object as child of the hand
            GameObject newObj = Instantiate(book, targetHand.transform);
            //NetworkManager.Instantiate(book, targetHand.transform);

            // Make sure the new object is correctly positioned relative to the hand
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, 62);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerLoadoutServerRpc(ulong senderClientId, ItemObject[] equipmentParam, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var client = NetworkManager.ConnectedClients[OwnerClientId];
        var clientStateMachine = client.PlayerObject.GetComponent<PlayerStateMachine>();

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            if (clientStateMachine.IsClient)
            {
                clientStateMachine.UpdatePlayerLoadoutClientRpc(senderClientId, equipmentParam);
            }

            if (clientStateMachine.IsServer)
            {
                return;
            }
        }
        else
        {
            Debug.LogError("Client not found in ConnectedClients.");
        }
    }


    [ClientRpc]
    public void UpdatePlayerLoadoutClientRpc(ulong senderClientId, ItemObject[] equipmentParam)
    {
        bool isLocalClient = senderClientId == NetworkManager.Singleton.LocalClientId;

        //EquipInHand(equipmentParam[0].itemObject[0], "left");

        if (isLocalClient)
        {
            Debug.Log($"called update player loadout (local client)");
            //Debug.Log(JsonUtility.ToJson(equipmentParam.Where(s => s != null).Select(s => s.description).ToArray()));
            EquipInHand(equipment[0].itemObject[0], "left");
        }
        else
        {
            Debug.Log($"called update player loadout: {senderClientId}");
            //Debug.Log(JsonUtility.ToJson(equipmentParam.Where(s => s != null).Select(s => s.description).ToArray()));
            EquipInHand(equipment[0].itemObject[0], "left");
        }


        //Debug.Log($"called update player loadout (local client)");
        //Debug.Log(JsonUtility.ToJson(equipmentParam.Where(s => s != null).Select(s => s.description).ToArray()));
        //EquipInHand(equipment[0].itemObject[0], "left");

        //if (IsHost)
        //    EquipInHand(equipmentParam[0].itemObject[0], "left");

    }

    [ClientRpc]
    public void CheckEquipmentAndStatsClientRpc(ulong senderClientId)
    {
  
    }

    #endregion

    void Awake()
    {

    }

    public void EnterState(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                currentState = states.Idle();
                currentState.EnterState();
                break;

        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Instantiate(cameraGameObject, new Vector3(0, 0, 0), Quaternion.identity * Quaternion.Euler(39.5f, 0, 0));
            CameraFollowPlayer();
            UpdatePlayerCountServerRpc();
        }

        currentSpeedMultiplier = 1;
        states = new PlayerStateFactory(this);
        //playerAnimator.FetchAnimator();
        currentState = states.Idle();
        currentState.EnterState();
        animator.Play("Walking");
        clientNetworkAnimator.Animator.Play("Walking");

        var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        playerController.transform.position = spawnPoint.transform.position;

        HUD = GameObject.Find("HUD");
        inventory = HUD.transform.Find("AdventurersBook").Find("LoadoutPage").GetComponent<InventoryManager>();
        inventory.player = this;
    }

    void Update()
    {
        if (!IsOwner) return;


        if (currentState != null)
            currentState.UpdateState();

        CameraFollowPlayer();

        //dash cooldown

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (inventory == null)
                Debug.Log("Null inventory");
            inventory.Add(itemToGive);
        }

        //count down timers

        if (currentState == null)
        {
            states = new PlayerStateFactory(this);
            currentState = states.Idle();
            currentState.EnterState();
        }
    }



    public float CameraX;
    public float CameraY;
    public float CameraZ;
    public void CameraFollowPlayer()
    {
        if (!IsOwner) return;
        if (camera == null)
        {
            try
            {
                camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            catch (Exception e) { }
        }
        if (camera != null)
            camera.transform.position = playerController.transform.position + new Vector3(CameraX, CameraY, CameraZ);
    }

    #region Actions

    public void EquipItem(GameObject objectParent, GameObject prefab, Vector3 offsetVector, Quaternion offsetQuaternion)
    {
        // Instantiate new object as child of the objectParent
        GameObject newObj = Instantiate(prefab, objectParent.transform);
        //NetworkManager.Instantiate(prefab, objectParent.transform);

        // Make sure the new object is correctly positioned relative to the objectParent
        newObj.transform.localPosition = offsetVector;
        newObj.transform.localRotation = offsetQuaternion;
    }

    public float PlayerRotationSpeed;

    public void Movement()
    {
        if (!IsOwner) return;
        //moves player according to players inputs
        playerController.transform.Translate(new Vector3(input.InputVector.x, 0, input.InputVector.y) * playerMovespeed);
        if (animator == null)
            Debug.Log("NO ANIMATOR Found");
         animator.SetFloat("Horizontal", input.InputVector.x);
        animator.SetFloat("Vertical", input.InputVector.y);
        clientNetworkAnimator.Animator.SetFloat("Horizontal", input.InputVector.x);
        clientNetworkAnimator.Animator.SetFloat("Vertical", input.InputVector.y);
    }

    public void RotateCharacter()
    {
        if (!IsOwner) return;
        if (camera == null)
        {
            try
            {
                camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            catch (Exception e) { }
        }

        try
        {
            Ray cameraRay = camera.ScreenPointToRay(input.Mouseposition);
            float rayLength;

            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    rayLength = hit.distance;
                    Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                    Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
                    playerController.transform.LookAt(new Vector3(pointToLook.x, playerController.transform.position.y, pointToLook.z));
                }
            }
            else
            {
                Ray cameraRayBackup = camera.ScreenPointToRay(input.Mouseposition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                float rayLengthBackup;

                if (groundPlane.Raycast(cameraRay, out rayLengthBackup))
                {
                    Vector3 pointToLook = cameraRayBackup.GetPoint(rayLengthBackup);
                    Debug.DrawLine(cameraRayBackup.origin, pointToLook, Color.blue);
                    playerController.transform.LookAt(new Vector3(pointToLook.x, playerController.transform.position.y, pointToLook.z));
                }
            }
        }
        catch (Exception e) { }

    }

    public void ResetCharacterRotation()
    {
        player.transform.rotation = Quaternion.Euler(-90f, playerController.transform.rotation.eulerAngles.y, playerController.transform.rotation.eulerAngles.z);
    }

    [HideInInspector]
    public float InventoryCooldown;

    public void DetectInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) && (InventoryCooldown <= 0 || currentState is PlayerIdleState))
        {
            EquipBookServerRpc(NetworkManager.Singleton.LocalClientId);
            //GameObject targetHand = GetHand("left");
            InventoryCooldown = 0.75f;
            //if (currentState is PlayerInventoryState)
            //{
            //    ////
            //    //currentState = states.Idle();
            //    //states.Idle().EnterState();
            //    //ToggleInventory(false);
            //    //ClearHand("left");
            //    //ToggleInventoryServerRpc(true, targetHand);
            //}
            //else
            //{
            //    //ToggleInventoryServerRpc(false, targetHand);
            //    ////
            //    //currentState = states.Inventory();
            //    //states.Inventory().EnterState();
            //    //ToggleInventory(true);
            //    //GameObject book = Resources.Load<GameObject>("Prefabs/OpenedBook");
            //    //GameObject targetHand = GetHand("left");

            //    //foreach (Transform child in targetHand.transform)
            //    //{
            //    //    Destroy(child.gameObject);
            //    //    NetworkManager.Destroy(child.gameObject);
            //    //}

            //    //// Instantiate new object as child of the hand
            //    //GameObject newObj = Instantiate(book, targetHand.transform);
            //    //NetworkManager.Instantiate(book, targetHand.transform);

            //    //// Make sure the new object is correctly positioned relative to the hand
            //    //newObj.transform.localPosition = Vector3.zero;
            //    //newObj.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 0, 62);
            //}
        }
    }


    public bool DetectDash()
    {
        //Take stamina later
        if (Input.GetButton("spacebar"))
        {
            currentState = states.Dash();
            states.Dash().EnterState();
            return true;
        }
        return false;
    }

    public void SpawnWeaponAttack(GameObject obj)
    {
        // Get the player's forward direction
        Vector3 playerForward = playerController.transform.forward;
        Vector3 spawnOffset = playerForward + new Vector3(0, 0.55f, 0);
        Vector3 spawnPosition = playerController.transform.position + spawnOffset;
        Instantiate(obj, spawnPosition, playerController.transform.rotation);
    }



    public bool DetectAttack()
    {
        if (!IsOwner) return false;
        //Take stamina later
        if (Input.GetMouseButton(0))
        {
            RotateCharacter();
            currentState = states.Attack();
            states.Attack().EnterState();
            return true;
        }
        return false;
    }

    public IEnumerator Move(Vector3 direction, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float step = direction.magnitude * (Time.deltaTime / duration);
            transform.Translate(direction.normalized * step);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
    }

    private float doubleTapTime = 0.2f; // The maximum amount of time between taps to register as a double tap
    private float lastTapTimeW, lastTapTimeA, lastTapTimeS, lastTapTimeD;

    public bool DetectStep()
    {
        // Check for double taps on W, A, S, or D and if Player has enough stamina
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastTapTimeW <= doubleTapTime)
            {
                //if (!PlayerStats.StaminaLose(5)) { return; } else { OnDoubleTapW(); }
                StepDirection("Forwards");
                return true;
            }
            lastTapTimeW = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastTapTimeA <= doubleTapTime)
            {
                //if (!PlayerStats.StaminaLose(5)) { return; } else { OnDoubleTapA(); }
                StepDirection("Left");
                return true;
            }
            lastTapTimeA = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastTapTimeS <= doubleTapTime)
            {
                //if (!PlayerStats.StaminaLose(5)) { return; } else { OnDoubleTapS(); }
                StepDirection("Backwards");
                return true;
            }
            lastTapTimeS = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapTimeD <= doubleTapTime)
            {
                //if (!PlayerStats.StaminaLose(5)) { return; } else { OnDoubleTapD(); }
                StepDirection("Right");
                return true;
            }
            lastTapTimeD = Time.time;
        }
        return false;
    }

    void StepDirection(string direction) 
    {
        animator.Play($"Step{direction}");
        clientNetworkAnimator.Animator.Play($"Step{direction}");
        currentState = states.Step();
        playerDashDirection = direction;
        states.Step().EnterState();
        animator.Play("StepForward"); 
    }

    ///<summary>
    ///accepts "left" and "right"
    ///</summary>
    private GameObject GetHand(string hand)
    {
        if (hand == "right")
        {
            return playerBodyNodes.FirstOrDefault(s => s.name == "RightHandNode");
        }
        else if (hand == "left")
        {
            return playerBodyNodes.FirstOrDefault(s => s.name == "LeftHandNode");
        }
        else
        {
            Debug.LogError("Invalid hand specified. Must be 'right' or 'left'.");
            return null;
        }
    }


    public void ClearHand(string hand)
    {
        GameObject targetHand = GetHand(hand);
        ClearObject(targetHand);
    }


    public void ClearObject(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
            NetworkManager.Destroy(child.gameObject);
        }
    }

    public void EquipInHand(GameObject obj, string hand)
    {
        GameObject targetHand = GetHand(hand);
        //foreach (Transform child in targetHand.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        if (targetHand == null)
        {
            Debug.Log($"Hand: {hand} was null");
            return;
        }

        if (obj == null)
        {
            Debug.Log($"Obj was null");
            return;
        }

        Debug.Log($"Is host: {IsHost}");

        Debug.Log($"Attempt to instantiate {obj.name} into {hand} hand");
        // Instantiate new object as child of the hand
        GameObject newObj = Instantiate(obj, targetHand.transform);
        GameObject networkObj = NetworkManager.Instantiate(obj, targetHand.transform);

        // Make sure the new object is correctly positioned relative to the hand
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localRotation = Quaternion.identity;
        networkObj.transform.localPosition = Vector3.zero;
        networkObj.transform.localRotation = Quaternion.identity;
    }

    public void UpdateEquipment()
    {
        //MIGHT JUST DO THE LOGICS ON THE SLOTS

        bool equipmentChanged = false;

        //See if weapon changed (Maybe)

        ItemObject[] slotsEquipment = new ItemObject[11];

        //Weapon
        slotsEquipment[0] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.Weapon).item;

        //Headpiece
        slotsEquipment[1] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.Helmet).item;

        //Chestpiece
        slotsEquipment[2] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.ChestPiece).item;

        //Leggings
        slotsEquipment[3] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.Leggings).item;

        //Footwear
        slotsEquipment[4] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.Boots).item;

        //Trinkets
        //slotsEquipment[4] = inventory.inventorySlots.FirstOrDefault(s => s.slotType == InventorySlot.SlotType.).item;

        //Check if serverRpc should be called
        //if any equipment has changed
        for (int i = 0; i < equipment.Length; i++)
        {
            var item = slotsEquipment[i];

            if (equipment[i] != item)
            {
                equipment[i] = item;
                equipmentChanged = true;
            }
        }

        if (equipmentChanged)
        {
            Debug.Log("THIS IS THE BASE FUNCTION" + JsonUtility.ToJson(equipment.ToArray()));
            //UpdatePlayerLoadoutServerRpc(NetworkManager.Singleton.LocalClientId, slotsEquipment);
            UpdatePlayerLoadoutServerRpc(NetworkManager.Singleton.LocalClientId, slotsEquipment);
        }

    }

    public void UpdateSpells()
    {

    }

    public GameObject FindChildByNameContains(GameObject parent, string nameContains)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name.Contains(nameContains))
            {
                return child.gameObject;
            }

            // Recursive search for deeper children
            GameObject found = FindChildByNameContains(child.gameObject, nameContains);
            if (found != null)
            {
                return found;
            }
        }
        Debug.Log($"{nameContains} not found in {parent.name}");
        return null;
    }


    #endregion 
}
