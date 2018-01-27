using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    GameOne,
    GameTwo
}

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager m_instance;

    private int m_gameOneHighScore;
    private int m_gameTwoHighScore;

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

    private void SetUpHighScore()
    {
        if (PlayerPrefs.HasKey("GameOneScore"))
        {
            m_gameOneHighScore = PlayerPrefs.GetInt("GameOneScore");
        }
        else
        {
            m_gameOneHighScore = 0;
            PlayerPrefs.SetInt("GameOneScore", 0);
        }

        if (PlayerPrefs.HasKey("GameTwoScore"))
        {
            m_gameTwoHighScore = PlayerPrefs.GetInt("GameTwoScore");
        }
        else
        {
            m_gameTwoHighScore = 0;
            PlayerPrefs.SetInt("GameTwoScore", 0);
        }
    }

    public void UpdateHighScore(GameType _game, int _score)
    {
        if (_game == GameType.GameOne)
        {
            if (_score > m_gameOneHighScore)
            {
                m_gameOneHighScore = _score;
                PlayerPrefs.SetInt("GameOneScore", m_gameOneHighScore);
            }
        }
        else
        {
            if (_score > m_gameTwoHighScore)
            {
                m_gameTwoHighScore = _score;
                PlayerPrefs.SetInt("GameTwoScore", m_gameTwoHighScore);
            }
        }
    }
}