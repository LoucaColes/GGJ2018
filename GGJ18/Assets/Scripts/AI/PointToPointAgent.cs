using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointToPointAgent : MonoBehaviour
{
    public List<Transform> m_targets = new List<Transform>();
    public float m_changeDistanceLimit = 0.3f;

    private NavMeshAgent m_agent;
    public int m_destinationIndex = 0;

    public bool m_looping = true;
    
	void Awake ()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.isStopped = false;
        //m_agent.SetDestination(m_targets[m_destinationIndex].position);
    }

    public bool AtDestination()
    {
        return m_agent.remainingDistance <= m_changeDistanceLimit;
    }

    /// <summary>
    /// Returns next index or -1 is hitting cap and not looping.
    /// </summary>
    /// <returns></returns>
    public int NextTargetIndex()
    {
        int next = m_destinationIndex + 1;
        if (next < m_targets.Count)
        {
            m_destinationIndex++;
            return m_destinationIndex;
        }
        else if(m_looping)
        {
            m_destinationIndex = 0;
            return m_destinationIndex;
        }
        return -1;
    }

    /// <summary>
    /// Returns previous index or -1 is hitting cap and not looping.
    /// </summary>
    /// <returns></returns>
    public int PreviousTargetIndex()
    {
        int prev = m_destinationIndex - 1;
        if (prev >= 0)
        {
            m_destinationIndex--;
            return m_destinationIndex;
        }
        else if (m_looping)
        {
            m_destinationIndex = m_targets.Count - 1;
            return m_destinationIndex;
        }
        return -1;
    }

    public void SetTargetIndex(int _index)
    {
        m_destinationIndex = _index;
    }

    public void SetDestination(Vector3 _destination)
    {
        m_agent.SetDestination(_destination);
    }

    public void AddTarget(Transform _target, int _insert = -1)
    {
        if(_insert != -1)
        {
            m_targets.Insert(_insert, _target);
        }
        else
        {
            m_targets.Add(_target);
        }
    }

    public void RemoveTarget(Transform _target)
    {
        m_targets.Remove(_target);
    }

    public void ClearTargets()
    {
        m_targets.Clear();
    }

    public void PathToCurrent()
    {
        if (m_targets.Count > 0)
        {
            m_agent.SetDestination(m_targets[m_destinationIndex].position);
        }
    }

    public bool IsOnNavMesh()
    {
        return m_agent.isOnNavMesh;
    }

    public float DistanceToDestination()
    {
        return m_agent.remainingDistance;
    }
}
