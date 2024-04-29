using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public SlimeData slimeA;
    public List<GameObject> listEnemy = new List<GameObject>();
    public GameObject enemy;

    private void Start()
    {
        AddEnemy();
        AddEnemy();
    }
    void AddEnemy()
    {
        GameObject gojA = Instantiate(enemy);
        gojA.AddComponent<EnemyClass>();
        gojA.GetComponent<EnemyClass>().Init(slimeA, playerHealth);
        gojA.transform.position = RandomPosEnemy();
        listEnemy.Add(gojA);
    }
    
    Vector3 RandomPosEnemy()
    {
        int x = Random.Range(0, 100);
        return new Vector3(x, 100, 0);
    }
    
}
