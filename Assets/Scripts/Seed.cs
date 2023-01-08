using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    public int growthTime;
    [SerializeField]
    SpriteRenderer childSR;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        growthTime = GetGrowthTime();
        childSR.sprite = SO_Item.ProductSprite;
    }

    private int GetGrowthTime()
    {
        int min = SO_Item.minGrowthTime;
        int max = SO_Item.maxGrowthTime;
        return UnityEngine.Random.Range(min, max);
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
            if (col.gameObject.TryGetComponent(out CropLand crop))
            {
                if(crop.GetState()==Crop_State.AWAITING_SEED)
                {
                    crop.PlantSeed(this);
                    Plant(crop, this);
                    return;
                }
            }
            
        }
        Debug.Log("didnt find valid planting space");
    }
    void Plant(CropLand crop, Seed seed)
    {
        itemCount--;
        Debug.Log("planting " + SO_Item.itemName + ". " + itemCount + " remaining");
        if (itemCount <= 0)
        {
            GameManager.Instance.ClearItem();
            gameObject.transform.SetParent(null);
            gameObject.SetActive(false);
            Debug.Log("NO MORE " + SO_Item.itemName);
        }
    }
    
}
