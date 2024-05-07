using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float HPmax;
    [SerializeField] Slider UIHP;
    [SerializeField] SettingContro setting;
    float countHP;
    bool isDie = false;
    // Start is called before the first frame update
    void Start()
    {
        countHP = HPmax;
        UpdateUIHP();
    }

    public void ReceiveDamage(float HP)
    {
        if (isDie) return;
        countHP -= HP;
        if(countHP < 0)
        {
            countHP = 0;
            Die();
        }
        UpdateUIHP();
    }
    public void RecoverHP(float HP)
    {
        if (isDie) return;
        countHP += HP;
        if(countHP > HPmax)
        {
            countHP = HPmax;
        }
        UpdateUIHP() ;
    }
    void UpdateUIHP()
    {
        UIHP.value = countHP/HPmax;
    }
    void Die()
    {
        isDie = true;
        setting.OnBoxThua();
    }
}
