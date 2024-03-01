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
}
