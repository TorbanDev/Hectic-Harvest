using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    PlayerInventory inventory;
    [SerializeField]
    PlayerController player;

    public Dictionary<SO_Item, int> todaysSales = new Dictionary<SO_Item, int>();

    public int dailyProfit=0;
    public int totalScore = 0;
    public int totalProfit = 0;

    public void SellProduct(Crate crate)
    {
        int amount = crate.currentAmount;
        int scoreValue= crate.productNeeded.scoreValue*amount;
        int moneyValue = crate.productNeeded.moneyValue*amount;
        dailyProfit += moneyValue;
        totalScore += moneyValue;
        if (todaysSales.ContainsKey(crate.SO_Item))
        {
            todaysSales[crate.SO_Item] += amount;
        }
        else
        {
            todaysSales.Add(crate.SO_Item, amount);
        }
        Debug.Log("sold " + amount + " " + crate.SO_Item.itemName + ". new daily sales: " + dailyProfit);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearItem()
    {
        inventory.ClearItem();
    }
    public Vector2 GetPlayerPos()
    {
        return playerTransform.position;
    }
    public void HoldPlayer(float scale)
    {
        float delay = player.ActionTime * scale;
        Debug.Log("holding player for " + delay + " seconds");
        player.SetState(Player_State.INACTVE);
        Invoke("ReleasePlayer",delay);
    }
    public void ReleasePlayer()
    {
        Debug.Log("releasing player");
        player.SetState(Player_State.ACTIVE);
    }
}
