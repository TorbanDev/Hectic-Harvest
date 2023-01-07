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
    public void UseItem()
    {
        if(inventory.HoldingItem())
        {
            inventory.GetHeldItem().Use();
        }
    }

}
