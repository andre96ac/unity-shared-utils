using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedSpawner : EnchantedMonoWithPool
{
    [SerializeField] private string spawnPointsName = "SpawnPoint"; 
    [SerializeField] private AdvancedSpawnerData spawnerData;
    
    private float lastSpawnTime;
    private List<Transform> spawnPoints = new  List<Transform>();

    protected void OnValidate(){
        CheckDependencyPropMissing(spawnerData, "spawnerData");
    }
    
    
    protected override void Start()
    {
        base.Start();
        foreach(Transform child in transform){
            if(child.name == spawnPointsName){
                spawnPoints.Add(child);
            }
        }

        if(spawnPoints.Count <= 0){
            Debug.LogWarning($"Attenzione, nessun figlio con nome {spawnPointsName} trovato nel gameObject {gameObject.name}");
        }

        spawn();
        
        
    }

    void Update()
    {
        if(Time.time > lastSpawnTime + spawnerData.spawnInterval){
            spawn();
        }
    }

    private void spawn(){
        lastSpawnTime = Time.time;
        GameObject objToSpawn = spawnerData.getOption();
        Transform transformWhereSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        pickOrIstantiate(objToSpawn, transformWhereSpawn.position, Quaternion.identity); 
    }
}
