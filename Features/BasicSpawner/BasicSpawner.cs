using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawner : EnchantedMonoWithPool
{
    [SerializeField] private BasicSpawnerData spawnData;
    private void Awake(){
        CheckDependencyPropMissing(spawnData, "data");
    }


    public void spawn(){
        GameObject objToSpawn = spawnData.getOption();
        if(objToSpawn != null && !objToSpawn.Equals(null)){
            pickOrIstantiate(objToSpawn, transform.position, Quaternion.identity);    
        }

    }
}
