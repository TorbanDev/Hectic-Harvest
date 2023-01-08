using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropLand : MonoBehaviour
{
    float maxGrowthTime;
    float currentGrowthTime;
    int growthState = 0;
    float maxRegenTime=10f;
    float currentRegenTime;

    public Crop_State state;
    Seed seed;
    SpriteRenderer sr;

    [SerializeField]
    SpriteRenderer plantSR;

    [SerializeField]
    Sprite PlowedSprite;
    [SerializeField]
    Sprite UnplowedSprite;
    [SerializeField]
    Sprite SowedSprite;
    [SerializeField]
    Sprite WateredSprite;

    [SerializeField]
    Seed testSeed;

    [SerializeField]
    Animator animator;
    


    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = UnplowedSprite;
        animator = GetComponent<Animator>();
        currentRegenTime = maxRegenTime;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case Crop_State.AWAITING_PLOW:
                break;
            case Crop_State.AWAITING_SEED:
                break;
            case Crop_State.AWAITING_WATER:
                break;
            case Crop_State.GROWING:
                Grow();
                break;
            case Crop_State.AWAITING_HARVEST:
                break;
            case Crop_State.DEAD:
                Regen();
                break;
            default: break;
        }
    }
    public void Grow()
    {
        currentGrowthTime -= Time.deltaTime;
        if (currentGrowthTime <= 0)
        {
            // finished growing

            growthState = 3;
            UpdatePlantSprite(growthState);
            FinishGrowth();
            return;
        }
        float growthPercent = (maxGrowthTime-currentGrowthTime)/maxGrowthTime;
        if(growthState<1&&growthPercent>=.33)
        {
            growthState = 1;
            UpdatePlantSprite(growthState);
        }
        else if(growthState<2&&growthPercent>=.66)
        {
            growthState = 2;
            UpdatePlantSprite(growthState);
        }


    }
    public void Plow()
    {
        Debug.Log("plowed");
        //sr.sprite = PlowedSprite;
        animator.SetTrigger("Plow");
        state = Crop_State.AWAITING_SEED;
    }
    public void PlantSeed(Seed newSeed)
    {
        seed = newSeed;
        //sr.sprite = SowedSprite;
        growthState = 0;
        maxGrowthTime = seed.growthTime;
        currentGrowthTime = maxGrowthTime;
        animator.SetTrigger("Sow");
        state = Crop_State.AWAITING_WATER;
    }
    public void Water()
    {
        Debug.Log("watered");
        //sr.sprite = WateredSprite;
        animator.SetTrigger("Water");
        state = Crop_State.GROWING;
        Invoke("FinishWater", .1f);
    }
    void FinishWater()
    {
        animator.SetTrigger("Watered");
    }
    public void FinishGrowth()
    {
        state = Crop_State.AWAITING_HARVEST;
    }
    public void Harvest()
    {
        //sr.sprite = UnplowedSprite;
        animator.SetTrigger("Harvest");
        plantSR.sprite = null;
        SpawnProduct();
        currentRegenTime = maxRegenTime;
        state = Crop_State.DEAD;
        seed = null;

    }

    private void SpawnProduct()
    {
        Vector2 playerPos = GameManager.Instance.GetPlayerPos();
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        GameObject obj = Instantiate(seed.SO_Item.SpawnablePrefab, transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddForce(currentPos - playerPos * 2, ForceMode2D.Impulse);
        //
    }

    public Crop_State GetState()
    {
        return state;
    }
    void UpdatePlantSprite(int state)
    {
        Sprite sprite = null;
        switch (state)
        {
            case 1:
                sprite=seed.SO_Item.state1Sprite != null ? seed.SO_Item.state1Sprite : null;
                break;
            case 2:
                sprite = seed.SO_Item.state2Sprite != null ? seed.SO_Item.state2Sprite : null;
                break;
            case 3:
                sprite = seed.SO_Item.state3Sprite != null ? seed.SO_Item.state3Sprite : null;
                break;
            default: break;
        }
        plantSR.sprite = sprite;
    }

    public void Next()
    {
        switch (state)
        {
            case Crop_State.AWAITING_PLOW:
                Plow();
                break;
            case Crop_State.AWAITING_SEED:
                PlantSeed(testSeed);
                break;
            case Crop_State.AWAITING_WATER:
                Water();
                break;
            case Crop_State.GROWING:
                Grow();
                break;
            case Crop_State.AWAITING_HARVEST:
                break;
            case Crop_State.DEAD:
                Regen();
                break;
            default: break;
        }

    }

    private void Regen()
    {
        currentRegenTime -= Time.deltaTime;
        if (currentRegenTime <= 0)
        {
            // finished growing
            FinishRegen();
            return;
        }
    }

    private void FinishRegen()
    {
        state = Crop_State.AWAITING_PLOW;
        animator.SetTrigger("Regen");
    }
}
public enum Crop_State
{
    AWAITING_PLOW=0,
    AWAITING_SEED=1,
    AWAITING_WATER=2,
    GROWING=3,
    AWAITING_HARVEST=4,
    DEAD=5
}
