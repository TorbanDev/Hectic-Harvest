using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    SO_Upgrade SO_Upgrade;
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI description;
    [SerializeField]
    TextMeshProUGUI cost;
    [SerializeField]
    Button button;

    int timesUpgraded;
    int _cost;
    string _description;
    string _title;
    Upgrade_type _type;

    // Get Data from Scriptable object
    //   BaseCost of upgrade
    //   Cost of additional upgrade
    //   Title
    //   Description
    //   reference to Upgrade Script. specific script will inteherit from a base class with onPurchase() method. onPurchase will do something to game.
    //   int totalupgrades



    // Start is called before the first frame update
    void Awake()
    {
        _cost = SO_Upgrade.baseCost;
        _title = SO_Upgrade.title;
        _description = SO_Upgrade.initialDescription;
        title.SetText(_title);
        description.SetText(_description);
        cost.SetText("$"+_cost);
        _type = SO_Upgrade.type;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Buy()
    {
        GameManager.Instance.DoUpgrade(_type);
        timesUpgraded++;
        GameManager.Instance.totalProfit -= _cost;
        UIManager.Instance.UpdateMoney();
        UIManager.Instance.UpdateShop();
        if (SO_Upgrade.totalUpgrades-timesUpgraded>0)
        {
            _cost += SO_Upgrade.additionalCost;
            _description = SO_Upgrade.additionalDescription;
        }
        else
        {
            button.gameObject.SetActive(false);
            cost.gameObject.SetActive(false);
            _description = "No more upgrades";
            _title = "SOLD OUT";
        }
        Refresh();
    }
    public void Refresh()
    {
        title.SetText(_title);
        description.SetText(_description);
        cost.SetText("$"+_cost);
        if(_cost > GameManager.Instance.totalProfit)
        {
            Debug.Log("_cost: " + _cost + ". totalProfit" + GameManager.Instance.totalProfit);
            button.interactable = false;
        }
        else
        {

            button.interactable = true;
        }
    }
}
