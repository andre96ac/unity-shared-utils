using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks{

[TaskDescription(   "Interroga la proprietà currentHealth del componente damageable e controlla se è morto (vita <= 0)")]
    public class CheckIfDead : EnchantedConditional
    {
        private Damageable myDamageableScript;

        public override void OnAwake(){
            myDamageableScript = gameObject.GetComponent<Damageable>();
            CheckDependencyComponentMissing(myDamageableScript, "myDamageableScript");
        }

        public override TaskStatus OnUpdate()
        {
            return myDamageableScript.readOnlyCurrentLife <= 0? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
