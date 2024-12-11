using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class puddle : MonoBehaviour
{
    public Material defaultMaterial;
    public Material disabledMaterial;
    private List<Transform> children;
    private bool entered_invisible = false;

    // Start is called before the first frame update
    void Start()
    {
        children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++){
            children.Add(transform.GetChild(i));
        }

        SetMaterial(defaultMaterial);
    }

    private void SetMaterial(Material material){
        transform.GetChild(0).GetComponent<Renderer>().material = material;
    }

    public void OnTriggerEnter(Collider other){
        PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            if(!PlayerStatus.isInvisible) {
                playerStatus.GetSlowed();
                entered_invisible = false;
            }
            else entered_invisible = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if (playerStatus != null)
        {
            if (!entered_invisible && (PlayerStatus.isInvisible || PlayerStatus.isImmune))
            {
                playerStatus.GetNormalSpeed();
                entered_invisible = true; // Marca que o jogador está imune ou invisível
            }

            if (entered_invisible && !PlayerStatus.isInvisible && !PlayerStatus.isImmune)
            {
                playerStatus.GetSlowed();
                entered_invisible = false; // Marca que o jogador saiu do estado imune/invisível
            }
        }
    }

    public void OnTriggerExit(Collider other){
        PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if(playerStatus != null){
            playerStatus.GetNormalSpeed();
            entered_invisible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerStatus.isInvisible){
            SetMaterial(disabledMaterial);
        } else {
            SetMaterial(defaultMaterial);
        } 

    }
}
