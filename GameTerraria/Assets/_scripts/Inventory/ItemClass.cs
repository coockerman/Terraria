using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public enum ItemType
    {
        block,
        tool
    }
    public enum ToolType
    {
        axe,
        pickage,
        hammer
    }

    public ItemType itemType;
    public ToolType toolType;

    public TileClass tile;
    public ToolClass tool;


    public string nameTool;
    public Sprite sprite;
    public bool isStackable;

    public ItemClass(TileClass _tile)
    {
        nameTool = _tile.tileName;
        sprite = _tile.tileSprites[0];
        isStackable = _tile.isImpact;
        itemType = ItemType.block;
        tile = _tile;
    }
    public ItemClass(ToolClass _tool)
    {
        nameTool = _tool.nameTool;
        sprite = _tool.sprite;
        isStackable = false;
        toolType = _tool.toolType;
        tool = _tool;
    }
}
