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
    private Transform m_point;
    public Vector3 m_boxHalf;

    private GameObject m_cube;

    public bool m_safe = true;

    public Human Initialise(int _ID)
    {
        m_ID = _ID;

        m_player = ReInput.players.GetPlayer(0);
        m_rb = GetComponent<Rigidbody>();
        m_point = transform.GetChild(0);
        m_cube = transform.GetChild(1).gameObject;
        m_cube.SetActive(false);

        return this;
    }

    private void Update()
    {
        if (m_inputActive)
        {
            if (!m_cube.activeSelf)
            {
                m_cube.SetActive(true);
            }
            m_rb.AddForce((-Vector3.left * (m_player.GetAxis("MoveHor") * m_moveCoefficents.x)) + (Vector3.forward * (m_player.GetAxis("MoveVert") * m_moveCoefficents.y)));

            if (m_player.GetButtonDown("Stun"))
            {
                Collider[] t_collider = Physics.OverlapBox(m_point.position, m_boxHalf, Quaternion.identity);
                foreach (Collider collider in t_collider)
                {
                    if (collider.gameObject.layer == 8)
                    {
                        Debug.Log("Happening");
                        collider.gameObject.GetComponent<Zombie>().Stun();
                    }
                }
            }
        }
        else
        {
            if (m_cube.activeSelf)
            {
                m_cube.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag("SafeZone"))
        {
            m_safe = true;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.CompareTag("SafeZone"))
        {
            m_safe = false;
        }
    }
}