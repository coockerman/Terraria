using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public SlimeData[] slimes;
    
    public List<GameObject> listEnemy = new List<GameObject>();
    public GameObject enemy;

    public GameObject tileDrop;

    private void Start()
    {
        RenderSlime();
        StartCoroutine(RandomInGame());
    }
    void RenderSlime()
    {
        foreach(SlimeData slime in slimes)
        {
            AddEnemy(slime);
        }
    }
    void AddEnemy(SlimeData slime)
    {
        GameObject gojA = Instantiate(enemy);
        gojA.AddComponent<EnemyClass>();
        gojA.GetComponent<EnemyClass>().Init(slime, playerHealth, tileDrop);
        gojA.transform.position = RandomPosEnemy();
        listEnemy.Add(gojA);
    }
    IEnumerator RandomInGame()
    {
        while(true)
        {
            float t = Random.Range(3, 10);
            yield return new WaitForSeconds(t);

            int b = Random.Range(0, slimes.Length - 2);
            AddEnemy(slimes[b]);
        }
    }
    Vector3 RandomPosEnemy()
    {
        int x = Random.Range(20, 80);
        return new Vector3(x, 80, 0);
    }
    
}
