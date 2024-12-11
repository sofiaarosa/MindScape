using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // all variables are static so they will be the same for all characters
    // because all characters are the same player

    public static int Memories {get; private set;} = 0;
    public static Dictionary<int, int> MemoriesTyped = new Dictionary<int, int>(){
        {0, 0},
        {1, 0},
        {2, 0},
        {3, 0},
        {4, 0}
    };
    public static Dictionary<int, int> MemoriesTypedGoal = new Dictionary<int, int>();
    public static int Points {get; private set;} = 0;
    public const int MAX_HEALTH = 100;
    public static int Health {get; set;} = 100;
    public const int MAX_SPEED = 5;
    public static int Speed {get; set;} = MAX_SPEED;

    public static bool CanChange {get; set;} = true;
    public static bool isImmune {get; set;} = false;
    public static bool isInvisible {get; set;} = false;
    public static bool isCloseToEnemy {get; set;} = false;
    public static IEnemy ClosestEnemy {get; set;} = null;
    public static int LastDamage {get; set;} = 0;

    public static Character CurrentCharacter {get; set;}

    public int mem = 0;
    private float LastDamageTime = 0;
    private Dictionary<int, string> memoryTypesString = new Dictionary<int, string>(){
        {0, "Alegria"},
        {2, "Tristeza"},
        {1, "Raiva"},
        {3, "Medo"},
        {4, "Nojinho"}
    };

    private AudioSource damageAudioSource;
    public static bool hasGoal = false;

    public void Start(){
        Health = MAX_HEALTH;
        Speed = MAX_SPEED;

        // sorteia um número aleatório entre 0 e 10 para o objetivo de cada memoria
        if(!hasGoal){
	        hasGoal = true;
		for (int i = 0; i < 5; i++)
		{
		    MemoriesTypedGoal[i] = Random.Range(0, 10);
		    Debug.Log("Memoria " + i + " tem objetivo de " + MemoriesTypedGoal[i]);
		}
        }
        if(GameController.Instance != null){
            GameController.Instance.updateHealthSlider(Health);
            GameController.Instance.updateMemoriesCollected(MemoriesTypedGoal, MemoriesTyped);
        }
        damageAudioSource = transform.GetComponent<AudioSource>();
    }

    public void MemoryCollected(int type){
        Memories++;
        MemoriesTyped[type]++;

        if(MemoriesTyped[type] > MemoriesTypedGoal[type]){
            DamageTaken(10, "Desequilíbrio - " + memoryTypesString[type]);
        }

        Debug.Log("Memoria coletada: " + type);
        GameController.Instance.updateMemoriesCollected(MemoriesTypedGoal, MemoriesTyped);
    }

    public void DamageTaken(int damage, string damageReason = " "){
        if(!isImmune){
            Health-=damage;
            damageAudioSource.Play();
            if(Health <= 0){
            	hasGoal = false;
                GameController.Instance.FinishGame(win: false);
                CurrentCharacter.CharacterTransform.gameObject.SetActive(false);
            }

            if(Time.time - LastDamageTime < 5){
                LastDamage += damage;
                GameController.Instance.updateHealthDescription("-" + LastDamage + " - " + damageReason);
            }
            else{
                LastDamageTime = Time.time;
                LastDamage = damage;
                GameController.Instance.updateHealthDescription("-" + damage + " - " + damageReason);
            }
        }   
    }

    public void GetImmune(){
        isImmune = true;    
    }

    public void GetVulnerable(){
        isImmune = false;
    }

    public void GetInvisible(){
        isInvisible = true;
        Transform filter = transform.Find("Filter");
        filter.gameObject.SetActive(true);
    }

    public void GetVisible(){
        isInvisible = false;
        Transform filter = transform.Find("Filter");
        filter.gameObject.SetActive(false);
    }

    public void GetSlowed(){
        Speed = 1;
    }

    public void GetNormalSpeed(){
        Speed = MAX_SPEED;
    }

}
