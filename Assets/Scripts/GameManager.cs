using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // References
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    PlayerInventory inventory;
    [SerializeField]
    PlayerController player;



    // Points
    public Dictionary<SO_Item, int> todaysSales = new Dictionary<SO_Item, int>();

    public int dailyProfit=0;
    public int totalScore = 0;
    public int totalProfit = 0;


    // Strikes
    public int dailyStrikes = 0;
    int totalStrikes = 0;
    [SerializeField]
    Image[] Strikes;
    [SerializeField]
    Sprite strikeSprite;

    // ScoreSprites
    [SerializeField]
    GameObject[] ScoreItems;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI moneyTextScoreboard;

    // Day/Night
    [SerializeField]
    SpriteRenderer nightSR;

    Color nightColorStart = new Color(.06f, .054f,.217f,0f);
    [SerializeField]
    float currentDayTimer;
    [SerializeField]
    float maxDayTimer = 30f;
    int dayState = 0;
    

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
        currentDayTimer = maxDayTimer;
    }

    // Update is called once per frame
    void Update()
    {
        currentDayTimer -= Time.deltaTime;
        if(currentDayTimer<0)
        {
            DayFinished();
        }
        if(dayState<1&&(maxDayTimer-currentDayTimer)/maxDayTimer>=.5)
        {
            dayState = 1;
            StartCoroutine(Sundown());
        }
    }

    IEnumerator Sundown()
    {
        Debug.Log("DIMMING");
        Color tempColor = nightSR.color;
        while (tempColor.a < .51)
        {
            tempColor.a += .00008f;
            nightSR.color = tempColor;
            yield return null;

        }
        Debug.Log("DONE");
    }

    private void DayFinished()
    {
        UIManager.Instance.ShowScore();
        UIManager.Instance.ShowBoard();
    }
    public void AddStrike()
    {
        dailyStrikes++;
        totalStrikes++;
        if(totalStrikes>=3)
        {
            StartGameOver();
        }
    }

    private void StartGameOver()
    {
        throw new NotImplementedException();
    }

    IEnumerator SetScoreboard()
    {
        List<TextMeshProUGUI> moneys = new List<TextMeshProUGUI>();
        List<int[]> amountAndValue = new List<int[]>();
        yield return new WaitForSeconds(3f);
        // Populate rows of harvest
        // Stamp strikes
        if (dailyStrikes > 0)
        {
            
            int index = 0;
            for(int i=0;i<=dailyStrikes;i++)
            {
                yield return new WaitForSeconds(1f);
                int strikeIndex = totalStrikes - 1 + index;
                Strikes[strikeIndex].sprite = strikeSprite;
                // Play SFX of strike
                index++;
            }
            dailyStrikes = 0;
        }
        int salesNum = todaysSales.Count;
        if(salesNum>0)
        {
            var e = todaysSales.GetEnumerator();
            for(int i=0;i<salesNum;i++)
            {
                e.MoveNext();
                int[] amounts = new int[2];

                GameObject row = ScoreItems[i];
                SO_Item item = e.Current.Key;
                int amount = e.Current.Value;
                Image img = row.GetComponentInChildren<Image>();
                TextMeshProUGUI tmp = row.GetComponentInChildren<TextMeshProUGUI>();
                moneys.Add(tmp);
                img.sprite = item.sprite;
                yield return new WaitForSeconds(.4f);
                // Play SFX
                int moneyValue = amount * item.moneyValue;
                int scoreValue = amount * item.scoreValue;

                amounts[0] = amount;
                amounts[1] = moneyValue;
                amountAndValue.Add(amounts);

                tmp.SetText(" x " + amount + " = " + moneyValue);
                totalScore += scoreValue;
                scoreText.SetText(totalScore.ToString());
                yield return new WaitForSeconds(.5f);
            }

        }
        // Set image sprite=sprite from SOitem in dict
        // Set text = value of SO_item*amount
        // Add up score and $$ for each row.

        int typeCount = amountAndValue.Count;
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < typeCount; i++)
        {
            TextMeshProUGUI tmp = moneys[i];
            int[] amountandval = amountAndValue[i];
            int amount = amountandval[0];
            int value = amountandval[1];
            while(value>0)
            {
                value--;
                tmp.SetText(" x " + amount + " = " + value);
                totalProfit += value;
                moneyTextScoreboard.SetText(totalProfit.ToString());
                yield return new WaitForSeconds(.1f);
            }
        }


        yield return null;
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
