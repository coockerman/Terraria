using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeData", menuName = "SlimeData")]
public class SlimeData : ScriptableObject
{
    public string nameEnemy;
    public int maxHP;
    public float attack;
    public float speed;
}
