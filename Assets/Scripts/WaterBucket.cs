using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBucket : Item
{
    float maxWaterLevel = 30;
    float currentWaterLevel;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        currentWaterLevel = maxWaterLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Upgrade()
    {
        maxWaterLevel += 10;
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
            else if (col.gameObject.TryGetComponent(out CropLand crop))
            {
                if (crop.GetState() == Crop_State.AWAITING_WATER)
                {
                    TryWater(crop);
                    // Make player unable to move for <action time> which is an upgradeable stat
                    return;
                }
            }
            else if(col.gameObject.TryGetComponent(out WaterWell well))
            {
                RefillBucket();
            }

        }
        Debug.Log("didnt find valid Watering space");
    }

    private void RefillBucket()
    {
        currentWaterLevel = maxWaterLevel;
        GameManager.Instance.HoldPlayer(1.5f*GameManager.Instance.workSpeed);
        // meter fill
        // play sound
    }

    private void TryWater(CropLand crop)
    {
        if (currentWaterLevel >= 10)
        {
            crop.Water();
            currentWaterLevel -= 10;
        }
        else
        {
            // show animation that bucket is empty;
            Debug.Log("tried to water crop but can is empty!");
        }
    }
}
