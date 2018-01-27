using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader m_instance;

    // Use this for initialization
    private void Start()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadGameOneMenu()
    {
        SceneManager.LoadScene("GameOneMenu");
    }

    public void LoadGameOneCharacterSelect()
    {
        SceneManager.LoadScene("GameOneCharacterSelect");
    }

    public void LoadGameOne()
    {
        SceneManager.LoadScene("GameOne");
    }

    public void LoadGameTwoMenu()
    {
        SceneManager.LoadScene("GameTwoMenu");
    }

    public void LoadGameTwoCharacterSelect()
    {
        SceneManager.LoadScene("GameTwoCharacterSelect");
    }

    public void LoadGameTwo()
    {
        SceneManager.LoadScene("GameTwo");
    }

    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
}