using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndSceneController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        if(scoreText != null){
            scoreText.text = "Pontuação: " + GameData.score;
        }
    }

    public void GoToMenu(){
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
