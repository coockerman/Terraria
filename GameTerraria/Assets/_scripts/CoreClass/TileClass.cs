using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "TileClass")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public TileClass wallVariant;
    public Sprite[] tileSprites;
    public bool isImpact = true;
    public TileClass tileDrop;
    public ItemEnum.ToolType toolToBreak;
    public bool isNaturallyPlace = false;

    public static TileClass CreateInstance(TileClass tileClass, bool isNaturallyPlace)
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();
        thisTile.Init(tileClass, isNaturallyPlace);
        return thisTile;
    }

    public void Init (TileClass tileClass, bool isNaturallyPlace)
    {
        this.tileName = tileClass.tileName;
        this.wallVariant = tileClass.wallVariant;
        this.tileSprites = tileClass.tileSprites;
        this.isImpact = tileClass.isImpact;
        this.tileDrop = tileClass.tileDrop;
        this.toolToBreak = tileClass.toolToBreak;
        this.isNaturallyPlace= isNaturallyPlace;
    }
}
