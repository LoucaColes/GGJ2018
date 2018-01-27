using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Cup : MonoBehaviour
{
    [HideInInspector]
    public bool m_initialised = false;

    private Player m_player;
    private Rigidbody m_rb;

    //public Transform m_waterInPoint;
    //public GameObject m_dropletPrefab;

    //public int m_numberOfWaterDroplets = 1000;
    //private int m_remainingDroplets = 0;
    //public int m_dropletsPerFrame = 10;

    public List<GameObject> m_waterDroplets = new List<GameObject>();

    public Vector2 m_moveCoefficents = new Vector2(1.0f, 1.0f);

    public Transform m_tiltPoint;
    public Transform m_shuntPoint;

    void Start ()
    {
        m_player = ReInput.players.GetPlayer(0);
        m_rb = GetComponent<Rigidbody>();
        //m_remainingDroplets = m_numberOfWaterDroplets;
    }
	
    //private void FillCup()
    //{
    //    int dropletsThisFrame = m_dropletsPerFrame;
    //    if(dropletsThisFrame > m_remainingDroplets)
    //    {
    //        dropletsThisFrame = m_remainingDroplets;
    //    }

    //    for(int dropletIter = 0; dropletIter < m_dropletsPerFrame; dropletIter++)
    //    {
    //        m_waterDroplets.Add(Instantiate(m_dropletPrefab, m_waterInPoint.position, Quaternion.identity));
    //    }

    //    m_remainingDroplets -= dropletsThisFrame;
    //}

	void FixedUpdate ()
    {
        Vector3 tiltForce = (-Vector3.left * (m_player.GetAxis("MoveHor") * m_moveCoefficents.x)) + (Vector3.forward * (m_player.GetAxis("MoveVert") * m_moveCoefficents.y));
        ApplyForce(tiltForce, m_tiltPoint.position);

        Vector3 shuntForce = (-Vector3.left * (m_player.GetAxis("ShuntHor") * m_moveCoefficents.x)) + (Vector3.forward * (m_player.GetAxis("ShuntVert") * m_moveCoefficents.y));
        ApplyForce(shuntForce, m_shuntPoint.position);

        m_rb.AddForce(CupTrolley.Instance.m_force, ForceMode.Impulse);
    }

    public void ApplyForce(Vector3 _force, Vector3 _offset)
    {
        m_rb.AddForceAtPosition(_force, transform.position, ForceMode.Impulse);
    }
}
