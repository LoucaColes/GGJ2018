using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance = null;

    public List<Transform> m_safeTargets = new List<Transform>();
    
	void Start ()
    {
        CreateInstance();
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
}
