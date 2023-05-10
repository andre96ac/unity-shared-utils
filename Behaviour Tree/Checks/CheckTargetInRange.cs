using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


namespace BehaviorDesigner.Runtime.Tasks{


[TaskDescription("Controlla se è presente l'elemento target all'interno del raggio specificato")]
public class CheckTargetInRange : Conditional
{

    [Header("Shared Variables")]
    [RequiredField][SerializeField] SharedGameObject target;

    [Space(5)]

    [Header(" Variables")]
    [SerializeField] private float range = 3f;

    public override void OnStart(){
        if(target == null){
            Debug.LogError("Errore: target non trovato nell'attività CheckTargetInRange");
        }
    }


    public override TaskStatus OnUpdate()
    {           
            List<Collider2D> foundColliders;

           
            foundColliders = Physics2D.OverlapCircleAll(transform.position, range).ToList();
            

            Collider2D foundCollider = foundColliders.Find(el => el.gameObject.name == ((GameObject)target.GetValue()).name);


            
            return foundCollider != null? TaskStatus.Success : TaskStatus.Failure;
            
    }


}
}