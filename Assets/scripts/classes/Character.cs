using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public Color EffectsColor {get; set;}
    public Transform CharacterTransform {get; set;}
    public Transform CamFocus {get; set;}
    public ParticleSystem Effect {get; set;}
    public IAbility Ability {get; set;}

    // constructors
    public Character(ParticleSystem effect, Transform transform){
        this.Effect = effect;
        this.EffectsColor = effect.colorOverLifetime.color.colorMax;
        this.CharacterTransform = transform;
        // Ability = null;
    }

    public Character(ParticleSystem effect, Transform transform, IAbility Ability){
        this.Effect = effect;
        this.EffectsColor = effect.colorOverLifetime.color.colorMax;
        this.CharacterTransform = transform;
        // this.Ability = Ability;
    }

    public Transform GetCurrentTransform(){
        return CharacterTransform.gameObject.transform;
    }

}
