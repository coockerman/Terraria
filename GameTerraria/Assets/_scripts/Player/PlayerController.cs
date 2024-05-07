using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask layerMask;

    public int selectSlotIndex = 0;
    public GameObject hotBarSelector;
    public GameObject handHolder;

    public Inventory inventory;
    public EnemyController enemyController;
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
    float vertical;

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
        vertical = Input.GetAxis("Vertical");

        float jump = Input.GetAxis("Jump");

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

        //if (FootRayCast() && !HeadRayCast() && movement.x != 0)
        //{
        //    if (onGround)
        //        movement.y = jumpForce;
        //}

        rb.velocity = movement;
    }



    private void Update()
    {
        hit = Input.GetMouseButtonDown(0);
        place = Input.GetMouseButtonDown(1);

        SelectedItemScroll();
        GetMousePos();
        ActiveBag();
        SetAnim();
    }



    public void SelectedItemScroll()
    {
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

        }
        UpdateSelectedItem();

        hotBarSelector.transform.position = inventory.hotbarUISlot[selectSlotIndex].transform.position;
        //set selected item
        if(hit)
        {
            if (selectedItem != null)
            {
                if (selectedItem.weaponType == ItemEnum.WeaponType.sword)
                {
                    if(Vector2.Distance(transform.position, mousePosFloat) <= selectedItem.phamvi)
                    {
                        FindAttackEnemy(false);
                    }
                }
                if (selectedItem.weaponType == ItemEnum.WeaponType.bow)
                {
                    if (Vector2.Distance(transform.position, mousePosFloat) <= selectedItem.phamvi)
                    {
                        FindAttackEnemy(true);
                    }
                }
            }
        }
        
        
        if (Vector2.Distance(transform.position, mousePosFloat) <= playerRangeMax)
        {
            if (hit)
            {
                if(selectedItem != null)
                {
                    terrainGeneration.BreakTile(mousePos.x, mousePos.y, selectedItem);
                }

            }
            else if (place && Vector2.Distance(transform.position, mousePosFloat) >= playerRangeMin)
            {
                if (selectedItem != null)
                {
                    if (selectedItem.itemType == ItemEnum.ItemType.block)
                    {
                        if (terrainGeneration.CheckTile(selectedItem.tile, mousePos.x, mousePos.y, selectedItem.tile.isNaturallyPlace))
                            inventory.RemoveItem(selectedItem);
                    }else if(selectedItem.itemType == ItemEnum.ItemType.medicine)
                    {
                        gameObject.GetComponent<PlayerHealth>().RecoverHP(selectedItem.hpRecover);
                        inventory.RemoveItem(selectedItem);
                    }
                }
            }
        }
    }
    public void UpdateSelectedItem()
    {
        if (inventory.inventorySlots[selectSlotIndex, 0] != null)
        {
            selectedItem = inventory.inventorySlots[selectSlotIndex, 0].item;
            hotBarSelector.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = selectedItem.nameTool;
        }
        else
        {
            selectedItem = new ItemClass();
            selectedItem = null;
            hotBarSelector.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        }
        //cap nhat sprite
        if (selectedItem != null)
        {
            handHolder.GetComponent<SpriteRenderer>().sprite = selectedItem.sprite;
            if (selectedItem.itemType == ItemEnum.ItemType.block)
            {
                handHolder.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                handHolder.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else if (selectedItem == null)
        {
            handHolder.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    void FindAttackEnemy(bool isBow)
    {
        foreach (GameObject enemy in enemyController.listEnemy)
        {
            if(enemy != null)
            {
                if (!enemy.GetComponent<EnemyClass>().isDie)
                {
                    float x = enemy.transform.position.x;
                    float y = enemy.transform.position.y;
                    if (Mathf.Abs(mousePosFloat.x - x) <= 1 && Mathf.Abs(mousePosFloat.y - y) <= 1)
                    {
                        enemy.GetComponent<EnemyClass>().ReceiveDamage(selectedItem.weapon.dame);
                        if (isBow)
                        {
                            GameObject bow = Instantiate(selectedItem.weapon.bow, transform.position, Quaternion.identity);
                            bow.GetComponent<BowMove>().SetBowMove(new Vector2(x, y), 10);

                        }
                    }
                }
            }
        }
    }
    
    void ActiveBag()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInventoryShow = !isInventoryShow;
        }
        inventory.inventoryUI.SetActive(isInventoryShow);
    }
    void GetMousePos()
    {
        mousePosFloat.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mousePosFloat.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
    }
    void SetAnim()
    {
        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);
    }
    bool FootRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), -Vector2.right * transform.localScale.x, 1f, layerMask);
        return hit;
    }
    bool HeadRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * 0.5f), -Vector2.right * transform.localScale.x, 1f, layerMask);
        return hit;
    }
}
