using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : Item
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Use()
    {
        Vector2 pos = GameManager.Instance.GetPlayerPos();
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 1f);
        foreach (Collider2D col in cols)
        {
            if (col.CompareTag("Player"))
            {
                continue;
            }
            if (col.gameObject.TryGetComponent(out Crate crate))
            {
                Debug.Log("found a crate");
                if (crate.productNeeded == SO_Item&&!crate.full)
                {
                    Debug.Log("Able to deposit product " + SO_Item.itemName + " into " + crate.name);
                    int depositAmount = itemCount;
                    int amountAfterDepot = crate.currentAmount + depositAmount;
                    if (amountAfterDepot>crate.amountNeeded)
                    {
                        depositAmount = amountAfterDepot - crate.amountNeeded;
                    }
                    crate.Deposit(this,depositAmount);
                    itemCount -= depositAmount;
                    if(itemCount<=0)
                    {
                        GameManager.Instance.ClearItem();
                        gameObject.transform.SetParent(null);
                        gameObject.SetActive(false);
                    }
                    // Play animation
                    // Play SFX
                    return;
                }
            }

        }
        GameManager.Instance.CantDoThat();
    }
}
