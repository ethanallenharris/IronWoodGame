using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : ButtonBase
{
    public MenuManager _menuManager;
    public override void OnClick()
    {
        _menuManager.UpdateMenuPage("Play");
    }
}
