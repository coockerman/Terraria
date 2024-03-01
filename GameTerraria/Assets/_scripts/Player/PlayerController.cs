using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TileClass selectTile;
    public TerrainGeneration terrainGeneration;

    public float playerRangeMax;
    public float playerRangeMin;
    public Vector2Int mousePos;
    public Vector2 mousePosFloat;

    public float moveSpeed;
    public float jumpForce;
    public bool onGround;

    public bool hit;
    public bool place;

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
        place = Input.GetMouseButton(1);
        if( Vector2.Distance(transform.position, mousePosFloat) <= playerRangeMax )
        {
            if (hit)
                terrainGeneration.RemoveTile(mousePos.x, mousePos.y);
            else if (place && Vector2.Distance(transform.position, mousePosFloat) >= playerRangeMin)
                terrainGeneration.CheckTile(selectTile, mousePos.x, mousePos.y, selectTile.isImpact);
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
        mousePosFloat.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mousePosFloat.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);

        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);
    }
}
