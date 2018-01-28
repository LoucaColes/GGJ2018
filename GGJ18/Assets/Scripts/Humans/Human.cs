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
    
    public GameObject m_stunPsPref;

    public float m_rotationCoefficient = 1.0f;
    public float m_rotationMinimum = 10.0f;

    public Vector3 m_force;
    public float m_rotationAngle = 0.0f;
    public float m_rotationSign = 0.0f;

    public GameObject m_stunObject = null;

    public float m_fireTimerLength = 2.0f;
    public float m_rechargeTimerLength = 5.0f;

    public float m_remainingFireTime = 0.0f;
    public float m_remainingRechargeTime = 0.0f;

    public bool m_recharging = false;

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

            m_force = -Vector3.left * (m_player.GetAxis("MoveHor") * m_moveCoefficents.x) + (Vector3.forward * (m_player.GetAxis("MoveVert") * m_moveCoefficents.y));

            float deadzone = 0.25f;

            Vector2 stickInput = new Vector2(m_player.GetAxis("ShuntHor"), -m_player.GetAxis("ShuntVert"));
            if(stickInput.magnitude < deadzone)
            {
                stickInput = new Vector2(m_player.GetAxis("MoveHor"), -m_player.GetAxis("MoveVert"));
            }

            if (stickInput.magnitude < deadzone)
            {
                stickInput = Vector2.zero;
                m_rotationSign = 0.0f;
            }
            else
            {
                stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

                float currentAngle = gameObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                Vector2 currentDirection = Vector2.zero;
                currentDirection.x = Mathf.Cos(currentAngle);
                currentDirection.y = Mathf.Sin(currentAngle);
                

                Vector3 cross = Vector3.Cross(stickInput, currentDirection);
                m_rotationSign = Mathf.Sign(cross.z);

                m_rotationAngle = Vector2.Angle(stickInput, currentDirection);
                if (stickInput.y >= 0.0f)
                {
                    m_rotationAngle += 90.0f;
                }
                else
                {
                    m_rotationAngle -= 90.0f;
                    m_rotationAngle *= -1;
                }
                transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, m_rotationAngle, transform.eulerAngles.z));
            }

            if (m_player.GetButton("Stun") && !m_recharging)
            {
                m_remainingFireTime -= Time.deltaTime;
                if (m_stunObject == null)
                {
                    m_stunObject = Instantiate(m_stunPsPref, transform.position, Quaternion.Euler(new Vector3(0, m_rotationAngle, 0)));
                    m_stunObject.transform.SetParent(transform);
                    m_stunObject.GetComponent<partkiller>().m_killTime = m_fireTimerLength;
                    m_remainingFireTime = m_fireTimerLength;
                    m_remainingRechargeTime = m_rechargeTimerLength;
                }

                Collider[] t_collider = Physics.OverlapBox(m_point.position, m_boxHalf, Quaternion.identity);
                foreach (Collider collider in t_collider)
                {
                    if (collider.gameObject.layer == 8)
                    {
                        collider.gameObject.GetComponent<Zombie>().Stun();
                    }
                }

                if((m_remainingFireTime <= 0.0f))
                {
                    m_recharging = true;
                }
            }
            else if(m_stunObject != null && !m_recharging)
            {
                m_remainingFireTime -= Time.deltaTime;

                if ((m_remainingFireTime <= 0.0f))
                {
                    m_recharging = true;
                }
            }
            else if(m_recharging)
            {
                m_remainingRechargeTime -= Time.deltaTime;
                if ((m_remainingRechargeTime <= 0.0f))
                {
                    m_recharging = false;
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

    void FixedUpdate()
    {
        m_rb.AddForce(m_force);
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