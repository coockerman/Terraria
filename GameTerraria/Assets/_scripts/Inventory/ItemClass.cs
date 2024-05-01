using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public ItemEnum.ItemType itemType;
    public ItemEnum.ToolType toolType;
    public ItemEnum.WeaponType weaponType;

    public TileClass tile;
    public ToolClass tool;
    public WeaponClass weapon;

    public string nameTool;
    public Sprite sprite;
    public bool isImpact;
    public ItemClass()
    {
        nameTool = "";
        sprite = null;
        isImpact = false;
        tile = null;
        tool = null;
        weapon = null;
    }

    public ItemClass(TileClass _tile)
    {
        nameTool = _tile.tileName;
        sprite = _tile.tileDrop.tileSprites[0];
        isImpact = _tile.isImpact;
        itemType = _tile.type;
        tile = _tile;
    }
    public ItemClass(ToolClass _tool)
    {
        nameTool = _tool.nameTool;
        sprite = _tool.sprite;
        isImpact = false;
        itemType = ItemEnum.ItemType.tool;
        toolType = _tool.toolType;
        tool = _tool;
    }
    public ItemClass(WeaponClass _weapon)
    {
        nameTool = _weapon.nameWeapon;
        sprite = _weapon.sprite;
        isImpact = false;
        itemType = ItemEnum.ItemType.weapon;
        weaponType = _weapon.weaponType;
        weapon = _weapon;
    }
}
