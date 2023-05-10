// This script moves a game object left, right, forwards, backwards...
// using input from keyboard/gamepad (set in the Input Manager)
// 'Update' Method is used for the Input (keyboard/Gamepad)
// 'Fixed' Method is used for physics movement
// The Input is 'Normalized' to prevent faster diagonal movement
// 'Time.fixedDeltaTime' is used to keep the physics framrate consistant on all devices
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class MoveByController : EnchantedMonobehaviour
{

    public bool isFlipped { get; private set; }
    [SerializeField] private InputAction moveControls;


    public bool isMoving {get => movement.x != 0 || movement.y != 0;} 
    public float speed = 100f; // Speed variable
    
    
    private Rigidbody2D _rigidbody2D; // Set the variable 'rb' as Rigibody
    private Vector2 movement; // Set the variable 'movement' as a Vector3 (x,y,z)


 
    // 'Start' Method run once at start for initialisation purposes
    void Awake()
    {
        // find the Rigidbody of this game object and add it to the variable 'rb'
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        CheckDependencyComponentMissing(_rigidbody2D, "RigidBody2D");
       
    }

 

    private void OnEnable() {
        moveControls.Enable();  
    }
    private void OnDisable() {
        moveControls.Disable();    
    }
 
 
 
    // 'Update' Method is called once per frame
    void Update()
    {
        // In Update we get the Input for left, right, up and down and put it in the variable 'movement'...
        // We only get the input of x and z, y is left at 0 as it's not required
        // 'Normalized' diagonals to prevent faster movement when two inputs are used together
        // movement = new Vector2(Input.GetAxis("Horizontal") ,Input.GetAxis("Vertical")).normalized;
        movement = moveControls.ReadValue<Vector2>();
    }
 
 
 
    // 'FixedUpdate' Method is used for Physics movements
    void FixedUpdate()
    {
        // We call the function 'moveCharacter' in FixedUpdate for Physics movement
        moveCharacter(movement); 
    }
 
 
 
    // 'moveCharacter' Function for moving the game object
    void moveCharacter(Vector2 direction)
    {
        if(direction.x < 0 && transform.eulerAngles.y == 0){
            transform.RotateAround(transform.position, transform.up, 180);
            isFlipped = true;
        }
        else if(direction.x > 0 && transform.eulerAngles.y == 180){
            transform.RotateAround(transform.position, transform.up, 180);
            isFlipped = false;
        }
        // We multiply the 'speed' variable to the Rigidbody's velocity...
        // and also multiply 'Time.fixedDeltaTime' to keep the movement consistant on all devices
        _rigidbody2D.velocity = direction * speed * Time.fixedDeltaTime;
    }




 
}