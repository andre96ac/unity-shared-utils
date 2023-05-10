using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks{

    [TaskDescription("Esegue l'animazione di spawn e restituisce Success quando viene completata")]
    public class CheckIfHasToFaceTarget : EnchantedConditional
    {


        [Header("Shared Variables")]
        [SerializeField][RequiredField] private SharedBool flipped;
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
            CheckDependencyPropMissing(targetTransform, "targetTransform");
         
        }

        public override TaskStatus OnUpdate()
        {
            if(transform.position.x > targetTransform.position.x && ! (bool)flipped.GetValue()){
                return TaskStatus.Success;
            }
            else{
                return TaskStatus.Failure;
            }
        }
    }

    
}