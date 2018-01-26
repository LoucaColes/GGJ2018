using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public PointToPointAgent m_agent;

	void Start ()
    {
        m_agent = GetComponent<PointToPointAgent>();

        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
        GlobalEventBoard.Instance.SubscribeToEvent(Event.ZOM_HumanRemoved, Ev_HumanRemoved);

        foreach (Human human in HumanManager.Instance.m_humans)
        {
            m_agent.AddTarget(human.transform);
        }

        m_agent.PathToCurrent();
    }
	
	void Update ()
    {
        GlobalEventBoard.Instance.UnsubscribeToEvent(Event.ZOM_HumanSpawned, Ev_HumanSpawned);
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

            m_agent.RemoveTarget(data.Human);
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
}
