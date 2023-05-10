using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAnimatorControllerStateOnDisable : EnchantedMonobehaviour
{
    
    private void Awake() {
        GetComponent<Animator>().keepAnimatorStateOnDisable = true;
    }
}
