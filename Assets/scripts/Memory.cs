using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public int Type;
    private Transform audioSource;

    private void Start(){
        audioSource = GameObject.Find("CollectSound").transform;
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log("Colidiu!");
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            playerStatus.MemoryCollected(Type);
            audioSource.position = transform.position;
            audioSource.GetComponent<AudioSource>().Play();
            gameObject.SetActive(false);
        }
    }
}
