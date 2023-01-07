using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    Item heldItem;
    [SerializeField]
    SpriteRenderer itemRenderer;

    GameObject lastPickedUpItem;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void PickupItem(Item item)
    {
        heldItem = item;
        itemRenderer.sprite = item.SO_Item.sprite;
        lastPickedUpItem = item.gameObject;
        lastPickedUpItem.SetActive(false);
    }
    void DropItem(Item item)
    {
        heldItem = null;
        itemRenderer.sprite = null;
        if(lastPickedUpItem!=null)
        {
            lastPickedUpItem.transform.position = gameObject.transform.position;
            lastPickedUpItem.SetActive(true);
        }
    }
    public void TryGrabDrop()
    {
        // If holding something, drop it
        if(heldItem!=null)
        {
            DropItem(heldItem);
        }
        else
        {
            Debug.Log("trying to pickup");
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach(Collider2D col in cols)
            {
                if(col.CompareTag("Player"))
                {
                    Debug.Log("found player");
                    continue;
                }
                Debug.Log("found col: "+col.gameObject.name);
                if (col.gameObject.TryGetComponent(out Item item)) {
                    Debug.Log("found Item component: " + col.gameObject.name);
                    if (item.SO_Item.lootable)
                    {
                        PickupItem(item);
                        col.gameObject.SetActive(false);
                        return;
                    }
                }
            }
            // try grab
        }
        // check for grabable items and grab the closest
    }
    public bool HoldingItem()
    {
        return heldItem != null;
    }
    public Item GetHeldItem()
    {
        return heldItem;
    }

    public void ClearItem()
    {
        heldItem = null;
        itemRenderer.sprite = null;
        lastPickedUpItem = null;
    }
}
