using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
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
        gojA.GetComponent<EnemyClass>().Init(slimeA);
        listEnemy.Add(gojA);
    }
    
    
    
}
