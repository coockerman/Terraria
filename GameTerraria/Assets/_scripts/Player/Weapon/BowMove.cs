using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BowMove : MonoBehaviour
{
    Vector2 dich;
    float moveSpeed;
    public void SetBowMove(Vector2 dich, float moveSpeed)
    {
        this.dich = dich;
        this.moveSpeed = moveSpeed;
    }
    
    private void Update()
    {
        if(dich != null)
        {
            Vector2 direction = (dich - (Vector2)transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            if (transform.position.x - dich.x <= 0.02f && transform.position.y - dich.y <=0.02f)
            {
                Destroy(gameObject);
            }
        }
    }
    
}
