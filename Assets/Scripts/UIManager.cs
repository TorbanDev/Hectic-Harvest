using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]
    GameObject infoBoardContainer;
    [SerializeField]
    GameObject store;
    [SerializeField]
    GameObject scoreboard;
    [SerializeField]
    GameObject targetPoint;
    [SerializeField]
    TextMeshProUGUI shopMoneyText;
    [SerializeField]
    float animSpeed = 1000f;

    [SerializeField]
    GameObject gameoverScreen;
    [SerializeField]
    TextMeshProUGUI finalScoreText;

    [SerializeField]
    List<ShopItem> shopItems;

    Vector3 ScoreHome;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        ScoreHome = infoBoardContainer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowScore()
    {
        scoreboard.SetActive(true);
    }
    public void ShowBoard()
    {
        infoBoardContainer.gameObject.SetActive(true);
        scoreboard.SetActive(true);
        MoveItem(infoBoardContainer.transform, targetPoint.transform.position);
    }
    public void RemoveBoard()
    {
        infoBoardContainer.transform.position = ScoreHome;
        infoBoardContainer.gameObject.SetActive(false);
        scoreboard.SetActive(false);
        store.SetActive(false);
    }
    public void MoveItem(Transform source, Vector3 target)
    {
        StartCoroutine(MoveScreen(source,target));
    }
    public void UpdateMoney()
    {
        shopMoneyText.SetText("$"+GameManager.Instance.totalProfit);
    }
    public IEnumerator MoveScreen(Transform source, Vector3 target)
    {

        // Debug.Log(Vector3.Distance(source.position, target));
        while (true)
        {
            while(Vector3.Distance(source.position, target)>1f)
            {
                //Debug.Log(Vector3.Distance(source.position, target));
                source.transform.position = Vector3.MoveTowards(source.position, target,Time.deltaTime* animSpeed);
                yield return new WaitForSeconds(.01f);
            }
            
            yield return new WaitForSeconds(1f);
        }


    }

    public void ResetScoreboard()
    {
        infoBoardContainer.transform.position = ScoreHome;
    }
    public void GoToShop()
    {
        shopMoneyText.SetText(GameManager.Instance.totalProfit.ToString());
        GameManager.Instance.ResetScoreboard();
        scoreboard.SetActive(false);
        UpdateShop();
        store.SetActive(true);
    }

    public void UpdateShop()
    {
        foreach (ShopItem item in shopItems)
        {
            item.Refresh();
        }
    }
    public void GameOver()
    {
        scoreboard.SetActive(false);
        store.SetActive(false);
        infoBoardContainer.SetActive(true);
        infoBoardContainer.transform.position = targetPoint.transform.position;
        finalScoreText.SetText("Score " + GameManager.Instance.totalScore);
        gameoverScreen.SetActive(true);
    }
}
