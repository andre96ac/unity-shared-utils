using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


namespace BehaviorDesigner.Runtime.Tasks{


[TaskDescription("Controlla se il target si trova sotto il trasform dell'agente")]
public class CheckTargetUnder : Conditional
{

    [Header("Shared Variables")]
    [RequiredField][SerializeField] SharedGameObject target;

    [Space(5)]

    [Header(" Variables")]
    [SerializeField] private float offset = 0;



    private Transform targetTransform;

    public override void OnStart(){

        if(target == null){
            Debug.LogError("Errore: target non trovato nell'attività CheckTargetInRange");
        }
        targetTransform = ((GameObject)target.GetValue()).transform;
    }


    public override TaskStatus OnUpdate()
    {      
            return targetTransform.position.y + offset < gameObject.transform.position.y? TaskStatus.Success : TaskStatus.Failure;
    }


}
}