using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveFilter : MonoBehaviour, IAbility
{
    public string Name { get; set; } = "Filtro Protetor";

    public string Description { get; set; } = "Com essa habilidade, Nojinho fica invisível aos olhos das Sentinelas da Insegurança e imune aos efeitos negativos por um certo período.";

    public float CallCooldown { get; set; } = 20f;
    public float Duration { get; set; } = 10f;

    public bool CanCall { get; set; } = true;

    public float LastCalled { get; set; }

    private PlayerStatus playerStatus;

    void Start(){
        playerStatus = GetComponent<PlayerStatus>();
    }

    public void Run(params object[] args)
    {
        if(!PlayerStatus.isInvisible){
            LastCalled = Time.time;
            Debug.Log("ProtectiveFilter: Starting Ability...");
            PlayerStatus.CanChange = false;
            playerStatus.GetInvisible();
            GameController.Instance.StartCooldown("ProtectiveFilterRunning", Duration, AbilityFinished);
        }
    }

    private void AbilityFinished()
    {
        playerStatus.GetVisible();
        PlayerStatus.CanChange = true;

        Debug.Log("ProtectiveFilter: Ability Finished!");
        CanCall = false;

        Debug.Log("ProtectiveFilter: Starting Cooldown...");
        GameController.Instance.StartCooldown("ProtectiveFilterCooldown", CallCooldown, CooldownFinished);
    }
    private void CooldownFinished()
    {
        CanCall = true;
        Debug.Log("ProtectiveFilter: Cooldown Finished!");
    }
}
