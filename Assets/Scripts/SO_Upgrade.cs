using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Torban/Upgrade")]
public class SO_Upgrade : ScriptableObject
{
    public int baseCost;
    public int additionalCost;
    public string title;
    public string initialDescription;
    public string additionalDescription;
    public int totalUpgrades;
    public Upgrade_type type;

}
public enum Upgrade_type
{
    BIGGER_CANS=0,
    DEEPER_POCKETS=1,
    FASTER_SHOES=2,
    FERTILE_SOIL=3,
    NUTRIENT_SOIL=4,
    WORK_GLOVES=5,
}