using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using BehaviorDesigner.Runtime.Tasks;
using System.Linq;


namespace BehaviorDesigner.Runtime.Tasks{

    [TaskDescription(   "Fa seguire all'agente il transform specificato, per il tempo specificato, alla velocità indicata" +
                        "Restitusce Success quando il tempo è scaduto")]
    public class FollowTargetUntilTime : EnchantedAction
    {

        public enum MovingBehavior {
            SEEK, //Inseguimento base del target
            FLEE, //Fuga base del target
            PURSUE, //Insegumento target predicendo la posizione
            EVADE //Fuga dal target predicendo la posizione
        }

        [Header("Shared Variables")]
        [SerializeField] private SharedGameObject targetGameObject;
        [SerializeField] private SharedTransform targetTransform; 
        [SerializeField] private SharedString idleAnimationName;        
        [Space(5)]

        [Header("Variables")]
        [Tooltip("Tempo di escuzione; impostare a 0 per un'esecuzione infinita")]
        [SerializeField] private float executionTime = 0f;
        [Tooltip("Velocità di inseguimento")]
        [SerializeField] private float speed;
        [Tooltip("Animazione di movimento")]
        [SerializeField] private string movingAnimationName;
        [Tooltip("Tipologia di comportamento: SEEK => Insegue il nemico, FLEE => Fugge dal nemico, PURSUE => Insegue il nemico predicendone gli spostamenti, EVADE => Fugge dal nemico predicendone gli spostamenti")]
        [SerializeField] private MovingBehavior behaviorType = MovingBehavior.SEEK;
        [Tooltip("Evita le collisioni con gli ostacoli")]
        [SerializeField] private bool tryAvoidObstacles;
        [Tooltip("Layer mask di filtro per le collisioni con gli ostacoli")]
        [SerializeField] private LayerMask obstaclesLayerMask;
        [Tooltip("Utilizza il componente rigidBody2D anzichè il transform per muovere l'agente")]
        [SerializeField] private bool useRigidBody;
        [Tooltip("Flippa il transform quando necessario per guardare il target. E' NECESSARIO IMPOSTARE LA VARIABILE FLIPPED")]
        [SerializeField] private bool faceMovingDirection;

        [Space(5)]

        [Header("Debug")]
        [SerializeField] private bool showGizmos;
        


        private float startTime;
        private Animator _myAnimator;
        private Transform finalTarget;
        private Rigidbody2D _myRigidBody;

        //Avoid debounce flip mechanism
        private float debounceInterval = 0.2f;
        private float lastTimeRotated;
        // private bool paused = false;





        

        // public override void OnPause(bool paused){
        //     if(!paused){
        //         _myAnimator.Play(movingAnimationName);
        //     }
        // }
    
        public override void OnAwake(){
            _myAnimator = gameObject.GetComponent<Animator>();

            if(useRigidBody){
                _myRigidBody = gameObject.GetComponent<Rigidbody2D>();
                CheckDependencyComponentMissing(_myRigidBody, "RigidBody2D", LogLevel.INFO);

            }

            CheckDependencyComponentMissing(_myAnimator, "Animator", LogLevel.WARN);
            CheckDependencyPropMissing((string)idleAnimationName.GetValue(), "idleAnimationName", LogLevel.WARN);
            CheckDependencyPropMissing(movingAnimationName, "movingAnimationName", LogLevel.WARN);
        }

        public override void OnStart(){
            startTime = Time.time;
            if(_myAnimator != null){
                _myAnimator.Play(movingAnimationName);
            }
            if(targetGameObject.GetValue() != null){
                finalTarget = ((GameObject)targetGameObject.GetValue()).transform;
            }
            else if (targetTransform.GetValue() != null){
                finalTarget = (Transform)targetTransform.GetValue();
            }
            else{
                Debug.LogError($"Errore, target non definti nell'azione FollowTargetUntilTime del GameObject {gameObject.name}");
            }
        }

        public override TaskStatus OnUpdate(){
                if((Time.time < startTime + executionTime) || executionTime == 0){
                    return TaskStatus.Running;
                }
                if(_myAnimator != null && !string.IsNullOrEmpty((string)idleAnimationName.GetValue())){
                    _myAnimator.Play((string)idleAnimationName.GetValue());
                }
                return TaskStatus.Success;
        }



        public override void OnFixedUpdate()
        {
            //Modalità 
            // Vector2 finalDirection;
            // if(Time.time > lastTimeChangedDirection + debounceInterval){
            //     finalDirection = calcFinaldirection();
            //     previousDirection = finalDirection;
            //     lastTimeChangedDirection = Time.time;
            // }
            // else{
            //     finalDirection = previousDirection;
            // }

            Vector2 finalDirection = calcFinaldirection();

            if(faceMovingDirection){
                flipInDirection(finalDirection);
            }

            if(useRigidBody && _myRigidBody != null){

                //Muovo con il rigidbody
                var finalVelocity = finalDirection.normalized * speed * Time.fixedDeltaTime;
                // _myRigidBody.velocity = finalVelocity;
                // _myRigidBody.velocity = finalDirection.normalized * speed * Time.fixedDeltaTime;
                _myRigidBody.AddForce(finalVelocity * 100, ForceMode2D.Force);
               
            }
            else{
                //Muovo  con il transform
                transform.position = (Vector2)transform.position + (finalDirection.normalized * speed * Time.fixedDeltaTime);
            }  
        }


        private Vector2 calcFinaldirection(){


            Vector2 retDirection;       

            switch(behaviorType){
                case MovingBehavior.SEEK:
                    retDirection = getDirectionSeek();
                break;
                case MovingBehavior.FLEE:
                    retDirection = getDirectionFlee();
                break;
                case MovingBehavior.PURSUE:
                    retDirection = getDirectionPursue();
                break;
                case MovingBehavior.EVADE:
                    retDirection = getDirectionEvade();
                break;
                default:
                    retDirection = Vector2.zero;
                    Debug.LogError("Errore: tipologia di comportamento nell'attività FollowTargetUntiltime non specificato");
                break;
            }

            if(tryAvoidObstacles){
                retDirection = getAvoidObstacleDirection(retDirection);
            }

            if(showGizmos && !tryAvoidObstacles){
                Debug.DrawRay(transform.position, retDirection.normalized * 2, Color.magenta);
            }

            return retDirection;
        }



        private Vector2 getDirectionSeek(){
            
            return (finalTarget.position - transform.position).normalized * speed * Time.deltaTime;
        }
        private Vector2 getDirectionFlee(){

            return (finalTarget.position - transform.position).normalized * speed * Time.deltaTime * -1;

        }
        private Vector2 getDirectionPursue(){



            Rigidbody2D targetRigidBody = finalTarget.GetComponent<Rigidbody2D>();
            if(targetRigidBody == null || targetRigidBody.Equals(null)){
                Debug.LogError($"Errore: impossibile eseguire il comportamento pursuit nel gameObject {gameObject.name}: componente RigidBody2D non trovato nel target {finalTarget.name}");
            }

            float distance = Vector3.Distance(finalTarget.position, transform.position);
            float ahead = distance / 10;
            Vector3 futurePosition = new Vector2(finalTarget.position.x, finalTarget.position.y) + targetRigidBody.velocity * ahead;

            return (futurePosition - transform.position).normalized * speed * Time.deltaTime;

            

        }
        private Vector2 getDirectionEvade(){

            Rigidbody2D targetRigidBody = finalTarget.GetComponent<Rigidbody2D>();
            if(targetRigidBody == null || targetRigidBody.Equals(null)){
                Debug.LogError($"Errore: impossibile eseguire il comportamento Evade nel gameObject {gameObject.name}: componente RigidBody2D non trovato nel target {finalTarget.name}");
            }

            float distance = Vector3.Distance(finalTarget.position, transform.position);
            float ahead = distance / 10;
            Vector3 futurePosition = new Vector2(finalTarget.position.x, finalTarget.position.y) + targetRigidBody.velocity * ahead;

            transform.position = Vector2.MoveTowards(
                transform.position,
                futurePosition,
                speed * Time.deltaTime * -1
            );

            return (futurePosition - transform.position).normalized * Time.deltaTime * speed * -1;


        }
        private Vector2 getAvoidObstacleDirection(Vector2 targetDirection){


            targetDirection = targetDirection.normalized;


            List<Vector2> directions = new List<Vector2>();

            directions.Add(Quaternion.Euler(0, 0, -60) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, -30) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, -10) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, 0) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, 10) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, 30) * targetDirection);
            directions.Add(Quaternion.Euler(0, 0, 60) * targetDirection);
            
        

            List<RaycastHit2D> collisions = directions.Select(el =>  Physics2D.Raycast(transform.position, el, 2, obstaclesLayerMask)).ToList();
            
            List<float> weights =  collisions.Select((collision, idx) => {return collision.distance == 0? 1: collision.distance / 2;}).ToList();
            weights[3] += 0.1f;
            List<float> finalWeights = weights.Select(el => el).ToList();
            weights
                .Select((weight, idx) => (weight, idx))
                .ToList()
                .ForEach(data => {
                    float remainder = 1 - data.weight;
                    if(data.idx == 0){
                        //primo elemento
                        finalWeights[data.idx + 1] -= (remainder/2);
                        finalWeights[data.idx + 2] -= (remainder/4);

                    }
                    else if(data.idx == 1){
                        finalWeights[data.idx - 1] -= (remainder/2);
                        finalWeights[data.idx + 1] -= (remainder/2);
                        finalWeights[data.idx + 2] -= (remainder/4);

                    }
                    else if(data.idx == weights.Count() - 1){
                        //ultimo elemento
                        finalWeights[data.idx - 1] -= (remainder/2);
                        finalWeights[data.idx - 2] -= (remainder/4);

                    }
                    else if(data.idx == weights.Count() - 2){
                        //ultimo elemento
                        finalWeights[data.idx - 1] -= (remainder/2);
                        finalWeights[data.idx - 2] -= (remainder/4);
                        finalWeights[data.idx + 1] -= (remainder/4);

                    }
                    else{
                        finalWeights[data.idx - 1] -= (remainder/2);
                        finalWeights[data.idx + 1] -= (remainder/2);
                        finalWeights[data.idx - 2] -= (remainder/4);
                        finalWeights[data.idx + 2] -= (remainder/4);

                    }
                });


            // foreach (var weight in finalWeights)
            // {   
            //     if(weight > 0) break;
            //     if(!string.IsNullOrEmpty((string)idleAnimationName.GetValue()) && _myAnimator != null){
            //         _myAnimator.Play((string)idleAnimationName.GetValue());
            //     }
            //     paused = true;
            //     return new Vector2(0, 0);
            // }
            // if(paused && !string.IsNullOrEmpty(movingAnimationName) && _myAnimator != null){
            //     _myAnimator.Play(movingAnimationName);
            // }
            int indexMax = finalWeights.IndexOf(finalWeights.Max());
            // transform.position = (Vector2)transform.position + (directionMax * speed * Time.deltaTime);


            //Debug
            if(showGizmos){
                directions.Select((direction, idx) => (direction, idx))
                .ToList()
                .ForEach(data => {
                    if(data.idx == indexMax){
                        Debug.DrawRay(transform.position, data.direction.normalized * 2, Color.white);
                        Debug.DrawRay(transform.position, data.direction.normalized * finalWeights[data.idx], Color.green);
                    }
                    else{
                        Debug.DrawRay(transform.position, data.direction.normalized * 2, Color.white);
                        Debug.DrawRay(transform.position, data.direction.normalized * finalWeights[data.idx], Color.red);
                    }
                });
            }

            return directions[indexMax].normalized * speed * Time.deltaTime;
        
        }

        private void flipInDirection(Vector2 direction){
            if(Time.time > lastTimeRotated + debounceInterval){
                lastTimeRotated = Time.time;

                if(direction.x < 0 && transform.eulerAngles.y == 0){
                    transform.RotateAround(transform.position, transform.up, 180);
                }
                else if(direction.x > 0 && transform.eulerAngles.y == 180){
                    transform.RotateAround(transform.position, transform.up, 180);
                }

            }

        }

        
    }


}