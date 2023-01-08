using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedContainer : Item
{
    [SerializeField]
    SpriteRenderer childSR;
    [SerializeField]
    SpriteRenderer productSR;
    [SerializeField]
    GameObject seedPouchPrefab;


    public SO_Item seedPouch;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        childSR.sprite = seedPouch.sprite;
        productSR.sprite = seedPouch.ProductSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject GetSeeds()
    {
        GameObject obj = Instantiate(seedPouchPrefab, transform.position, Quaternion.identity);
        Seed seed = obj.GetComponent<Seed>();
        if (seed == null) Debug.Log("couldnt grab seed from " + obj.name);
        seed.Setup(seedPouch);
        //obj.SetActive(false);
        return obj;
    }
}
