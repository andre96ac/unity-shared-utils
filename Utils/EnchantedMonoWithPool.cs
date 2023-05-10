using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Questa classe aggiunge la proprietà di un pool ad un oggetto EnchantedMonobehavior, ed espone il metodo "PickOrIstantiate()"; 
/// </summary>
public class EnchantedMonoWithPool : EnchantedMonobehaviour
{
    [SerializeField] protected string poolName;

    private ObjectPool poolScript;


    protected virtual void Start(){
        CheckDependencyPropMissing(poolName, "poolName", LogLevel.INFO);
        if(!string.IsNullOrEmpty(poolName)){
            poolScript = GameObject.Find(poolName)?.GetComponent<ObjectPool>();
            if(poolScript == null || poolScript.Equals(null)){
                Debug.LogWarning($"Attenzione: Object Pool {poolName} non trovato dal gameObject {gameObject.name}, gli oggetti verranno spawnnati manualmente");
            }
        }
    }





    protected GameObject pickOrIstantiate(GameObject objToIstantiate, Vector3 position, Quaternion rotation){
    if(poolScript != null){
        return poolScript.spawnByName(objToIstantiate.name, position, rotation);
    }
    else{
        Debug.Log($"Pool {poolName} non trovato, oggetto {objToIstantiate.name} istanziato manualmente");
        return GameObject.Instantiate(objToIstantiate, position, rotation);
    }
}

}