using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Play,
    Pause,
    GameOver
}

public class CanvasManager : MonoBehaviour
{
    public GameObject m_hud;
    public GameObject m_pauseCanvas;
    public GameObject m_gameOverCanvas;

    public static CanvasManager m_instance;

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
        UpdateCanvases(GameState.Play);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void UpdateCanvases(GameState _newState)
    {
        switch (_newState)
        {
            case GameState.Play:
                m_hud.SetActive(true);
                m_pauseCanvas.SetActive(false);
                m_gameOverCanvas.SetActive(false);
                break;

            case GameState.Pause:
                m_hud.SetActive(false);
                m_pauseCanvas.SetActive(true);
                m_gameOverCanvas.SetActive(false);
                break;

            case GameState.GameOver:
                m_hud.SetActive(false);
                m_pauseCanvas.SetActive(false);
                m_gameOverCanvas.SetActive(true);
                break;
        }
    }
}