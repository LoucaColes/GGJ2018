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
    public float m_gameTimer;

    public float m_sfxTimeMin;
    public float m_sfxTimeMax;
    private float m_sfxTime;
    private float m_sfxTimer;

    // Use this for initialization
    private void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_player = ReInput.players.GetPlayer(0);
        m_gameState = GameState.Play;
        m_score = 0;
        m_gameTimer = m_gameTime;
        AudioManager.AudioManager.m_instance.PlayMusic(0);
        m_sfxTimer = 0;
        m_sfxTime = Random.Range(m_sfxTimeMin, m_sfxTimeMax);
        //CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudScore(m_score);
    }

    // Update is called once per frame
    private void Update()
    {
        if (HumanManager.Instance.m_humans.Count == 0)
        {
            m_gameState = GameState.GameOver;
            CanvasManager.m_instance.UpdateCanvases(m_gameState);
        }

        if (m_player.GetButtonDown("Pause") && m_gameState == GameState.Play)
        {
            m_gameState = GameState.Pause;
            CanvasManager.m_instance.UpdateCanvases(m_gameState);
        }

        if (m_gameState == GameState.Play)
        {
            CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudScore(m_score);
            m_sfxTimer += Time.deltaTime;
            if (m_sfxTimer > m_sfxTime)
            {
                int t = Random.Range(0, 12);
                float pan = Random.Range(-1, 1);
                AudioManager.AudioManager.m_instance.UpdateSoundVariables(AudioManager.AudioData.AudioType.SFX, t, 1, 1, false, pan, 0, 0, 0);
                AudioManager.AudioManager.m_instance.PlaySFX(t);
                m_sfxTime = Random.Range(m_sfxTimeMin, m_sfxTimeMax);
                m_sfxTimer = 0;
            }
        }

        if (m_enableTimer)
        {
            if (m_gameTimer > 0)
            {
                m_gameTimer -= Time.deltaTime;
                CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudTime(Mathf.RoundToInt(m_gameTimer));
            }
            //else if (m_gameTimer <= 0)
            //{
            //    m_gameState = GameState.GameOver;
            //    CanvasManager.m_instance.UpdateCanvases(m_gameState);
            //}
        }
    }

    public void IncreaseScore(int _amount)
    {
        m_score += _amount;
        CanvasManager.m_instance.m_hud.GetComponent<HUDCanvas>().UpdateHudScore(m_score);
    }
}