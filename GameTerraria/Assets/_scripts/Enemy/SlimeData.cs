using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeData", menuName = "SlimeData")]
public class SlimeData : ScriptableObject
{
    public LayerMask layerPlayer;
    public string nameEnemy;
    public int maxHP;
    public float attack;
    public float speed;
    public float attackSpeed;
}
