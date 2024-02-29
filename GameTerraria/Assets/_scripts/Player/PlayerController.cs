using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TerrainGeneration terrainGeneration;

    public Vector2Int mousePos;

    public float moveSpeed;
    public float jumpForce;
    public bool onGround;

    public bool hit;

    private Rigidbody2D rb;
    private Animator anim;
    float horizontal;

    [HideInInspector]
    public Vector2 spawnPos { get; set; }
    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        hit = Input.GetMouseButton(0);
        if(hit)
        {
            terrainGeneration.RemoveTile(mousePos.x, mousePos.y);
        }

        if (horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if(horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (vertical > 0.1f || jump > 0.1f)
        {
            if (onGround)
                movement.y = jumpForce;
        }

        rb.velocity = movement;
    }
    private void Update()
    {
        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit);
    }
}
