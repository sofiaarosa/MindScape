using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance; //singleton instance - boa prática já que vai envolver diversos scripts

    //elementos de UI
    public TextMeshProUGUI clockText;
    public Slider healthSlider;
    public List<Image> charactersImages;
    public Image abilityImageComponent;
    public List<Sprite> abilitiesSprites;
    public List<TextMeshProUGUI> memoriesCounters;
    public GameObject pauseImage;
    public CinemachineFreeLook freeLookCamera;
    public Transform warningText;

    // eventos - cooldown
    public event Action OnCharacterSwapCooldownStart, OnCharacterSwapCooldownEnd;
    // gerenciador de cooldowns
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
    private int currentCharacterIndex = 0;
    private List<Image> charactersKeywordsImages = new List<Image>();
    private List<TextMeshProUGUI> charactersKeywordsTexts = new List<TextMeshProUGUI>();
    private List<Image> charactersDisabledImages = new List<Image>();
    private List<TextMeshProUGUI> charactersDisabledTexts = new List<TextMeshProUGUI>();
    private Image abilityCooldownImage;
    private TextMeshProUGUI abilityCooldownText;
    private Image abilityRunningImage;
    private TextMeshProUGUI abilityRunningText;
    private TextMeshProUGUI healthDescription;
    private bool isDescriptionActive = false;
    private bool isWarningActive = false;

    private Dictionary<int, int> collected, goal;

    private Dictionary<string, Color> abilitiesColors = new Dictionary<string, Color>(){
        {"StrenghtExplosionRunning", new Color(255, 60, 60, 153)},
        {"CarefulStepRunning", new Color(200, 50, 255, 153)},
        {"ProtectiveFilterRunning", new Color(60, 255, 60, 153)}
    };

    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //inicializa o slider de vida
        healthSlider.maxValue = PlayerStatus.MAX_HEALTH;
        healthSlider.value = PlayerStatus.Health;

        //inicializa o personagem atual
        updateCharacter(0);

        foreach (var character in charactersImages)
        {
            charactersKeywordsImages.Add(character.transform.GetChild(0).GetComponent<Image>());
            charactersKeywordsTexts.Add(character.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());
            charactersDisabledImages.Add(character.transform.GetChild(1).GetComponent<Image>());
            charactersDisabledTexts.Add(character.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        abilityCooldownImage = abilityImageComponent.transform.GetChild(0).GetComponent<Image>();
        abilityCooldownText = abilityImageComponent.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        abilityRunningImage = abilityImageComponent.transform.GetChild(1).GetComponent<Image>();
        abilityRunningText = abilityImageComponent.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        healthDescription = healthSlider.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    public void updateHealthSlider(int health)
    {
        //set Slider color based on health
        Transform slider_fill = healthSlider.transform.Find("Fill Area").Find("Fill");
        Transform slider_text = healthSlider.transform.Find("Text");
        if (health > 70)
        {
            slider_fill.GetComponent<Image>().color = Color.green;
        }
        else if (health > 30)
        {
            slider_fill.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            slider_fill.GetComponent<Image>().color = Color.red;
        }
        healthSlider.value = health;
        slider_text.GetComponent<TextMeshProUGUI>().text = health + "/" + PlayerStatus.MAX_HEALTH;
    }

    public void updateHealthDescription(string description)
    {
        if(!isDescriptionActive){
            healthDescription.text = description;
            StartCoroutine(HealthDescriptionCoroutine());
        }else{
            healthDescription.text = description;
            StopCoroutine(HealthDescriptionCoroutine());
            StartCoroutine(HealthDescriptionCoroutine());
        }
    }

    private IEnumerator HealthDescriptionCoroutine()
    {
        healthDescription.enabled = true;
        isDescriptionActive = true;
        yield return new WaitForSeconds(5);
        healthDescription.enabled = false;
        isDescriptionActive = false;
    }

    public void UpdateWarning(string warning)
    {
        if(!isWarningActive){
            warningText.GetComponent<TextMeshPro>().text = warning;
            StartCoroutine(WarningCoroutine());
        }else{
            warningText.GetComponent<TextMeshPro>().text = warning;
            StopCoroutine(WarningCoroutine());
            StartCoroutine(WarningCoroutine());
        }
    }

    private IEnumerator WarningCoroutine()
    {
        warningText.GetComponent<TextMeshPro>().enabled = true;
        isWarningActive = true;
        yield return new WaitForSeconds(5);
        warningText.GetComponent<TextMeshPro>().enabled = false;
        isWarningActive = false;
    }

    public void updateMemoriesCollected(Dictionary<int, int> goals, Dictionary<int, int> memories)
    {
        collected = memories;
        goal = goals;
        for (int i = 0; i < memoriesCounters.Count; i++)
        {
            memoriesCounters[i].text = memories[i].ToString() + "/" + goals[i].ToString();
        }
    }

    public void updateCharacter(int characterIndex)
    {
        currentCharacterIndex = characterIndex;
        for (int i = 0; i < charactersImages.Count; i++)
        {
            if (i == characterIndex)
            {
                charactersImages[i].rectTransform.sizeDelta = new Vector2(35, 35);
            }
            else
            {
                charactersImages[i].rectTransform.sizeDelta = new Vector2(25, 25);
            }
        }

        abilityImageComponent.sprite = abilitiesSprites[characterIndex];
    }

    // cooldown genérico - parâmetros: keyword, duração, onEnd
    public void StartCooldown(string keyword, float duration, Action onEnd)
    {
        if (!cooldowns.ContainsKey(keyword))
        {
            cooldowns.Add(keyword, duration);
            StartCoroutine(CooldownCoroutine(keyword, duration, onEnd));
        }
    }

    private IEnumerator AbilityRunningUIUpdateCoroutine(string keyword, float duration)
    {
        abilityRunningImage.enabled = true;
        abilityRunningImage.color = abilitiesColors[keyword];
        abilityRunningText.enabled = true;
        
        while(duration > 0){
            abilityRunningText.text = duration.ToString("F0");
            duration -= Time.deltaTime;
            cooldowns[keyword] = duration;
            yield return null;
        }

        abilityRunningImage.enabled = false;
        abilityRunningText.enabled = false;
    }

    private IEnumerator AbilityCooldownUIUpdateCoroutine(string keyword, float duration)
    {
        abilityCooldownImage.enabled = true;
        abilityCooldownText.enabled = true;
        
        while(duration > 0){
            abilityCooldownText.text = duration.ToString("F0");
            duration -= Time.deltaTime;
            cooldowns[keyword] = duration;
            yield return null;
        }

        abilityCooldownImage.enabled = false;
        abilityCooldownText.enabled = false;
    }

    private IEnumerator CharacterSwapCooldownUIUpdateCoroutine(float duration)
    {
        for (int i = 0; i < charactersDisabledImages.Count; i++)
        {
            if(i == currentCharacterIndex) continue;

            charactersDisabledImages[i].enabled = true;
            charactersDisabledTexts[i].enabled = true;
            charactersKeywordsImages[i].enabled = false;
            charactersKeywordsTexts[i].enabled = false;
        }

        while(duration > 0){
            for(int i = 0; i < charactersDisabledTexts.Count; i++){
                if(i == currentCharacterIndex) continue;
                charactersDisabledTexts[i].text = duration.ToString("F0");
            }
            duration -= Time.deltaTime;
            cooldowns["CharacterSwap"] = duration;
            yield return null;
        }

        for (int i = 0; i < charactersDisabledImages.Count; i++)
        {
            if(i == currentCharacterIndex) continue;

            charactersDisabledImages[i].enabled = false;
            charactersDisabledTexts[i].enabled = false;
            charactersKeywordsImages[i].enabled = true;
            charactersKeywordsTexts[i].enabled = true;
        }
        
    }

    private IEnumerator CooldownCoroutine(string keyword, float duration, Action onEnd)
    {
        if(keyword == "CharacterSwap"){
            OnCharacterSwapCooldownStart?.Invoke();
            StartCoroutine(CharacterSwapCooldownUIUpdateCoroutine(duration));
        }else if(keyword.Contains("Running")){
            StartCoroutine(AbilityRunningUIUpdateCoroutine(keyword, duration));
        }else if(keyword.Contains("Cooldown")){
            StartCoroutine(AbilityCooldownUIUpdateCoroutine(keyword, duration));
        }
        
        yield return new WaitForSeconds(duration);
        cooldowns.Remove(keyword);

        if(keyword == "CharacterSwap"){
            OnCharacterSwapCooldownEnd?.Invoke();
        }

        onEnd?.Invoke();
    }

    private int CalculateScore(){
        float time = Time.time;

        int baseScore = 0;
        int penalty = 0;

        for(int i = 0; i < collected.Count; i++){
            baseScore += 100 * collected[i];
            penalty += 10 * (collected[i] - goal[i]);
        }

        int timeBonus = Mathf.Max(500 - Mathf.FloorToInt(time / 2), 0);

        int finalScore = baseScore - penalty + timeBonus;
        return finalScore;
    }

    public void FinishGame(bool win = false){
        if(win){
            Debug.Log("You Win!");
            int score = CalculateScore();
            GameData.score = score;
            //change to scene with score screen
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }else{
            Debug.Log("Game Over!");
            //change to game over scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(4);
        }
    }

    public void ExitGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        // Pausa o tempo do jogo
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (freeLookCamera != null)
        {
            freeLookCamera.enabled = false;
        }

        pauseImage.SetActive(true);
    }

    private void ResumeGame()
    {
        // Retoma o tempo do jogo
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (freeLookCamera != null)
        {
            freeLookCamera.enabled = true;
        }

        pauseImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //update clock text in format MM:SS
        clockText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(Time.time / 60), Mathf.FloorToInt(Time.time % 60));

        updateHealthSlider(PlayerStatus.Health);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.Delete)){
            if(isPaused){
            	PlayerStatus.hasGoal = false;
                ExitGame();
            }
        }
    }
}
