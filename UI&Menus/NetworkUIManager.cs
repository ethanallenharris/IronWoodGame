using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class NetworkUIManager : MenuManager
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject menuCamera;

    public override void UpdateMenuPage(string v)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            parent.SetActive(false);
            Debug.Log($"Has menu camera got a reference?: {menuCamera == null}");
            menuCamera.SetActive(false);
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            parent.SetActive(false);
            menuCamera.SetActive(false);
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            parent.SetActive(false);
            menuCamera.SetActive(false);
        });
    }

}