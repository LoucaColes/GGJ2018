using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : MonoBehaviour
{
    public static GameManager m_instance;

    public GameType m_gameType;
    public GameState m_gameState;

    private Player m_player;

    private int m_score;

    public bool m_enableTimer;
    public float m_gameTime;
    private float m_gameTimer;

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

        m_player = ReInput.players.GetPlayer(0);
        m_gameState = GameState.Play;
        m_score = 0;
        m_gameTimer = m_gameTime;
        CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudScore(m_score);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_player.GetButtonDown("Pause") && m_gameState == GameState.Play)
        {
            m_gameState = GameState.Pause;
            CanvasManager.m_instance.UpdateCanvases(m_gameState);
        }

        if (m_enableTimer)
        {
            m_gameTimer -= Time.deltaTime;
            if (m_gameTimer > 0)
            {
                CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudTime(Mathf.RoundToInt(m_gameTimer));
            }
            else if (m_gameTimer <= 0)
            {
                m_gameState = GameState.GameOver;
                CanvasManager.m_instance.UpdateCanvases(m_gameState);
            }
        }
    }

    public void IncreaseScore(int _amount)
    {
        m_score += _amount;
        CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudScore(m_score);
    }
}