using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        CheckingForHuman,
        MovingToHuman,
        Wandering
    }

    public PointToPointAgent m_agent;

    public List<Transform> m_wonderPoints = new List<Transform>();

    public ZombieState m_state = ZombieState.Wandering;

    public float m_wanderPeriodLength = 100f;
    private float m_remainingWanderTime = 0.0f;

    void Start ()
    {
        m_agent = GetComponent<PointToPointAgent>();

        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanRemoved, Ev_HumanRemoved);
    }

    private void OnDestroy()
    {
        GlobalEventBoard.Instance.UnsubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
        GlobalEventBoard.Instance.UnsubscribeToEvent(Event.ZOM_HumanRemoved, Ev_HumanRemoved);
    }

    void Update ()
    {
        if(m_state == ZombieState.CheckingForHuman)
        {
            GetNextHuman();
        }
        else if((m_state == ZombieState.MovingToHuman) && m_agent.AtDestination())
        {
            m_state = ZombieState.CheckingForHuman;
        }
        else if(m_state == ZombieState.Wandering)
        {
            m_remainingWanderTime -= Time.deltaTime;

            if (m_remainingWanderTime <= 0.0f)
            {
                m_state = ZombieState.CheckingForHuman;
            }
        }
    }

    public void Ev_HumanSpawned(object _data = null)
    {
        if(_data != null)
        {
            HumanSpawnedEvent data = (HumanSpawnedEvent)_data;

            m_agent.AddTarget(HumanManager.Instance.m_humans[data.HumanIndex].transform);
        }
    }

    public void Ev_HumanRemoved(object _data = null)
    {
        if (_data != null)
        {
            HumanRemovedEvent data = (HumanRemovedEvent)_data;

            m_agent.ClearTargets();
            m_state = ZombieState.CheckingForHuman;
        }
    }

    void OnCollisionEnter(Collision _other)
    {
        if(_other.gameObject.CompareTag("Human"))
        {
            ZombieManager.Instance.SpawnZombieAtPosition(_other.transform.position);

            Human human = _other.gameObject.GetComponent<Human>();
            HumanManager.Instance.RemoveHuman(human);
        }
    }

    private void GetNextHuman()
    {
        Transform target = HumanManager.Instance.GetClosest(transform.position);
        if (target)
        {
            m_agent.AddTarget(target);
            m_state = ZombieState.MovingToHuman;
        }
        else
        {
            DoWondering();
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

        if(position == transform.position)
        {
            position = WorldManager.Instance.m_safeTargets[Random.Range(0, WorldManager.Instance.m_safeTargets.Count - 1)].position;
        }

        m_remainingWanderTime = m_wanderPeriodLength;
    }
}
