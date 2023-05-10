using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//permette all'agente di causare danno agli oggetti con cui entra in contatto che possiedono un componente "Damageable"
public class DamageOnContact : EnchantedMonobehaviour
{
    public enum CollisionDetectionMode {OnEnter, Continuous}


    [Tooltip("Danno provocato al contatto")]
    [SerializeField] private float damage;
    [Tooltip("Knockback provocato al contatto")]
    [SerializeField] private float pushForce;
    [Tooltip("Origine della forza applicata per il KnockBack; se non impostata, verrà utilizzato il transform del gameobject")]
    [SerializeField] private Transform customDamageSourcePoint;
    [Tooltip("Layers da escludere nella rilevazione delle collisioni")]
    [SerializeField] private LayerMask excludeLayers;
    [Tooltip("Modalità di rilevamento delle collisioni: solo al contatto, o continua")]
    [SerializeField] private CollisionDetectionMode detectionMode = CollisionDetectionMode.OnEnter;




    


    private void Awake() {

        CheckDependencyComponentMissing(gameObject.GetComponent<Collider2D>(), "Collider2D", LogLevel.WARN);
    }

    void Update(){

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(detectionMode == CollisionDetectionMode.OnEnter){
            setDamage(other);
        }

    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(detectionMode == CollisionDetectionMode.OnEnter){
            setDamage(other.collider);
        }
        
    }
    private void OnCollisionStay2D(Collision2D other) {
        if(detectionMode == CollisionDetectionMode.Continuous){
            setDamage(other.collider);
        }

    }
    private void OntriggerStay2D(Collider2D other) {
        if(detectionMode == CollisionDetectionMode.Continuous){
            setDamage(other);
        }
        
    }


    /// <summary>
    /// Controlla che l'oggetto non possieda un layer tra quelli esclusi, recupera il componente Damageable dell'oggetto, e se lo
    /// trova, invia il danno all'oggetto
    /// </summary>
    /// <param name="collider"></param>
    private void setDamage(Collider2D collider){


        if(!IsLayerInLayermask(collider.gameObject.layer, excludeLayers)){
            Damageable damageableScript = collider.gameObject.GetComponent<Damageable>();

            var damageOrigin = customDamageSourcePoint != null? customDamageSourcePoint.position : transform.position;

            if(damageableScript != null){
                DamageModel dmgData = new DamageModel{
                damageAmount = damage,
                origin = damageOrigin,
                pushForce = pushForce

                };
                
                damageableScript.reciveDamage(dmgData);

            }
        }

    }

}
