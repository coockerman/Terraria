using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour 
{
    public string nameEnemy;
    public float maxHP;
    float countHP;
    public float attack;
    public float speed;
    public bool isDie = false;
    public void Init(SlimeData slimeData)
    {
        nameEnemy = slimeData.nameEnemy;
        maxHP = slimeData.maxHP;
        countHP = maxHP;
        attack = slimeData.attack;
        speed = slimeData.speed;
    }
    public void ReceiveDamage(int damage)
    {
        if (isDie) return;
        countHP -= damage;
        if (countHP < 0)
        {
            Debug.Log(countHP);
            countHP = 0;
            Die();
        }
    }
    public void Die()
    {
        if (isDie) return;
        Debug.Log("Chet roi");
        isDie = true;
    }
}
