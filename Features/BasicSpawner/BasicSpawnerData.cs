using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu()]
public class BasicSpawnerData : ScriptableObject
{
    public List<SpawnOnDisableOption> options;

    public GameObject getOption()
    {
        List<SpawnOnDisableOption> finalOptions = options.FindAll(el => el.timeDelay < Time.time).ToList();
        if ((finalOptions?.Count?? 0) == 0)
        {
            return null;
        }

        int offset = 0;
        (GameObject Item, int RangeTo)[] rangedItems = finalOptions
            .OrderBy(item => item.percentageWeight)
            .Select(entry => (entry.prefab, RangeTo: offset += entry.percentageWeight))
            .ToArray();

        int randomNumber = Random.Range(0, finalOptions.Sum(item => item.percentageWeight)) + 1;
        return rangedItems.First(item => randomNumber <= item.RangeTo).Item;
    }


}


[System.Serializable]
public class SpawnOnDisableOption{
    public GameObject prefab;
    public int percentageWeight;
    public float timeDelay;
}
