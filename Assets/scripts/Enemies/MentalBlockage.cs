using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalBlockage : MonoBehaviour, IEnemy
{
    private Animator animator;
    public void GetDestroyed()
    {
        transform.Find("EnemyProximity").GetComponent<CapsuleCollider>().enabled = false;
        animator.SetBool("isTaunting", true);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
