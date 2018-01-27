using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public float m_waitPercentage = 0.5f;

	void Update ()
    {
        if (GameManager.m_instance.m_enableTimer)
        {
            if (GameManager.m_instance.m_gameTimer < GameManager.m_instance.m_gameTime * m_waitPercentage)
            {
                GetComponent<BoxCollider>().enabled = false;
                enabled = false;
            }
        }
	}
}
