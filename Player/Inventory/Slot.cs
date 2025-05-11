using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private GameObject _stackObj;

       
    public void Set(ItemObject item)
    {
        _icon.sprite = item.icon;
        if (item.stackSize <= 1)
        {
            _stackObj.SetActive(false);
            return;
        }

        //stack label stuff here
    }
}
