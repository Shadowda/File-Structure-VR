using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
  
public class Spawner: MonoBehaviour {  
    public GameObject[] enemies;  
    public Vector3 spawnRange;  
    public float spawnWait;  
    public float spawnMostWait;  
    public float spawnLeastWait;  
    public int startWait;  
    public bool stop;  
  
    int randEnemy;  
  
    void Start() {
        //StartCoroutine(waitSpawner());
        //Reader.GetComponent<Reader>().MyFunction();
        Vector3 spawnPosition = new Vector3(0f,0f,10f);
        Instantiate(enemies[randEnemy], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

    }  
  
    void Update() {  
        //spawnWait = Random.Range(spawnMostWait, spawnMostWait);  
    }  
  
    IEnumerator waitSpawner() {  
        yield  
        return new WaitForSeconds(startWait);  
  
        while (!stop) {  
            randEnemy = Random.Range(0, 2);  
  
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange.x, spawnRange.x), 1f, Random.Range(-spawnRange.z, spawnRange.z));  
  
            Instantiate(enemies[randEnemy], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);  
  
            yield  
            return new WaitForSeconds(spawnWait);  
        }  
    }  
}  