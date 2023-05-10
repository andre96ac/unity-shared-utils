using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Questa classe gestisce un pool generico di oggetti inizializzandolo, ed espone un metodo per recuperarne uno; 
/// </summary>
public class ObjectPool : EnchantedMonobehaviour
{

    [Tooltip("Oggetti contenuti nel Pool")]
    [SerializeField] private List<GameObject> factoryPrefabs;

    [Tooltip("Numero di istanze create all'inizio per ogni oggetto")]
    [SerializeField] private int startCountPerObj;




    private List<GameObject> pool = new List<GameObject>();

    private void OnValidate(){
        CheckDependencyPropMissing(factoryPrefabs, "factoryPrefabs", LogLevel.WARN);
    }



    private void Start(){
        createPool();
    }


    /// <summary>
    /// Riempie la pool all'inizio
    /// </summary>
    private void createPool(){
        factoryPrefabs.ForEach(el => {
            for(int i = 0; i < startCountPerObj; i++){
                GameObject newObj = Instantiate(el);
                newObj.SetActive(false);
                newObj.transform.parent = this.transform;
                newObj.name = newObj.name.Replace("(Clone)", "");
                pool.Add(newObj);
            }
        });
    }



    /// <summary>
    /// Spawna un oggetto della pool nella posizione  e rotazione specificata 
    /// (Se non ci sono oggetti disponibili, un nuovo oggetto verrà creato)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject spawnByName(string name, Vector3 position, Quaternion rotation){
        GameObject retGameObject = pool.Find(el=> (el.name == name && !el.activeInHierarchy));

        if(retGameObject == null){
            GameObject factory = factoryPrefabs.Find(el => el.name == name);
            if(factory == null){
                Debug.LogError($"Errore, si sta cercando di creare un oggetto non configurato nella pool {gameObject.name}");
            }
            else{
                retGameObject = Instantiate(factory);
                retGameObject.transform.parent = transform;
                retGameObject.name = retGameObject.name.Replace("(Clone)", "");
                pool.Add(retGameObject);
            }
        }


        retGameObject.SetActive(true);
        retGameObject.transform.position = position;
        retGameObject.transform.rotation = rotation;


        return retGameObject;

    }


}
