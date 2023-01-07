using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crate : Item
{
    public SO_Item productNeeded;
    public int amountNeeded;
    public int currentAmount=0;
    public bool full = false;

    [SerializeField]
    SpriteRenderer productSR;

    TextMeshProUGUI tmp;


    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
        productSR.sprite = productNeeded.sprite;
    }
    void UpdateText()
    {
        tmp.SetText((amountNeeded-currentAmount).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Deposit(Product product, int amount)
    {
        currentAmount += amount;
        UpdateText();
        if (currentAmount >= amountNeeded)
        {
            full = true;
        }
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
            if (col.gameObject.TryGetComponent(out LoadingBay bay))
            {
                GameManager.Instance.SellProduct(this);
                GameManager.Instance.ClearItem();
                return;
            }

        }
        Debug.Log("didnt find valid loading space");
    }
}
