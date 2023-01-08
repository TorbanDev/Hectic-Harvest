using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    PlayerInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }
    public void GrabOrUseItem()
    {
        if(inventory.HoldingItem())
        {
            inventory.GetHeldItem().Use();
        }
        else
        {
            inventory.TryGrab();
        }
    }
    public void ThrowItem()
    {
        inventory.Throw();
    }

}
