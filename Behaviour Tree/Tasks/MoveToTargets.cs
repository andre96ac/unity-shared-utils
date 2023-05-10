using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace BehaviorDesigner.Runtime.Tasks{
    

[TaskDescription("Navigra seguendo il percorso tra i waypoints dall'inizio alla fine; restituisce Success quando l'agente è arrivato a destinazione")]

public class MoveToTargets : Action
{

    [Header("Shared Variables")]
    [SerializeField][RequiredField] private SharedString idleAnimationName;
    [SerializeField] private List<SharedGameObject> waypointsGameObject;
    [SerializeField] private List<SharedTransform> waypointsPositions;

    [Space(5)]

    [Header("Variables")]
    [SerializeField] private float waitTime = 1f; 
    [SerializeField] private float speed = 1f;
    [SerializeField] private string movingAnimationName;
    [Tooltip("Ritorna failure se l'agente si blocca")]
    // [SerializeField] private bool throwFailure = false;



    private int _currentWaypointIndex;
    private float _waitCounter;
    private bool _waiting;
    private Animator _myAnimator;
    private List<Vector3> _waypointsPositions;



    public override void OnAwake(){
        _myAnimator = gameObject.GetComponent<Animator>();
        if(_myAnimator == null){
            Debug.LogWarning($"Attenzione, componente animator non trovato nel GameObject {gameObject.name}; le animazioni non funzioneranno");
        }
        if(string.IsNullOrEmpty((string)idleAnimationName.GetValue()) || string.IsNullOrEmpty(movingAnimationName) || _myAnimator == null){
            Debug.LogWarning("Attenzione, parametri di animazione non definiti per l'azone MoveToTarget, le animazioni potrebbero non funzionare");
        }
    }

    public override void OnStart(){


        if(waypointsGameObject.Count > 0){
            _waypointsPositions = waypointsGameObject
                                        .Select(el => ((GameObject)el.GetValue()).transform.position)
                                        .ToList();
        }
        else if(waypointsPositions.Count > 0){
            _waypointsPositions = waypointsPositions
                                        .Select(el => (Vector3)el.GetValue())
                                        .ToList();
        }
        else{
            Debug.LogError($"Errore, target non definiti per l'azione MoveToTarget nel GameObject {gameObject.name}");
        }

        _currentWaypointIndex = 0;
        _waitCounter = 0f;
        _waiting = false;
        

        _myAnimator.Play(movingAnimationName);
    }



    public override TaskStatus OnUpdate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= waitTime)
            {
                _waiting = false;
                _myAnimator.Play(movingAnimationName);

            }
        }
        else
        {

            Vector3 target = _waypointsPositions[_currentWaypointIndex];
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {


                transform.position = target;
                _waitCounter = 0f;
                _waiting = true;
                _myAnimator.Play((string)idleAnimationName.GetValue());



                _currentWaypointIndex ++;
                if(_currentWaypointIndex == _waypointsPositions.Count){
                    return TaskStatus.Success;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );


                
            }
        }
    
        return  TaskStatus.Running;
    }

    public override void OnEnd(){
        _myAnimator.Play((string)idleAnimationName.GetValue());
    }


   

}
}