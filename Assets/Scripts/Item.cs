using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public SO_Item SO_Item;
    SpriteRenderer sr;
    public int itemCount;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = SO_Item.sprite;
        itemCount = SO_Item.maxCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Use()
    {

        // GameManager.Instance.UseItem(this);
    }
    public void ClearItem()
    {
        GameManager.Instance.ClearItem();
    }
}
