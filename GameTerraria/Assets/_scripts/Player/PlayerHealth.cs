using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float HPmax;
    float countHP;
    bool isDie = false;
    // Start is called before the first frame update
    void Start()
    {
        countHP = HPmax;
    }

    public void ReceiveDamage(float HP)
    {
        if (isDie) return;
        Debug.Log("aaa");
        countHP -= HP;
        if(countHP < 0)
        {
            countHP = 0;
            Die();
        }
    }
    public void RecoverHP(float HP)
    {
        if (isDie) return;
        countHP += HP;
        if(countHP > HPmax)
        {
            countHP = HPmax;
        }
    }
    void Die()
    {
        Debug.Log("ua");
        isDie = true;
    }
}
