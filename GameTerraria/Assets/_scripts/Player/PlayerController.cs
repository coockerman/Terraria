using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int selectSlotIndex = 0;
    public GameObject hotBarSelector;

    public Inventory inventory;
    public bool isInventoryShow = false;

    public ItemClass selectedItem;
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
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal < 0)
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
        hit = Input.GetMouseButtonDown(0);
        place = Input.GetMouseButtonDown(1);

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //scroll up
            if (selectSlotIndex < inventory.inventoryWidth - 1)
                selectSlotIndex += 1;

            if (inventory.inventorySlots[selectSlotIndex, 0] != null)
                selectedItem = inventory.inventorySlots[selectSlotIndex, 0].item;
            else
                selectedItem = null;

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //scroll down
            if (selectSlotIndex > 0)
                selectSlotIndex -= 1;

            if (inventory.inventorySlots[selectSlotIndex, 0] != null)
                selectedItem = inventory.inventorySlots[selectSlotIndex, 0].item;
            else
                selectedItem = null;

        }

        hotBarSelector.transform.position = inventory.hotbarUISlot[selectSlotIndex].transform.position;
        //set selected item

        if (Input.GetKeyDown(KeyCode.E))
        {
            isInventoryShow = !isInventoryShow;
        }

        if (Vector2.Distance(transform.position, mousePosFloat) <= playerRangeMax)
        {
            if (hit)
                terrainGeneration.RemoveTile(mousePos.x, mousePos.y);
            else if (place && Vector2.Distance(transform.position, mousePosFloat) >= playerRangeMin)
                if (selectedItem != null)
                    if (selectedItem.itemType == ItemClass.ItemType.block)
                        terrainGeneration.CheckTile(selectedItem.tile, mousePos.x, mousePos.y, true);
        }

        mousePosFloat.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mousePosFloat.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);

        inventory.inventoryUI.SetActive(isInventoryShow);

        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);
    }
}
