using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class GameOverCanvas : MonoBehaviour
{
    public Text m_scoreText;

    public Button[] m_buttons;
    public int m_startIndex;
    private int m_buttonIndex;

    public Color m_enabledColour;
    public Color m_disabledColour;

    public float m_moveSpeed;
    private float m_timer;

    private Player m_player;

    // Use this for initialization
    private void Start()
    {
        m_buttonIndex = m_startIndex;
        m_player = ReInput.players.GetPlayer(0);
        m_timer = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        InputHandling();
        UpdateUI();
    }

    private void InputHandling()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_moveSpeed)
        {
            if (m_player.GetAxis("MoveVert") > 0 || m_player.GetAxis("MoveHor") > 0)
            {
                m_buttonIndex++;
                if (m_buttonIndex >= m_buttons.Length)
                {
                    m_buttonIndex = 0;
                }
                m_timer = 0;
            }
            else if (m_player.GetAxis("MoveVert") < 0 || m_player.GetAxis("MoveHor") < 0)
            {
                m_buttonIndex--;
                if (m_buttonIndex < 0)
                {
                    m_buttonIndex = m_buttons.Length - 1;
                }
                m_timer = 0;
            }
        }

        if (m_player.GetButtonDown("Accept"))
        {
            switch (m_buttons[m_buttonIndex].name)
            {
                case "ReplayButton":
                    if (GameManager.m_instance.m_gameType == GameType.GameOne)
                    {
                        SceneLoader.m_instance.LoadGameOne();
                    }
                    else
                    {
                        SceneLoader.m_instance.LoadGameTwo();
                    }
                    break;

                case "ExitButton":
                    if (GameManager.m_instance.m_gameType == GameType.GameOne)
                    {
                        SceneLoader.m_instance.LoadGameOneMenu();
                    }
                    else
                    {
                        SceneLoader.m_instance.LoadGameTwoMenu();
                    }
                    break;
            }
        }

        if (m_player.GetButtonDown("Back"))
        {
            if (GameManager.m_instance.m_gameType == GameType.GameOne)
            {
                SceneLoader.m_instance.LoadGameOneMenu();
            }
            else
            {
                SceneLoader.m_instance.LoadGameTwoMenu();
            }
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < m_buttons.Length; i++)
        {
            if (i == m_buttonIndex)
            {
                if (!m_buttons[i].enabled)
                {
                    m_buttons[i].enabled = true;
                    m_buttons[i].image.color = m_enabledColour;
                }
            }
            else
            {
                if (m_buttons[i].enabled)
                {
                    m_buttons[i].enabled = false;
                    m_buttons[i].image.color = m_disabledColour;
                }
            }
        }
    }

    public void HideText()
    {
        m_scoreText.text = "";
    }

    public void UpdateScoreText(int _score)
    {
        m_scoreText.text = "You Scored: " + _score;
    }
}