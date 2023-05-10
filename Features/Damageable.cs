using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


// Rende un agente danneggiabile ed in grado di ricevere colpi tramite il metodo reciveDamage
public class Damageable : EnchantedMonoWithPool
{
    
    /// Espone la vita corrente all'esterno
    public float readOnlyCurrentLife{get => _currentLife;}

    /// Espone la vita massima all'esterno
    public float readOnlyMaxLife{get => maxLife;}

    [Tooltip("Vita iniziale e massima")]
    [SerializeField] private float maxLife;
    [Tooltip("Tempo di immunità tra un attacco e l'altro (definisce anche il tempo di animazione)")]
    [SerializeField] private float immuneTime;
    [Tooltip("Il personaggio non sarà veramente immune, ma riprodurrà l'animazione di danno per il tempo specificato in \"immuneTime\"")]
    [SerializeField] private bool fakeImmunity;
    [Tooltip("GameObject da istanziare al momento di prendere danno")]
    [SerializeField] private GameObject damageParticles;
    [Tooltip("Suono da riprodurre nel momento di prendere danno")]
    [SerializeField] private AudioClip damageSound;
    [Tooltip("Suono da riprodurre quando viene ricaricata la vita")]
    [SerializeField] private AudioClip refuelLifeSound;
    [Tooltip("Animazione da riprodurre al momento di prendere danno (ATTENZIONE: Verrà riprodotta sul layer 1)")]
    [SerializeField] private string damageAnimationName;
    [SerializeField] private bool resetLifeOnAwake;



    private Animator myAnimator;
    private Rigidbody2D myRigidBody;
    private float _currentLife;
    private float lastImmuneTime;
    private float lastHitTime;
    private bool isImmune = false;
    private AudioSource myAudioSource;



    private void OnValidate() {
        CheckDependencyPropMissing(damageParticles, "damageParticles", LogLevel.INFO);
        CheckDependencyPropMissing(maxLife, "maxLife", LogLevel.WARN);
        CheckDependencyPropMissing(damageSound, "damageSoundName", LogLevel.INFO);
        CheckDependencyPropMissing(refuelLifeSound, "refuelLifeSound", LogLevel.INFO);
        CheckDependencyPropMissing(damageAnimationName, "damageAnimationName", LogLevel.INFO);

        
    }



    private void Awake() {
        _currentLife = maxLife;
        myAnimator = gameObject.GetComponent<Animator>();
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();

        CheckDependencyComponentMissing(gameObject.GetComponent<Collider2D>(), "Collider2D");
        CheckDependencyComponentMissing(myAnimator, "Animator", LogLevel.WARN);
        CheckDependencyComponentMissing(myRigidBody, "RigidBody2D");

        if(damageSound != null || refuelLifeSound != null){
            myAudioSource = GetComponent<AudioSource>();
            CheckDependencyComponentMissing(myAudioSource, "myAudioSource");
        }

    }

    void OnEnable(){
        if(resetLifeOnAwake){
            _currentLife = maxLife;
        }
    }
  

    void Update()
    {
        
        //Scaduto il tempo dell'immunità... la disabilito
        if(Time.time > lastImmuneTime + immuneTime && isImmune){
            isImmune = false;
            if(myAnimator != null && !string.IsNullOrEmpty(damageAnimationName)){
                myAnimator.Play("Void", 1);
            }
        }


    }


    /// <summary>
    /// Da chiamare per infliggere danno; questa funzione scatta solo se il personaggio non è immune
    /// </summary>
    /// <param name="dmgData"></param>
    public void reciveDamage(DamageModel dmgData){

        if(!isImmune || fakeImmunity)
        {
            lastImmuneTime = Time.time;
            isImmune = true;
            
            if(myAudioSource != null && damageSound != null){
                myAudioSource.PlayOneShot(damageSound);
            }

            _currentLife -= dmgData.damageAmount;
            lastHitTime=Time.time;
            myRigidBody.AddForce((transform.position - dmgData.origin).normalized *dmgData.pushForce, ForceMode2D.Impulse);

            if(myAnimator != null && !string.IsNullOrEmpty(damageAnimationName)){
                myAnimator.Play(damageAnimationName, 1);
            }
            if(damageParticles != null){
                GameObject particles = pickOrIstantiate(damageParticles,transform.position,Quaternion.identity);
            }
            
            

        }
    }

    /// <summary>
    /// Resetta la vita corrente a "maxLife
    /// </summary>
    public void refuelLife(){
        if(myAudioSource != null && refuelLifeSound != null){
                myAudioSource.PlayOneShot(refuelLifeSound);
        }
        _currentLife = maxLife;
    }   


}
