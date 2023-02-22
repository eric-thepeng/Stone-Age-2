using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "ScriptableObjects/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    public string name;
    public enum Category {RawMaterial, CraftMaterial, Food,  Furniture, Object , Tool}
    public Sprite inventoryIcon { get { return iso.iconSprite; } }
    public Category category;
    public ItemScriptableObject iso;
}
