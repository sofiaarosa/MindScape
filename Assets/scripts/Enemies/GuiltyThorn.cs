using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiltyThorn : MonoBehaviour
{
    private Animator animator;
    private Transform proximity;
    private bool isDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        proximity = transform.Find("ProximityCollider");
    }

    public void GetDisabled(){
        if(isDisabled) return;
        animator.SetBool("isDisabled", true);   
        isDisabled = true;
    }

    public void GetEnabled(){
        if(!isDisabled) return;
        isDisabled = false;
        animator.SetBool("isDisabled", false);
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Guilty Thorn OnTriggerEnter");
        if(isDisabled) return;
        PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            Debug.Log("Player hit by Guilty Thorn");
            playerStatus.DamageTaken(10, "Espinho da Culpa");
            GetDisabled();
            StartCoroutine(EnableAfterSeconds(5));
        }
    }

    private IEnumerator EnableAfterSeconds(int seconds){
        yield return new WaitForSeconds(seconds);
        GetEnabled();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
