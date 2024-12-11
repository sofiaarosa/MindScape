using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InstructionsController : MonoBehaviour
{

    // public GameObject content1, content2;
    public List<GameObject> contents;
    public Button nextButton;
    public Button backButton;
    public Button closeButton;

    private int currentContent = 0;

    public void NextButton()
    {
        contents[currentContent].SetActive(false);
        currentContent++;
        contents[currentContent].SetActive(true);
        if (currentContent == contents.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
        backButton.gameObject.SetActive(true);
    }

    public void BackButton()
    {
        contents[currentContent].SetActive(false);
        currentContent--;
        contents[currentContent].SetActive(true);
        if (currentContent == 0)
        {
            backButton.gameObject.SetActive(false);
        }
        nextButton.gameObject.SetActive(true);
    }
    
    public void CloseButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
