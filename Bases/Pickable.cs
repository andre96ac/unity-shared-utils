using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Classe che espone il metodo OnPck(), notificato nel momento in cui 
/// </summary>
public abstract class Pickable : EnchantedMonobehaviour
{

    [Tooltip("Bottone per equipaggiare")]
    [SerializeField] private InputAction equipButton;
    
    // Start is called before the first frame update


    protected virtual void Awake(){
        CheckDependencyComponentMissing(GetComponent<Collider2D>(), "Collider2D");
    }

    protected virtual void OnEnable(){
        equipButton.Enable();
    }
    protected virtual void OnDisable(){
        equipButton.Disable();
    }


    private void OnTriggerStay2D(Collider2D other) {
        if(equipButton.IsPressed()){
            OnPick(other);
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if(equipButton.IsPressed()){
            OnPick(other.collider);
        }
    }

    protected abstract void OnPick(Collider2D other);
}
