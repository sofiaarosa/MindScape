using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyProximity : MonoBehaviour
{
    private Transform enemy;
    void Start()
    {
        enemy = transform.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player is close to " + enemy.tag);
            PlayerStatus.isCloseToEnemy = true;
            PlayerStatus.ClosestEnemy = enemy.GetComponent<IEnemy>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player is far from " + enemy.tag);
            PlayerStatus.isCloseToEnemy = false;
            PlayerStatus.ClosestEnemy = null;
        }
    }

    void Update()
    {
        
    }
}
