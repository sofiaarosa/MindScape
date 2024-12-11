using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // para os casos em que o cooldown foi interrompido pela troca de personagem
    private bool verifyCooldown(){
        return Time.time - PlayerStatus.CurrentCharacter.Ability.LastCalled >= PlayerStatus.CurrentCharacter.Ability.CallCooldown;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("PlayerStatus.CurrentCharacter.Ability: " + PlayerStatus.CurrentCharacter.Ability.Name);
            if(PlayerStatus.CurrentCharacter.Ability.CanCall) 
                PlayerStatus.CurrentCharacter.Ability.Run();
        }
    }
}
