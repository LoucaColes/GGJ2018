using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class Menu : MonoBehaviour
{
    public Button[] m_buttons;
    public int m_startIndex;
    private int m_buttonIndex;

    public Color m_enabledColour;
    public Color m_disabledColour;

    public float m_moveSpeed;
    private float m_timer;

    private Player m_player;
    private bool m_played;

    // Use this for initialization
    private void Start()
    {
        m_buttonIndex = m_startIndex;
        m_player = ReInput.players.GetPlayer(0);
        m_timer = 0;
        //AudioManager.AudioManager.m_instance.StopPlaying();
        //AudioManager.AudioManager.m_instance.PlayMusic(0);
        m_played = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (SceneLoader.m_instance.GetCurrentScene() == "GameOneMenu")
        {
            if (!m_played)
            {
                AudioManager.AudioManager.m_instance.PlayMusic("Music");
                m_played = true;
            }
        }
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
                int t = Random.Range(0, 12);
                AudioManager.AudioManager.m_instance.PlaySFX(t);
                m_buttonIndex++;
                if (m_buttonIndex >= m_buttons.Length)
                {
                    m_buttonIndex = 0;
                }
                m_timer = 0;
            }
            else if (m_player.GetAxis("MoveVert") < 0 || m_player.GetAxis("MoveHor") < 0)
            {
                int t = Random.Range(0, 12);
                AudioManager.AudioManager.m_instance.PlaySFX(t);

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
            int t = Random.Range(0, 12);
            AudioManager.AudioManager.m_instance.PlaySFX(t);
            switch (m_buttons[m_buttonIndex].name)
            {
                case "Game1Button":
                    SceneLoader.m_instance.LoadGameOneMenu();
                    break;

                case "GameOnePlayButton":
                    SceneLoader.m_instance.LoadGameOne();
                    break;

                case "GameOneReturnButton":
                case "GameTwoReturnButton":
                    SceneLoader.m_instance.LoadMainMenu();
                    break;

                case "Game2Button":
                    SceneLoader.m_instance.LoadGameTwoMenu();
                    break;

                case "GameTwoPlayButton":
                    SceneLoader.m_instance.LoadGameTwoCharacterSelect();
                    break;

                case "ExitButton":
                    SceneLoader.m_instance.ExitGame();
                    break;
            }
        }

        if (m_player.GetButtonDown("Back"))
        {
            int t = Random.Range(0, 12);
            AudioManager.AudioManager.m_instance.PlaySFX(t);

            switch (SceneLoader.m_instance.GetCurrentScene())
            {
                case "MainMenu":
                    SceneLoader.m_instance.ExitGame();
                    break;

                case "GameOneMenu":
                case "GameTwoMenu":
                    SceneLoader.m_instance.LoadMainMenu();
                    break;

                case "GameOneCharacterSelect":
                    SceneLoader.m_instance.LoadGameOneMenu();
                    break;

                case "GameTwoCharacterSelect":
                    SceneLoader.m_instance.LoadGameTwoMenu();
                    break;
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
}