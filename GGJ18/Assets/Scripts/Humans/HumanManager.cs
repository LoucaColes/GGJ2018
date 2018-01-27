using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Rewired;

public class HumanManager : MonoBehaviour
{
    public class SelectedHuman
    {
        public int m_selectedHumanIndex = 0;
        public int m_humanID = -1;

        public SelectedHuman(int _selectedHumanIndex, int _humanID)
        {
            m_selectedHumanIndex = _selectedHumanIndex;
            m_humanID = _humanID;
        }

        public void SetData(int _selectedHumanIndex, int _humanID)
        {
            m_selectedHumanIndex = _selectedHumanIndex;
            m_humanID = _humanID;
        }
    }

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

    public Player m_player;
    public SelectedHuman m_selectedHuman = new SelectedHuman(0, -1);

    public int m_humansSafe = 0;

    void Start ()
    {
        CreateInstance();

        m_navTester = new GameObject("NavTester").transform;
        m_navTester.transform.SetParent(transform);
        m_agent = m_navTester.gameObject.AddComponent<NavMeshAgent>();

        m_humanHolder = new GameObject("HumanHolder").transform;
        m_humanHolder.SetParent(transform);

        SpawnHumans(m_startingHumanPopulation);        
        if (m_humans.Count > 0)
        {
            m_selectedHuman.SetData(m_selectedHuman.m_selectedHumanIndex, m_humans[m_selectedHuman.m_selectedHumanIndex].m_ID);
            m_humans[m_selectedHuman.m_selectedHumanIndex].m_inputActive = true;
        }

        m_player = ReInput.players.GetPlayer(0);
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

    void Update()
    {
        if (m_humans.Count > 0 || m_selectedHuman.m_selectedHumanIndex != -1)
        {
            if (m_player.GetButtonDown("SwitchPos"))
            {
                IncreaseSelectedIndex();
            }
            else if (m_player.GetButtonDown("SwitchNeg"))
            {
                DecreaseSelectedIndex();
            }
        }
    }

    private void IncreaseSelectedIndex()
    {
        m_humans[m_selectedHuman.m_selectedHumanIndex].m_inputActive = false;

        m_selectedHuman.m_selectedHumanIndex++;
        if(m_selectedHuman.m_selectedHumanIndex >= m_humans.Count)
        {
            m_selectedHuman.m_selectedHumanIndex = 0;
        }
        m_selectedHuman.m_humanID = m_humans[m_selectedHuman.m_selectedHumanIndex].m_ID;

        m_humans[m_selectedHuman.m_selectedHumanIndex].m_inputActive = true;
    }

    private void DecreaseSelectedIndex()
    {
        if (m_humans.Count > 1)
        {
            m_humans[m_selectedHuman.m_selectedHumanIndex].m_inputActive = false;

            m_selectedHuman.m_selectedHumanIndex--;
            if (m_selectedHuman.m_selectedHumanIndex < 0)
            {
                m_selectedHuman.m_selectedHumanIndex = m_humans.Count - 1;
            }
            m_selectedHuman.m_humanID = m_humans[m_selectedHuman.m_selectedHumanIndex].m_ID;

            m_humans[m_selectedHuman.m_selectedHumanIndex].m_inputActive = true;
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
        
        Destroy(_human.gameObject);
        if (m_humans.Count > 0)
        {
            if (m_selectedHuman.m_selectedHumanIndex >= m_humans.Count)
            {
                m_selectedHuman.m_selectedHumanIndex = m_humans.Count - 1;
                m_selectedHuman.m_humanID = m_humans[m_selectedHuman.m_selectedHumanIndex].m_ID;
            }
            else
            {
                DecreaseSelectedIndex();
            }
        }
        else
        {
            m_selectedHuman.m_selectedHumanIndex = -1;
            m_selectedHuman.m_humanID = -1;
        }

        GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_HumanRemoved);
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

    public Transform GetClosest(Vector3 _position, float _maxDistance = -1.0f)
    {
        Transform closest = null;
        float shortestDistance = 0.0f;


        bool useMaxDistance = false;
        if(_maxDistance != -1.0f)
        {
            useMaxDistance = true;
        }
        
        foreach(Human human in m_humans)
        {
            RaycastHit hit;
            Physics.Raycast(_position, (human.transform.position - _position).normalized, out hit);

            if (hit.collider.gameObject.CompareTag("Human"))
            {
                if (closest == null)
                {
                    float distance = Vector3.Distance(_position, human.transform.position);
                    if (!useMaxDistance || (distance <= _maxDistance))
                    {
                        closest = human.transform;
                        shortestDistance = distance;
                    }
                }
                else
                {
                    float distance = Vector3.Distance(_position, human.transform.position);
                    if (!useMaxDistance || (distance <= _maxDistance))
                    {
                        if (distance < shortestDistance)
                        {
                            closest = human.transform;
                            shortestDistance = distance;
                        }
                    }
                }
            }
        }
        return closest;
    }

    public void HumanSafe(Human _human)
    {
        RemoveHuman(_human);
        m_humansSafe++;
    }
}
