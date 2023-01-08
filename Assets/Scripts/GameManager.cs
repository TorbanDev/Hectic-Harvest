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
    AudioClip waterfillClip;
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

    [SerializeField]
    GameObject cornSpawner;
    [SerializeField]
    GameObject zukeSpawner;
    [SerializeField]
    GameObject grapeSpawner;

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

    [SerializeField]
    SO_Item Tomato;
    [SerializeField]
    SO_Item Corn;
    [SerializeField]
    SO_Item Zuke;
    [SerializeField]
    SO_Item Grapes;
    [SerializeField]
    CrateSpawner spawner;
    [SerializeField]
    AudioManager audioManager;


    // Day/Night
    [SerializeField]
    SpriteRenderer nightSR;

    Color nightColorStart = new Color(.06f, .054f,.217f,0f);
    [SerializeField]
    float currentDayTimer;
    [SerializeField]
    float maxDayTimer = 30f;
    int dayState = 0;

    public GM_STATE state = GM_STATE.PAUSE;

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
    }
    public void FillBucket()
    {
        audioSource.PlayOneShot(waterfillClip);
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
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        audioManager.PlayNight();
        StartCoroutine(Sunup());
        yield return new WaitForSeconds(3f);
        state = GM_STATE.PLAY;
        StartCoroutine(NextWave());
        audioManager.PlayDay();
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
            if(dayState<1&&(maxDayTimer-currentDayTimer)/maxDayTimer>=.75)
            {
                dayState = 1;
                StartCoroutine(Sundown());
            }
        }
    }
    int overtime = 0;

    IEnumerator Sundown()
    {
        Color tempColor = nightSR.color;
        while (tempColor.a < .51)
        {
            tempColor.a += .0008f;
            nightSR.color = tempColor;
            yield return null;

        }
 
    }
    IEnumerator Sunup()
    {
        Color tempColor = nightSR.color;
        while (tempColor.a > 0)
        {
            tempColor.a -= .008f;
            nightSR.color = tempColor;
            yield return null;

        }

    }

    private void DayFinished()
    {
        audioManager.StopPlaying();
        audioManager.PlayNight();
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
        todaysSales.Clear();

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
        audioManager.PlayDay();
        StartCoroutine(NextWave());
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
        List<TextMeshProUGUI> moneys = new List<TextMeshProUGUI>();
        List<int[]> amountAndValue = new List<int[]>();
        yield return new WaitForSeconds(2f);
        // Populate rows of harvest
        // Stamp strikes
        if (dailyStrikes > 0)
        {
            for(int i=0;i<dailyStrikes;i++)
            {
                yield return new WaitForSeconds(1f);
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
                yield return new WaitForSeconds(.0005f);
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
        player.SetState(Player_State.INACTVE);
        Invoke("ReleasePlayer",delay);
    }
    public void ReleasePlayer()
    {
        player.SetState(Player_State.ACTIVE);
    }

    IEnumerator NextWave()
    {
        List<SO_Item> products = new List<SO_Item>();
        List<int> amounts = new List<int>();
        float timer = 60;
        float delay = 0f; ;

        switch(dayCounter)
        {
            case 1:
                products.Add(Tomato);
                products.Add(Tomato);
                products.Add(Tomato);
                products.Add(Tomato);
                products.Add(Tomato);
                products.Add(Tomato);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(2);
                amounts.Add(1);
                amounts.Add(2);
                amounts.Add(1);
                break;
            case 2:
                cornSpawner.SetActive(true);
                products.Add(Corn);
                products.Add(Tomato);
                products.Add(Corn);
                products.Add(Tomato);
                products.Add(Corn);
                products.Add(Tomato);
                amounts.Add(1);
                amounts.Add(2);
                amounts.Add(1);
                amounts.Add(2);
                amounts.Add(1);
                amounts.Add(2);
                break;
            case 3:
                products.Add(Corn);
                products.Add(Tomato);
                products.Add(Tomato);
                products.Add(Corn);
                amounts.Add(4);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(4);
                break;
            case 4:
                zukeSpawner.SetActive(true);
                products.Add(Zuke);
                products.Add(Zuke);
                amounts.Add(3);
                amounts.Add(3);
                products.Add(Corn);
                amounts.Add(1);
                products.Add(Corn);
                amounts.Add(1);
                products.Add(Zuke);
                amounts.Add(1);
                break;
            case 5:
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                products.Add(Zuke);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(1);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                break;
            case 6:
                products.Add(Corn);
                amounts.Add(10);
                timer = 110;
                break;
            case 7:
                grapeSpawner.SetActive(true);
                products.Add(Grapes);
                products.Add(Tomato);
                products.Add(Grapes);
                products.Add(Corn);
                products.Add(Grapes);
                products.Add(Zuke);
                products.Add(Grapes);
                amounts.Add(3);
                amounts.Add(1);
                amounts.Add(3);
                amounts.Add(1);
                amounts.Add(3);
                amounts.Add(1);
                amounts.Add(1);
                break;
            case 8:
                products.Add(Corn);
                products.Add(Grapes);
                products.Add(Zuke);
                products.Add(Tomato);
                products.Add(Corn);
                products.Add(Grapes);
                products.Add(Zuke);
                products.Add(Tomato);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                break;
            case 9:
                products.Add(Zuke);
                amounts.Add(15);
                timer = 110f;
                break;
            case 10:
                products.Add(Grapes);
                products.Add(Zuke);
                amounts.Add(7);
                amounts.Add(7);
                timer = 110f;
                delay = 1f;
                break;
            case 11:
                products.Add(Corn);
                amounts.Add(20);
                break;
            case 12:
                products.Add(Corn);
                products.Add(Grapes);
                products.Add(Zuke);
                products.Add(Tomato);
                products.Add(Corn);
                products.Add(Grapes);
                products.Add(Zuke);
                products.Add(Tomato);
                amounts.Add(3);
                amounts.Add(3);
                amounts.Add(3);
                amounts.Add(3);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                amounts.Add(2);
                break;
            default:
                overtime++;
                if (dayCounter%2==0)
                {
                    products.Add(Corn);
                    products.Add(Zuke);
                    amounts.Add(Mathf.RoundToInt(dayCounter / 2));
                    amounts.Add(Mathf.RoundToInt(dayCounter / 2));
                    delay = 1f;
                    timer = 110f;
                }
                else
                {
                    SO_Item tempP = null;
                    int rand = UnityEngine.Random.Range(1, 4);
                    switch (rand)
                    {
                        case 1: tempP = Zuke;
                            break;
                        case 2: tempP = Corn;
                            break;
                        case 3: tempP = Grapes;
                            break;
                        case 4: tempP = Tomato;
                            break;
                    }
                    for(int i=0; i<= dayCounter;i++)
                    {
                        products.Add(tempP);
                        amounts.Add(1);
                    }
                }
                break;

        }

        delay=delay==0f ? (120 / (products.Count)) : delay;


        int index = 0;
        yield return new WaitForSeconds(2f);
        foreach(SO_Item item in products)
        {
            Debug.Log("spawning an item");
            SpawnCrate(item, amounts[index], timer);
            index++;
            yield return new WaitForSeconds(delay);
        }
        Debug.Log("done spawning");
        products.Clear();
        amounts.Clear();
        yield return null;
    }
    void SpawnCrate(SO_Item product, int amountNeeded, float timer)
    {
        spawner.SpawnCrate(product, amountNeeded, timer);
    }
}
public enum GM_STATE
{
    PLAY=0,
    PAUSE=1
}
