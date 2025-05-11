using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClass
{
    public Transform FindChildByNameContains(Transform parent, string nameContains)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(nameContains))  // Checks if the name contains "OpenedBook"
            {
                return child;
            }
        }
        return null; // No matching child found
    }
}
