using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisableOnProximity : MonoBehaviour
{
    private GuiltyThorn parent;
    private bool gotDisabled;
    void Start(){
        parent = GetComponentInParent<GuiltyThorn>();
    }
    void OnTriggerEnter(Collider other){
        PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            if(PlayerStatus.isImmune){
                parent.GetDisabled();
                gotDisabled = true;
            }
        }
    }

    void OnTriggerExit(){
        if(gotDisabled){
            parent.GetEnabled();
            gotDisabled = false;
        }
    }
}
