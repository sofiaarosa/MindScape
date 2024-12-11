using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sentinel : MonoBehaviour, IEnemy
{
    private Transform wall;
    private Animator wallAnimator;
    private Transform sentinel1, sentinel2;
    private Transform proximity;
    void Start()
    {
        wall = transform.Find("Wall");
        wallAnimator = wall.GetComponent<Animator>();
        sentinel1 = transform.Find("sentinel1");
        sentinel2 = transform.Find("sentinel2");
        proximity = transform.Find("EnemyProximity");
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            wallAnimator.SetBool("isGrowing", true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player"){
            wallAnimator.SetBool("isGrowing", false);
        }
    }

    public void GetDestroyed(){
        proximity.GetComponent<CapsuleCollider>().enabled = false;
        sentinel1.GetComponent<Animator>().SetBool("isTaunting", true);
        sentinel2.GetComponent<Animator>().SetBool("isTaunting", true);
        if(wallAnimator.GetBool("isGrowing")){
            wallAnimator.SetBool("isGrowing", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
