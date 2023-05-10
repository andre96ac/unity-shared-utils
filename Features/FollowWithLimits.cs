using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Consente a un agente di seguire costantemente la posizione di un target, ma entro un dato limite sugli assi x e y
/// </summary>
public class FollowWithLimits : EnchantedMonobehaviour
{

    [Tooltip("Il gameObject da seguire")]
    [SerializeField] private GameObject target;
    [Tooltip("Il centro da cui calcolare i limiti")]
    [SerializeField] private Transform center;
    [Tooltip("Limiti di distanza dal centro entro i quali muoversi")]
    [SerializeField] private Vector2 limits;


    private void OnEnable(){
        CheckDependencyPropMissing(target, "target");
        if(center != null){
            CheckDependencyPropMissing(limits, "limits", LogLevel.WARN);
        }
        else if(limits != null){
            CheckDependencyPropMissing(center, "center", LogLevel.WARN);
        }
       
    }

    void FixedUpdate()
    {   
        float targetX;
        float targetY;
        if(center != null && limits != null){
            targetX = Mathf.Clamp(target.transform.position.x, center.position.x - limits.x, center.position.x + limits.x);
            targetY = Mathf.Clamp(target.transform.position.y, center.position.y - limits.y, center.position.y + limits.y);
        }
        else{
            targetX = target.transform.position.x;
            targetY = target.transform.position.y;
        }

        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}
