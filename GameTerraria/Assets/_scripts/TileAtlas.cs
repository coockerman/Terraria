using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tileAtlas", menuName = "TileAtlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass grass;
    public TileClass dirt;
    public TileClass stone;
    public TileClass log;
    public TileClass leaf;

    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
