using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComfortingTear : MonoBehaviour, IAbility
{
    public string Name { get; set; } = "Lágrima Reconfortante";

    public string Description { get; set; } = "Com essa habilidade, a Tristeza pode se curar do último dano emocional que o jogador recebeu";

    public float CallCooldown { get; set; } = 15f;

    public bool CanCall { get; set; } = true;
    public float LastCalled { get; set; }

    private ParticleSystem effect;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
      effect = transform.Find("sadness_effect").GetComponent<ParticleSystem>();   
      effect.Stop();
      audioSource = transform.Find("sadness_effect").GetComponent<AudioSource>();
    }

    public void Run(params object[] args)
    {
        if (PlayerStatus.LastDamage > 0)
        {
            LastCalled = Time.time;

            if(PlayerStatus.Health + PlayerStatus.LastDamage > PlayerStatus.MAX_HEALTH){
                PlayerStatus.Health = PlayerStatus.MAX_HEALTH;
            } else{
                PlayerStatus.Health += PlayerStatus.LastDamage;
            }

            GameController.Instance.updateHealthDescription("+" + PlayerStatus.LastDamage + " - Lágrima Reconfortante");

            effect.gameObject.SetActive(true);
            effect.Play();
            audioSource.Play();
            Debug.Log("ComfortingTear: Starting Cooldown...");
            GameController.Instance.StartCooldown("ComfortingTearCooldown", CallCooldown, CooldownFinished);
        }else{
            GameController.Instance.UpdateWarning("Você não sofreu dano emocional recentemente");
        }
    }

    private void CooldownFinished()
    {
        CanCall = true;
        Debug.Log("ComfortingTear: Cooldown Finished!");
    }


    // Update is called once per frame
    void Update()
    {
    }
}
