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
    public bool isDrop = true;
    public bool isNaturallyPlace = false;

    public TileClass (TileClass tileClass, bool isNaturallyPlace)
    {
        this.tileName = tileClass.tileName;
        this.wallVariant = tileClass.wallVariant;
        this.tileSprites = tileClass.tileSprites;
        this.isImpact = tileClass.isImpact;
        this.isDrop = tileClass.isDrop;
        this.isNaturallyPlace= isNaturallyPlace;
    }
}
