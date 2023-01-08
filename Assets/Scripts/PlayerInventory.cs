using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    Item heldItem;
    [SerializeField]
    SpriteRenderer itemRenderer;
    [SerializeField]
    SpriteRenderer previewRenderer;
    [SerializeField]
    PlayerController player;

    [SerializeField]
    float dropRange;

    Vector3 interactPoint;

    GameObject lastPickedUpItem;
    int cachedLayer = 0;

    public int maxStackSize = 3;

    // Start is called before the first frame update
    void Start()
    {
        previewRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PickupItem(Item item)
    {
        previewRenderer.enabled = true;
        heldItem = item;
        //itemRenderer.sprite = item.SO_Item.sprite;
        item.gameObject.transform.SetParent(gameObject.transform);
        item.gameObject.transform.position = itemRenderer.transform.position;
        lastPickedUpItem = item.gameObject;
        Debug.Log("********PICKUP****************");
        if(item.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        cachedLayer = lastPickedUpItem.layer;
        item.gameObject.layer = 6;
        // lastPickedUpItem.SetActive(false);
        // Play pickup SFX pop sound
    }
    void StackItem(Item item)
    {
        heldItem.itemCount++;
        item.gameObject.SetActive(false);
        // Play pickup SFX pop sound
    }
    void PickupNonLootable(SeedContainer container)
    {
        previewRenderer.enabled = true;
        GameObject newSeedPouch= container.GetSeeds();

        Item item= newSeedPouch.GetComponent<Seed>();
        item.itemCount = maxStackSize;
        heldItem = item;
        //itemRenderer.sprite = item.SO_Item.sprite;

        if (newSeedPouch.TryGetComponent(out Rigidbody2D rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        lastPickedUpItem = newSeedPouch;
        cachedLayer = lastPickedUpItem.layer;
        lastPickedUpItem.layer = 6;
        lastPickedUpItem.transform.SetParent(gameObject.transform);
        lastPickedUpItem.transform.position = itemRenderer.transform.position;
        // lastPickedUpItem.SetActive(false);
        // Play pickup SFX pop sound
    }
    public void Throw()
    {
        if (!HoldingItem()) return;
        DropItem(heldItem);
    }
    void DropItem(Item item)
    {
        
        Vector3 pos= player.playerCenter.position;
        Vector3 interactionDir = player.interactionDirection;
        Vector3 throwDir = (pos+interactionDir).normalized;

        Debug.Log("player center pos: " + pos);
        Debug.Log("interactionDir: " + interactionDir);
        Debug.Log("throwDir: " + throwDir);

        previewRenderer.enabled = false ;
        heldItem = null;
        itemRenderer.sprite = null;
        if(lastPickedUpItem!=null)
        {
            
            lastPickedUpItem.transform.SetParent(null);
            lastPickedUpItem.transform.position = (pos);
            
        }
        if (lastPickedUpItem.TryGetComponent(out Rigidbody2D rb))
        {
            
            rb.bodyType = RigidbodyType2D.Dynamic;
            float force = player.IsStationary() ? 2f : player.throwForce;
            rb.AddForce(interactionDir * force, ForceMode2D.Impulse);
        }
        lastPickedUpItem.layer = cachedLayer;
        lastPickedUpItem = null;
    }
    public void TryGrab()
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
                        //col.gameObject.SetActive(false);
                        return;
                    }
                    else
                    {
                        // Try to get a seed
                        if(col.gameObject.TryGetComponent(out SeedContainer container))
                        {
                            PickupNonLootable(container);
                        }
                    }
                }
            }
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
        previewRenderer.enabled = false;
        heldItem = null;
        itemRenderer.sprite = null;
        lastPickedUpItem = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (heldItem == null) return;
        if (heldItem.SO_Item.type != item_type.PRODUCT) return;
        if (lastPickedUpItem == collision.gameObject) return;

        if(collision.gameObject.TryGetComponent(out Product product))
        {
            if(heldItem.SO_Item==product.SO_Item)
            {
                if (heldItem.itemCount < maxStackSize)
                {
                    Debug.Log("Stacking item and deleting copy");
                    StackItem(product);
                }
            }
        }
    }
}
