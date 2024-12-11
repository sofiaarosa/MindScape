using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrenghtExplosion : MonoBehaviour, IAbility
{
    public string Name {get; set;} = "Explosão de Força";

    public string Description {get; set;} = "Com essa habilidade, Raiva pode destruir bloqueios mentais e sentinelas da insegurança.";

    public float CallCooldown {get; set;} = 5f;

    public bool CanCall {get; set;} = true;
    public float Duration {get; set;} = 2f;
    public float LastCalled { get; set;}

    private bool isRunning = false;
    private Animator animator;
    private PlayerMovement playerMovement;

    public void Run(params object[] args)
    {
        if(PlayerStatus.isCloseToEnemy && !isRunning){
            LastCalled = Time.time;
            
            Debug.Log("StrenghtExplosion: Starting Ability...");
            animator.SetBool("isTaunting", true);
            isRunning = true;
            
            playerMovement.enabled = false;
            PlayerStatus.CanChange = false;
            Debug.Log("StrenghtExplosion: Closest Enemy: " + PlayerStatus.ClosestEnemy);
            PlayerStatus.ClosestEnemy.GetDestroyed();

            GameController.Instance.StartCooldown("StrenghtExplosionRunning", Duration, AbilityFinished);
        }else{
            GameController.Instance.UpdateWarning("Você precisa estar perto de um bloqueio ou sentinela");
        }
    }

    private void AbilityFinished()
    {
        animator.SetBool("isTaunting", false);
        isRunning = false;
        playerMovement.enabled = true;
        PlayerStatus.CanChange = true;
        PlayerStatus.ClosestEnemy.transform.gameObject.SetActive(false);
        PlayerStatus.ClosestEnemy = null;
        Debug.Log("StrenghtExplosion: Ability Finished!");
        CanCall = false;

        Debug.Log("StrenghtExplosion: Starting Cooldown...");
        GameController.Instance.StartCooldown("StrenghtExplosionCooldown", CallCooldown, CooldownFinished);
    }

    private void CooldownFinished()
    {
        CanCall = true;
        Debug.Log("StrenghtExplosion: Cooldown Finished!");
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.Find("Armature").GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerStatus.isCloseToEnemy){
            CanCall = false;
        }else{
            CanCall = true;
        }
    }
}
