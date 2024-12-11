using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscilator : MonoBehaviour
{
    private List<Light> lights;
    private bool isOscillating;
    private bool entered_invisible;
    private bool entered_visible;
    private int previousHealth;

    // Start is called before the first frame update
    void Start()
    {
        lights = new List<Light>();
        Transform lightsParents = transform.Find("lights");
        foreach (Transform child in lightsParents)
        {
            lights.Add(child.GetComponent<Light>());
        }
    }

    public void StartOscillation()
    {
        if (!isOscillating)
        {
            isOscillating = true;
            StartCoroutine(OscillateHealth());
        }
    }

    public void StopOscillation()
    {
        isOscillating = false;
        // Não precisa usar StopCoroutine porque `isOscillating` controlará a interrupção dos loops.
        Debug.Log("Oscillation stopped! Health: " + PlayerStatus.Health);
        // GameController.Instance.updateHealthSlider(PlayerStatus.Health);
        if (PlayerStatus.Health < previousHealth)
        {
            GameController.Instance.updateHealthDescription("-" + (previousHealth - PlayerStatus.Health) + " - Oscilador");
            PlayerStatus.LastDamage = previousHealth - PlayerStatus.Health;
        }else if(PlayerStatus.Health > previousHealth){
            GameController.Instance.updateHealthDescription("+" + (PlayerStatus.Health - previousHealth) + " - Oscilador");
        }
    }

    private IEnumerator OscillateHealth()
    {
        while (isOscillating)
        {
            // Escolhe aleatoriamente o intervalo e a velocidade de oscilação
            int targetMin = Random.Range(0, 2) == 0 ? 10 : 50; // 10 ou 50 como valor mínimo
            int targetMax = PlayerStatus.MAX_HEALTH; 
            float oscillationSpeed = Random.Range(0.5f, 2f); // Velocidade aleatória (quanto menor, mais rápido)

            // Oscila entre os dois valores
            yield return OscillateBetween(targetMin, targetMax, oscillationSpeed);
        }
    }

    private IEnumerator OscillateBetween(int min, int max, float speed)
    {
        // Primeiro, subindo de min para max
        while (PlayerStatus.Health < max && isOscillating)
        {
            PlayerStatus.Health += Mathf.CeilToInt(speed * Time.deltaTime * 100);
            PlayerStatus.Health = Mathf.Clamp(PlayerStatus.Health, min, max); // Garante que não ultrapasse o limite

            // GameController.Instance.updateHealthSlider(PlayerStatus.Health);

            UpdateLightColors(true);
            yield return null;
        }

        // Depois, descendo de max para min
        while (PlayerStatus.Health > min && isOscillating)
        {
            PlayerStatus.Health -= Mathf.CeilToInt(speed * Time.deltaTime * 100);
            PlayerStatus.Health = Mathf.Clamp(PlayerStatus.Health, min, max); // Garante que não ultrapasse o limite
            UpdateLightColors(false);
            yield return null;
        }
    }

    private void UpdateLightColors(bool isIncreasing)
    {
        // Define a cor com base no estado da energia emocional
        Color targetColor = isIncreasing ? Color.white : Color.red;

        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.color = targetColor;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            previousHealth = PlayerStatus.Health;

            if (!PlayerStatus.isInvisible && !PlayerStatus.isImmune)
            {
                entered_invisible = false;
                StartOscillation(); 
            }
            else
            {
                entered_invisible = true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!entered_invisible && (PlayerStatus.isInvisible || PlayerStatus.isImmune))
            {
                StopOscillation();
                entered_invisible = true; // Marca que o jogador está imune ou invisível
            }

            if (entered_invisible && !PlayerStatus.isInvisible && !PlayerStatus.isImmune)
            {
                StartOscillation();
                entered_invisible = false; // Marca que o jogador saiu do estado imune/invisível
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!PlayerStatus.isInvisible && !PlayerStatus.isImmune)
            {
                StopOscillation();
                entered_invisible = false;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(isOscillating == false)
        {
            foreach (Light light in lights)
            {
                light.color = Color.white;
                light.intensity = Mathf.PingPong(Time.time * 2, 1);
            }
        }else{
            Debug.Log("health: " + PlayerStatus.Health);
        }
    }
}
