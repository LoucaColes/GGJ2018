using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour
{
    public Text m_scoreText;
    public Text m_timerText;

    // Use this for initialization
    private void Start()
    {
        HideText();
    }

    public void UpdateHudScore(int _score)
    {
        m_scoreText.text = "Score: " + _score;
    }

    public void UpdateHudTime(int _time)
    {
        m_timerText.text = "Time: " + _time;
    }

    public void HideText()
    {
        m_scoreText.text = "";
        m_timerText.text = "";
    }
}