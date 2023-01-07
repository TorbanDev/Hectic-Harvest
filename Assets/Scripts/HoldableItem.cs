using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HoldableItem : MonoBehaviour
{
    public string itemName;
    public SO_Item item;
    public abstract void Use();

}
