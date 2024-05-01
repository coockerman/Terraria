using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    ItemClass itemEnemy;
    GameObject tileDrop;
    public LayerMask layerPlayer;
    public string nameEnemy;
    public float maxHP;
    float countHP;
    public float attack;
    public float speed;
    public bool isDie = false;
    public float DelayAttack;
    float countDelayAttack;

    float moveChangeTime;
    float countMove;
    int statusMove = 0;
    private void Start()
    {
        countDelayAttack = DelayAttack;
        moveChangeTime = Random.Range(2, 4);
        countMove = moveChangeTime;
    }
    private void Update()
    {
        if (isDie) return;
        XuLyAttack();
        AutoMove();
    }
    void XuLyAttack()
    {
        if (countDelayAttack > 0)
        {
            countDelayAttack -= Time.deltaTime;
        }
        if (RightCheck() || LeftCheck())
        {
            if (countDelayAttack <= 0)
            {
                Attack();
            }
        }

    }
    void AutoMove()
    {
        Move(statusMove);
        countMove -= Time.deltaTime;
        if (countMove <= 0)
        {
            if (statusMove == 0)
                statusMove = 1;
            else
                statusMove = 0;
            countMove = moveChangeTime;
        }
    }
    void Move(int i)
    {
        if (i == 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            Vector3 variable = new Vector3(1, 0, 0);
            transform.Translate(variable * speed * Time.deltaTime);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            Vector3 variable = new Vector3(-1, 0, 0);
            transform.Translate(variable * speed * Time.deltaTime);
        }
    }
    void Attack()
    {
        playerHealth.ReceiveDamage(attack);
        countDelayAttack = DelayAttack;
    }
    public void Init(SlimeData slimeData, PlayerHealth playerHealth, GameObject tileDrop)
    {
        this.playerHealth = playerHealth;
        gameObject.GetComponent<SpriteRenderer>().sprite = slimeData.sprite;
        itemEnemy = new ItemClass(slimeData.vatpham);
        layerPlayer = slimeData.layerPlayer;
        nameEnemy = slimeData.nameEnemy;
        maxHP = slimeData.maxHP;
        countHP = maxHP;
        attack = slimeData.attack;
        speed = slimeData.speed;
        DelayAttack = slimeData.attackSpeed;
        this.tileDrop = tileDrop;
    }
    public void ReceiveDamage(int damage)
    {
        if (isDie) return;
        countHP -= damage;
        Debug.Log(countHP);

        if (countHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        if (isDie) return;
        isDie = true;
        if(itemEnemy != null)
        {
            GameObject newTileDrop = Instantiate(tileDrop, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
            newTileDrop.GetComponent<SpriteRenderer>().sprite = itemEnemy.tile.tileSprites[0];
            ItemClass tileDropItem = new ItemClass(itemEnemy.tile);
            newTileDrop.GetComponent<TileCropController>().item = tileDropItem;
        }
        gameObject.SetActive(false);
    }
    bool RightCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.right * transform.localScale.x, 2f, layerPlayer);
        return hit;
    }
    bool LeftCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left * transform.localScale.x, 2f, layerPlayer);
        return hit;
    }
}
