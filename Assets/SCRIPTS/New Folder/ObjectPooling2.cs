using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling2 : MonoBehaviour
{
    [SerializeField] private PoolStruct[] objects;
    
    [SerializeField] private List<Transform> spawns;
    [SerializeField] private List<Transform> usedSpawns;

    [SerializeField] private int maxObjInScene;
    [SerializeField] private float spawnRate;

    private List<GameObject> pool = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Awake()
    {       
        foreach (PoolStruct obj in objects)
        {
            for (int i = 0; i < obj.cantidad; i++)
            {
                GameObject instance = Instantiate(obj.prefab, transform);
                instance.SetActive(false);
                pool.Add(instance);
            }
        }

        spawnCoroutine = StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            if (usedSpawns.Count >= maxObjInScene || spawns.Count == 0)
                continue;


            GameObject obj = GetObjectByChance();
            if (obj == null) continue;

            Transform spawn = GetRandomSpawn();
            obj.transform.position = spawn.position;

           
            BuffIman scriptIman = obj.GetComponent<BuffIman>();
            if (scriptIman != null)
            {
                scriptIman.poolManager = this;
                scriptIman.currentSpawn = spawn;
            }

            
            BuffSalto scriptSalto = obj.GetComponent<BuffSalto>();
            if (scriptSalto != null)
            {
                scriptSalto.poolManager = this;
                scriptSalto.currentSpawn = spawn;
            }

            Collectible scriptCollectible = obj.GetComponent<Collectible>();
            if (scriptCollectible != null)
            {
                scriptCollectible.poolManager = this;
                scriptCollectible.currentSpawn = spawn;
            }

            obj.SetActive(true);
        }
    }
    
    private GameObject GetObjectByChance()
    {
        float randomValue = Random.Range(0f, 100f);

        foreach (PoolStruct obj in objects)
        {
            if (randomValue >= obj.chanceMin && randomValue < obj.chanceMax)
            {
                foreach (GameObject pooled in pool)
                {
                    if (!pooled.activeInHierarchy &&
                        pooled.name.Contains(obj.prefab.name))
                    {
                        return pooled;
                    }
                }
            }
        }

        return null;
    }

    private Transform GetRandomSpawn()
    {
        int index = Random.Range(0, spawns.Count);
        Transform spawn = spawns[index];
        spawns.RemoveAt(index);
        usedSpawns.Add(spawn);
        return spawn;
    }

    public void ReturnToList(Transform spawn, GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);

        
        if (spawn != null && usedSpawns.Contains(spawn))
        {
            usedSpawns.Remove(spawn);
            if (!spawns.Contains(spawn))
            {
                spawns.Add(spawn);
            }
        }
    }
}
