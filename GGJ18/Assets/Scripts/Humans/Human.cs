using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Human : MonoBehaviour
{
    public int m_ID = -1;
    public Vector2 m_moveCoefficents = new Vector2(1.0f, 1.0f);

    public bool m_inputActive = false;

    private Player m_player;
    private Rigidbody m_rb;

    public bool m_safe = true;

    public Human Initialise(int _ID)
    {
        m_ID = _ID;

        m_player = ReInput.players.GetPlayer(0);
        m_rb = GetComponent<Rigidbody>();

        return this;
    }
	
	void Update ()
    {
        if (m_inputActive)
        {
            m_rb.AddForce((-Vector3.left * (m_player.GetAxis("MoveHor") * m_moveCoefficents.x)) + (Vector3.forward * (m_player.GetAxis("MoveVert") * m_moveCoefficents.y)));
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        if(_other.gameObject.CompareTag("SafeZone"))
        {
            m_safe = true;
        }
    }

    void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.CompareTag("SafeZone"))
        {
            m_safe = false;
        }
    }
}
