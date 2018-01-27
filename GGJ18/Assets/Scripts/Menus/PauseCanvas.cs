using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PauseCanvas : MonoBehaviour
{
    public Button[] m_buttons;
    public Image[] m_buttonImages;
    public int m_startIndex;
    private int m_buttonIndex;

    public Sprite m_sfxMuteImage;
    public Sprite m_sfxUnmuteImage;
    public Sprite m_musicMuteImage;
    public Sprite m_musicUnmuteImage;

    public Color m_enabledColour;
    public Color m_disabledColour;

    public float m_moveSpeed;
    private float m_timer;

    private Player m_player;
    public GameType m_gameType;

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
                case "PlayButton":
                    CanvasManager.m_instance.UpdateCanvases(GameState.Play);
                    break;

                case "SFXMuteButton":
                    if (m_buttons[m_buttonIndex].image.sprite == m_sfxUnmuteImage)
                    {
                        //AudioManager.AudioManager.m_instance.SetSfxGlobalVolume(0);
                        m_buttons[m_buttonIndex].image.sprite = m_sfxMuteImage;
                    }
                    else
                    {
                        //AudioManager.AudioManager.m_instance.SetSfxGlobalVolume(1);
                        m_buttons[m_buttonIndex].image.sprite = m_sfxUnmuteImage;
                    }
                    break;

                case "MusicMuteButton":
                    if (m_buttons[m_buttonIndex].image.sprite == m_musicUnmuteImage)
                    {
                        //AudioManager.AudioManager.m_instance.SetSfxGlobalVolume(0);
                        m_buttons[m_buttonIndex].image.sprite = m_musicMuteImage;
                    }
                    else
                    {
                        //AudioManager.AudioManager.m_instance.SetSfxGlobalVolume(1);
                        m_buttons[m_buttonIndex].image.sprite = m_musicUnmuteImage;
                    }
                    break;

                case "ExitButton":
                    if (m_gameType == GameType.GameOne)
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
            CanvasManager.m_instance.UpdateCanvases(GameState.Play);
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
                    m_buttonImages[i].color = m_enabledColour;
                }
            }
            else
            {
                if (m_buttons[i].enabled)
                {
                    m_buttons[i].enabled = false;
                    m_buttonImages[i].color = m_disabledColour;
                }
            }
        }
    }
}