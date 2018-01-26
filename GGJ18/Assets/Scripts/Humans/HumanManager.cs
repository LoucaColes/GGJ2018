using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class HumanManager : MonoBehaviour
{
    public static HumanManager Instance = null;

    public int m_startingHumanPopulation;
    public List<Human> m_humans = new List<Human>();
    public GameObject m_humanPrefab;
    private Transform m_humanHolder;

    public List<Transform> m_spawnPoints = new List<Transform>();
    public Vector2 m_spawnOffsetX = new Vector2(-5.0f, 5.0f);
    public Vector2 m_spawnOffsetZ = new Vector2(-5.0f, 5.0f);

    private Transform m_navTester;
    private NavMeshAgent m_agent;

    private List<int> m_humanIDs = new List<int>();

    void Start ()
    {
        CreateInstance();

        m_navTester = new GameObject("NavTester").transform;
        m_navTester.transform.SetParent(transform);
        m_agent = m_navTester.gameObject.AddComponent<NavMeshAgent>();

        m_humanHolder = new GameObject("HumanHolder").transform;
        m_humanHolder.SetParent(transform);

        SpawnHumans(m_startingHumanPopulation);
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

    private void SpawnHumans(int _humansToSpawn)
    {
        for (int humanIter = 0; humanIter < _humansToSpawn; humanIter++)
        {
            GameObject human = Instantiate(m_humanPrefab, GetRandomPosition(), Quaternion.identity);
            m_humans.Add(human.GetComponent<Human>().Initialise(GenerateID()));
            human.transform.SetParent(m_humanHolder);
            human.name = "Human " + m_humans.Count;

            HumanSpawnedEvent data = new HumanSpawnedEvent(m_humans.Count - 1);
            GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_HumanSpawned, data);
        }
    }

    private Vector3 GetRandomPosition()
    {
        int spawnIndex = Random.Range(0, m_spawnPoints.Count - 1);
        Vector3 position = Vector3.zero;
        do
        {
            position = m_spawnPoints[spawnIndex].position + GetRandomOffset();
            m_navTester.transform.position = position;

        } while (!m_agent.isOnNavMesh);

        return position;
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(m_spawnOffsetX.x, m_spawnOffsetX.y), 0.0f, Random.Range(m_spawnOffsetZ.x, m_spawnOffsetZ.y));
    }

    public void RemoveHuman(Human _human)
    {
        int ID = _human.m_ID;
        m_humans.Remove(_human);
        m_humanIDs.Remove(ID);

        HumanRemovedEvent data = new HumanRemovedEvent(_human.transform);
        GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_HumanRemoved, data);

        Destroy(_human.gameObject);
    }

    private int GenerateID()
    {
        int ID = -1;

        do
        {
            ID = Random.Range(0, 1000);
        } while (m_humanIDs.Contains(ID));

        m_humanIDs.Add(ID);
        return ID;
    }
}
