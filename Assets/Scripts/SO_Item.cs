using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Torban/Item")]
public class SO_Item : ScriptableObject
{ 
    public string itemName;
    public Sprite sprite;
    public bool held = false;
    public bool lootable;
    public int count;
    public int maxCount;
    public item_type type;
    public seed_type seedType;
    public int minGrowthTime;
    public int maxGrowthTime;
    public Sprite state1Sprite;
    public Sprite state2Sprite;
    public Sprite state3Sprite;

    public GameObject SpawnablePrefab;
    public int moneyValue;
    public int scoreValue;
}
public enum item_type
{
    SEED = 0,
    TOOL = 1,
    PRODUCT = 2
}
public enum seed_type
{
    TEST=0,
    TOMATO=1,
    CORN=2
}
