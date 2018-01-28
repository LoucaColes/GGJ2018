﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        CheckingForHuman,
        MovingToHuman,
        Wandering,
        Stunned
    }

    public PointToPointAgent m_agent;

    public List<Transform> m_wonderPoints = new List<Transform>();

    public ZombieState m_state = ZombieState.Wandering;

    public float m_wanderPeriodLength = 1.0f;
    public float m_remainingWanderTime = 0.0f;

    public float m_maxDetectionDistance = 10.0f;

    public float m_chaseSpeed = 3.5f;
    public float m_wanderSpeed = 1.0f;

    public float m_stunTime;
    private float m_stunTimer;

    public GameObject m_bloodPsPref;
    public float m_eatingTime;

    private void Start()
    {
        m_agent = GetComponent<PointToPointAgent>();
        m_agent.m_tracking = true;
        m_stunTimer = 0;

        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanRemoved, Ev_HumanRemoved);
    }

    private void OnDestroy()
    {
        GlobalEventBoard.Instance.UnsubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
        GlobalEventBoard.Instance.UnsubscribeToEvent(Event.ZOM_HumanRemoved, Ev_HumanRemoved);
    }

    private void Update()
    {
        if (m_state == ZombieState.CheckingForHuman)
        {
            GetNextHuman();
        }
        else if ((m_state == ZombieState.MovingToHuman) && m_agent.AtDestination())
        {
            m_state = ZombieState.CheckingForHuman;
        }
        else if (m_state == ZombieState.Wandering)
        {
            m_remainingWanderTime -= Time.deltaTime;
            m_state = ZombieState.CheckingForHuman;
            GetNextHuman();

            if (m_remainingWanderTime <= 0.0f)
            {
                DoWondering();
            }
        }
        else if (m_state == ZombieState.Stunned)
        {
            m_stunTimer += Time.deltaTime;
            if (m_stunTimer > m_stunTime)
            {
                m_state = ZombieState.CheckingForHuman;
                m_stunTimer = 0;
            }
        }
    }

    public void Ev_HumanSpawned(object _data = null)
    {
        if (_data != null)
        {
            HumanSpawnedEvent data = (HumanSpawnedEvent)_data;

            m_agent.AddTarget(HumanManager.Instance.m_humans[data.HumanIndex].transform);
        }
    }

    public void Ev_HumanRemoved(object _data = null)
    {
        m_agent.ClearTargets();
        m_state = ZombieState.CheckingForHuman;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.CompareTag("Human") && m_state != ZombieState.Stunned)
        {
            ZombieManager.Instance.SpawnZombieAtPosition(_other.transform.position);
            Human human = _other.gameObject.GetComponent<Human>();
            HumanManager.Instance.RemoveHuman(human);
            Instantiate(m_bloodPsPref, _other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }

    public void Stun()
    {
        m_state = ZombieState.Stunned;
    }

    private void GetNextHuman()
    {
        Transform target = HumanManager.Instance.GetClosest(transform.position, m_maxDetectionDistance);
        if (target)
        {
            m_agent.ClearTargets();
            m_agent.AddTarget(target);
            m_agent.SetMoveSpeed(m_chaseSpeed);
            m_state = ZombieState.MovingToHuman;
        }
        else
        {
            m_state = ZombieState.Wandering;
        }

        m_agent.PathToCurrent();
    }

    private void DoWondering()
    {
        Vector3 position = transform.position;
        for (int wonderIter = 0; wonderIter < m_wonderPoints.Count; wonderIter++)
        {
            m_agent.ClearTargets();
            m_agent.AddTarget(m_wonderPoints[Random.Range(0, m_wonderPoints.Count - 1)]);

            if (m_agent.IsOnNavMesh())
            {
                break;
            }
        }

        if (position == transform.position)
        {
            position = WorldManager.Instance.m_safeTargets[Random.Range(0, WorldManager.Instance.m_safeTargets.Count - 1)].position;
        }

        m_agent.SetMoveSpeed(m_wanderSpeed);
        m_remainingWanderTime = m_wanderPeriodLength;
    }

    public void SetHordeMode()
    {
        if (m_agent == null)
        {
            m_agent = GetComponent<PointToPointAgent>();
        }
        m_agent.m_tracking = false;

        Transform target = HumanManager.Instance.GetClosest(transform.position, -1.0f, true);
        if (target)
        {
            m_agent.ClearTargets();
            m_agent.AddTarget(target);
            m_agent.SetMoveSpeed(m_chaseSpeed);
            m_state = ZombieState.MovingToHuman;
        }
        else
        {
            m_state = ZombieState.Wandering;
        }

        m_agent.PathToCurrent();
    }
}