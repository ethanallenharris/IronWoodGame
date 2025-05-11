using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MenuManager
{
    public List<GameObject> _menuItems;

    public override void UpdateMenuPage(string update)
    {
        switch (update)
        {
            case "Play":
                SceneManager.LoadScene("Overworld");
                break;
            default:
                Debug.Log($"{update} was not recognised as a menu option");
                break;
        }
    }
}