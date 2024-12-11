using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
using System;

public class CharacterSwap : MonoBehaviour
{
    public List<Transform> transforms;
    public List<ParticleSystem> effects;
    public CinemachineFreeLook cam;
    private List<Character> characters = new List<Character>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var effect in effects)
        {
            if(effect.isPlaying) effect.Stop();
        }

        SetUpCharacters(transforms.ToArray(), effects.ToArray());
        PlayerStatus.CurrentCharacter = characters[0];
        CurrentCharacterSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            if(PlayerStatus.CurrentCharacter != characters[0] && PlayerStatus.CanChange)
                Swap(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            if(PlayerStatus.CurrentCharacter != characters[1] && PlayerStatus.CanChange)
                Swap(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            if(PlayerStatus.CurrentCharacter != characters[2] && PlayerStatus.CanChange)
                Swap(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            if(PlayerStatus.CurrentCharacter != characters[3] && PlayerStatus.CanChange)
                Swap(3);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            if(PlayerStatus.CurrentCharacter != characters[4] && PlayerStatus.CanChange)
                Swap(4);
        }
    }


    private void SetUpCharacterAbility(Character character, Type alibityType){
        character.CharacterTransform.gameObject.AddComponent(alibityType);
        character.Ability = character.CharacterTransform.gameObject.GetComponent(alibityType) as IAbility;
        Debug.Log(character.CharacterTransform.name + " - Ability: " + character.CharacterTransform.gameObject.GetComponent(alibityType).GetType().Name.ToString());
    }

    private void SetUpCharacters(Transform[] transforms, ParticleSystem[] effects){
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform camFocus = transforms[i].Find("camFocus");
            Character character = new Character(effects[i], transforms[i]);
            character.CamFocus = camFocus;
            characters.Add(character);
        }

        SetUpCharacterAbility(characters[0], typeof(OptimisticJump));
        SetUpCharacterAbility(characters[1], typeof(StrenghtExplosion));
        SetUpCharacterAbility(characters[2], typeof(ComfortingTear));
        SetUpCharacterAbility(characters[3], typeof(CarefulStep));
        SetUpCharacterAbility(characters[4], typeof(ProtectiveFilter));
    }

    private void CurrentCharacterSetup(){
        Transform transform = PlayerStatus.CurrentCharacter.CharacterTransform;
        Transform camFocus = PlayerStatus.CurrentCharacter.CamFocus;
        transform.gameObject.SetActive(true);
        foreach (var t in transforms)
        {
            if(t == transform) continue;
            t.gameObject.SetActive(false);
        }
        cam.LookAt = camFocus;
        cam.Follow = camFocus;

        if (GameController.Instance != null)
        {
            GameController.Instance.updateCharacter(0);
        }
    }

    void ChangePlayerCharacter(Character newCharacter){
        PlayerStatus.CurrentCharacter = newCharacter;
        PlayerStatus.CanChange = false;
    }

    void GameControllerChangeCharacter(int index){
        GameController.Instance.updateCharacter(index);
        GameController.Instance.StartCooldown("CharacterSwap", 10f, () => {PlayerStatus.CanChange = true; Debug.Log("Can Change");});
    }

    void Swap(int character){
        Transform currentTransform = PlayerStatus.CurrentCharacter.GetCurrentTransform();

        PlayerStatus.CurrentCharacter.CharacterTransform.gameObject.SetActive(false);

        ChangePlayerCharacter(characters[character]);

        PlayerStatus.CurrentCharacter.CharacterTransform.gameObject.transform.rotation = currentTransform.rotation;
        PlayerStatus.CurrentCharacter.CharacterTransform.gameObject.transform.position = currentTransform.position;
        
        cam.LookAt = PlayerStatus.CurrentCharacter.CamFocus;
        cam.Follow = PlayerStatus.CurrentCharacter.CamFocus;

        ParticleSystem effect = PlayerStatus.CurrentCharacter.Effect;
        effect.transform.position = currentTransform.position;
        effect.Play();
        PlayerStatus.CurrentCharacter.CharacterTransform.gameObject.SetActive(true);

        GameControllerChangeCharacter(character);
    }
}
