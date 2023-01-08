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
    

    AudioSource audioSource;
    [SerializeField]
    AudioClip coinClip;
    [SerializeField]
    AudioClip strikeClip;
    [SerializeField]
    AudioClip blipClip;
    [SerializeField]
    AudioClip goToShopClip;
    [SerializeField]
    AudioClip depositClip;
    [SerializeField]
    public AudioClip sowClip;
    [SerializeField]
    public AudioClip cantDoThatClip;

    // Upgrade refs
    [SerializeField]
    WaterBucket bucket;
    public float growthReduction = 1f;
    public float regenTime = 1f;
    public float workSpeed = 1f;




    // Points
    public Dictionary<SO_Item, int> todaysSales = new Dictionary<SO_Item, int>();

    public int dailyProfit=0;
    public int totalScore = 0;
    public int totalProfit = 0;
    public int dayCounter = 1;


    // Strikes
    [SerializeField]
    public int dailyStrikes = 0;
    [SerializeField]
    int totalStrikes = 0;
    int markedStrikes = 0;
    [SerializeField]
    Image[] Strikes;
    [SerializeField]
    Sprite strikeSprite;

    [SerializeField]
    TextMeshProUGUI dayText;

    // ScoreSprites
    [SerializeField]
    GameObject[] ScoreItems;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI moneyTextScoreboard;
    [SerializeField]
    GameObject GoToShopButton;


    // Day/Night
    [SerializeField]
    SpriteRenderer nightSR;

    Color nightColorStart = new Color(.06f, .054f,.217f,0f);
    [SerializeField]
    float currentDayTimer;
    [SerializeField]
    float maxDayTimer = 30f;
    int dayState = 0;

    public GM_STATE state = GM_STATE.PLAY;

    public void CantDoThat()
    {
        audioSource.PlayOneShot(cantDoThatClip);
    }
    public void SellProduct(Crate crate)
    {
        audioSource.PlayOneShot(depositClip);
        int amount = crate.currentAmount;
        int moneyValue = crate.productNeeded.moneyValue*amount;
        dailyProfit += moneyValue;
        totalScore += amount * crate.productNeeded.scoreValue;
        if (todaysSales.ContainsKey(crate.productNeeded))
        {
            todaysSales[crate.productNeeded] += amount;
        }
        else
        {
            todaysSales.Add(crate.productNeeded, amount);
        }
        Debug.Log("sold " + amount + " " + crate.SO_Item.itemName + ". new daily sales: " + dailyProfit);
    }

    public void DoUpgrade(Upgrade_type type)
    {
        switch(type)
        {
            case Upgrade_type.BIGGER_CANS:
                UpgradeBucket();
                break;
            case Upgrade_type.DEEPER_POCKETS:
                UpgradePockets();
                break;
            case Upgrade_type.FASTER_SHOES:
                UpgradeShoes();
                break; 
            case Upgrade_type.FERTILE_SOIL:
                UpgradeFertile();
                break; 
            case Upgrade_type.NUTRIENT_SOIL:
                UpgradeNutrient();
                break;
            case Upgrade_type.WORK_GLOVES:
                Gloves();
                break;
            default: break;
        }
    }
    public void UpgradeNutrient()
    {
        regenTime -= 1f;
    }
    public void Gloves()
    {
        workSpeed -= .2f;
    }
    public void UpgradeFertile()
    {
        growthReduction -= .15f;
    }
    public void UpgradeShoes()
    {
        player.speed += 2;
    }
    public void UpgradeBucket()
    {
        bucket.Upgrade();
    }
    public void UpgradePockets()
    {
        player.inventory.maxStackSize++;
    }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        currentDayTimer = maxDayTimer;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state==GM_STATE.PLAY)
        {
            currentDayTimer -= Time.deltaTime;
            if(currentDayTimer<0)
            {
                currentDayTimer = maxDayTimer;
                DayFinished();
            }
            if(dayState<1&&(maxDayTimer-currentDayTimer)/maxDayTimer>=.5)
            {
                dayState = 1;
                StartCoroutine(Sundown());
            }
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
    IEnumerator Sunup()
    {
        Debug.Log("BRIGHTENING");
        Color tempColor = nightSR.color;
        while (tempColor.a > 0)
        {
            tempColor.a -= .0008f;
            nightSR.color = tempColor;
            yield return null;

        }
        Debug.Log("DONE");
    }

    private void DayFinished()
    {
        state = GM_STATE.PAUSE;
        UIManager.Instance.ShowScore();
        UIManager.Instance.ShowBoard();
        StartCoroutine(SetScoreboard());
        
    }

    private void Cleanup()
    {
        // private vars to reset
        dailyProfit = 0;
        dailyStrikes = 0;
        currentDayTimer = maxDayTimer;


        // objects to cleanup
        if (inventory.HoldingItem())
        {
            inventory.Throw();
        }
        inventory.ClearItem();
        GameObject[] trash=GameObject.FindGameObjectsWithTag("Perishable");
        foreach(GameObject obj in trash)
        {
            Destroy(obj); 
        }
    }

    public void StartNewDay()
    {
        Cleanup();
        dayCounter++;
        UIManager.Instance.RemoveBoard();
        dayText.SetText("Day " + dayCounter);
        StartCoroutine(Sunup());
        StartCoroutine(NewDayStart());
    }

    IEnumerator NewDayStart()
    {
        yield return new WaitForSeconds(2f);
        state = GM_STATE.PLAY;
        yield return null;
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
        state = GM_STATE.PAUSE;
        UIManager.Instance.GameOver();
    }

    IEnumerator SetScoreboard()
    {
        Debug.Log("getting score");
        List<TextMeshProUGUI> moneys = new List<TextMeshProUGUI>();
        List<int[]> amountAndValue = new List<int[]>();
        yield return new WaitForSeconds(2f);
        // Populate rows of harvest
        // Stamp strikes
        if (dailyStrikes > 0)
        {
            Debug.Log("Setting strikes");
            for(int i=0;i<dailyStrikes;i++)
            {
                Debug.Log("i=" + i+". dailystrikes= "+ dailyStrikes);
                yield return new WaitForSeconds(1f);
                Debug.Log("STRIKE");
                int strikeIndex = markedStrikes;
                Strikes[strikeIndex].sprite = strikeSprite;
                markedStrikes++;
                audioSource.PlayOneShot(strikeClip);
            }
            dailyStrikes = 0;
        }
        int salesNum = todaysSales.Count;
        if(salesNum>0)
        {
            yield return new WaitForSeconds(1f);
            var e = todaysSales.GetEnumerator();
            for(int i=0;i<salesNum;i++)
            {
                e.MoveNext();
                int[] amounts = new int[2];

                GameObject row = ScoreItems[i];
                SO_Item item = e.Current.Key;
                int amount = e.Current.Value;
                Image img = row.GetComponentInChildren<Image>();
                img.sprite = null;
                TextMeshProUGUI tmp = row.GetComponentInChildren<TextMeshProUGUI>();
                tmp.SetText("");
                row.SetActive(true);
                moneys.Add(tmp);
                img.sprite = item.sprite;
                audioSource.PlayOneShot(blipClip);
                yield return new WaitForSeconds(.7f);
                // Play SFX
                int moneyValue = amount * item.moneyValue;
                int scoreValue = amount * item.scoreValue;

                amounts[0] = amount;
                amounts[1] = moneyValue;
                amountAndValue.Add(amounts);

                tmp.SetText(" x " + amount + " = " + moneyValue);
                audioSource.PlayOneShot(blipClip);
                scoreText.SetText("Score "+totalScore.ToString());
                yield return new WaitForSeconds(1f);
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
                totalProfit += 1;
                moneyTextScoreboard.SetText("$"+totalProfit.ToString());
                audioSource.PlayOneShot(coinClip);
                yield return new WaitForSeconds(.05f);
            }
        }
        GoToShopButton.SetActive(true);

        yield return null;
    }

    public void ResetScoreboard()
    {
        GoToShopButton.SetActive(false);
        foreach(GameObject row in ScoreItems)
        {
            row.SetActive(false);
        }
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
public enum GM_STATE
{
    PLAY=0,
    PAUSE=1
}
