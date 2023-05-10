using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks{

    [TaskDescription("Esegue l'animazione di spawn e restituisce Success quando viene completata")]
    public class FaceTargetAction : Action
    {


        [Header("Shared Variables")]
        [SerializeField][RequiredField] private SharedGameObject target;

        [Space(5)]
        
        [Header("Variables")]

        private SpriteRenderer spriteRenderer;
        private Transform targetTransform;

        public override void OnAwake()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        public override void OnStart(){
            targetTransform = ((GameObject)target.GetValue())?.transform;
            if(targetTransform == null){
                Debug.LogError($"Attenzione, target non trovato nell'attività FaceTargetAction");
            }
        }

        public override TaskStatus OnUpdate()
        {
            if(transform.position.x > targetTransform.position.x && transform.eulerAngles.y == 0){
                transform.RotateAround(transform.position, transform.up, 180);
            }
            else if(transform.position.x < targetTransform.position.x && transform.eulerAngles.y == 180){
                transform.RotateAround(transform.position, transform.up, 180);
            }
            return TaskStatus.Success;
        }
    }

    
}