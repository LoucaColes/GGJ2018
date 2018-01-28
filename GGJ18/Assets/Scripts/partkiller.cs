using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partkiller : MonoBehaviour
{
    public float m_killTime;

    // Use this for initialization
    private void Start()
    {
        Destroy(gameObject, m_killTime);
    }
}