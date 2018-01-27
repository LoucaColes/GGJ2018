using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupTrolley : MonoBehaviour
{
    public static CupTrolley Instance = null;

    private PointToPointAgent m_agent;

    public Vector3 m_force = Vector3.zero;
    public float m_forceCoefficient = 5.0f;
    
	void Start ()
    {
        CreateInstance();

        m_agent = GetComponent<PointToPointAgent>();
        m_agent.PathToCurrent();
    }

    private void CreateInstance()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void LateUpdate ()
    {
        m_force = m_agent.GetForce() * m_forceCoefficient;

    }
}
