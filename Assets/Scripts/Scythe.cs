using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Item
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
            if (col.gameObject.TryGetComponent(out CropLand crop))
            {
                if (crop.GetState() == Crop_State.AWAITING_HARVEST)
                {
                    crop.Harvest();
                    // Play animation
                    // Play SFX
                }
            }

        }
        Debug.Log("didnt find valid Harvest space");
    }
}
