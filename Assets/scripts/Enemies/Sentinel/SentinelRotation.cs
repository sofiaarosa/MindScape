using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SentinelRotation : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform parent;
    private Transform lightTransform;
    private Light lightComponent;
    private bool isInside = false;
    void Start()
    {
        parent = transform.parent;
        lightTransform = parent.Find("Light");
        lightComponent = lightTransform.GetComponent<Light>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(PlayerStatus.isInvisible) return;
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            lightComponent.intensity = 2;
            isInside = true;
        }
    }

    private IEnumerator DamageCooldown(){
        yield return new WaitForSeconds(2);
        isInside = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if(PlayerStatus.isInvisible) return;
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            if(isInside){
                Debug.Log("Health: " + PlayerStatus.Health);
                playerStatus.DamageTaken(1, "Sentinela da Ansiedade");
                isInside = false;
                // damage cooldown
                StartCoroutine(DamageCooldown());
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(PlayerStatus.isInvisible) return;
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            lightComponent.intensity = 0;
            isInside = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition  = PlayerStatus.CurrentCharacter.CharacterTransform.position;
        // Calculate direction to the player
        Vector3 direction = (playerPosition - transform.position);
        direction.y = 0;

        if (direction.magnitude > 0.1f)
            {
                // Calculate the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the player
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }
    }
}
