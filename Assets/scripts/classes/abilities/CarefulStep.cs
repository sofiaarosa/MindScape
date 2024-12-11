using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CarefulStep : MonoBehaviour, IAbility
{
    public string Name {get; set;} = "Passo Cuidadoso";

    public string Description {get; set;} = "Com essa habilidade, o Medo fica imune a qualquer dano ou oscilação emocional por um certo período de tempo.";

    public float CallCooldown {get; set;} = 10f;

    public bool CanCall {get; set;} = true;

    public float Duration {get; set;} = 10f;
    public float LastCalled { get; set;}

    private PlayerStatus playerStatus;
    private Animator animator;

    void Start()
    {   
        playerStatus = GetComponent<PlayerStatus>();
        animator = transform.Find("Armature").GetComponent<Animator>();
    }

    public void Run(params object[] args)
    {
        if(!PlayerStatus.isImmune)
        {
            LastCalled = Time.time;
            Debug.Log("CarefulStep: Starting Ability...");
            playerStatus.GetImmune();
            animator.SetBool("isSneaking", true);
            PlayerStatus.CanChange = false;
            GameController.Instance.StartCooldown("CarefulStepRunning", Duration, AbilityFinished);
        }
    }

    private void AbilityFinished()
    {
        playerStatus.GetVulnerable();
        animator.SetBool("isSneaking", false);
        PlayerStatus.CanChange = true;

        Debug.Log("CarefulStep: Ability Finished!");
        CanCall = false;

        Debug.Log("CarefulStep: Starting Cooldown...");
        GameController.Instance.StartCooldown("CarefulStepCooldown", CallCooldown, CooldownFinished);
    }

    private void CooldownFinished()
    {
        CanCall = true;
        Debug.Log("CarefulStep: Cooldown Finished!");
    }


    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}
