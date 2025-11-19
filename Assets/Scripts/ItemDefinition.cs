using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Definition", fileName = "ItemDef_")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;             // e.g., "battery", "medkit"
    public string displayName;    // "Battery"
    public Sprite icon;

    [Header("Stacking")]
    public bool stackable = true;
    public int maxStack = 99;
}
