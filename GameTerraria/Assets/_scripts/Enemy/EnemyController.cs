using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public SlimeData slimeBlue;
    public SlimeData slimeRed;
    public SlimeData slimeYellow;
    public SlimeData slimeWhite;
    public SlimeData slimeGreen;
    public SlimeData slimeBoss;
    public List<GameObject> listEnemy = new List<GameObject>();
    public GameObject enemy;

    private void Start()
    {
        AddEnemy(slimeBlue);
        AddEnemy(slimeRed);
        AddEnemy(slimeYellow);
        AddEnemy(slimeWhite);
        AddEnemy(slimeGreen);
        AddEnemy(slimeBoss);
    }
    void AddEnemy(SlimeData slime)
    {
        GameObject gojA = Instantiate(enemy);
        gojA.AddComponent<EnemyClass>();
        gojA.GetComponent<EnemyClass>().Init(slime, playerHealth);
        gojA.transform.position = RandomPosEnemy();
        listEnemy.Add(gojA);
    }
    
    Vector3 RandomPosEnemy()
    {
        int x = Random.Range(0, 100);
        return new Vector3(x, 100, 0);
    }
    
}
