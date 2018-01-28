using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance = null;

    public int m_maxZombiePopulation = 10;
    public List<Zombie> m_zombies = new List<Zombie>();
    public GameObject m_zombiePrefab;
    private Transform m_zombieHolder;

    public List<Transform> m_spawnPoints = new List<Transform>();
    public Vector2 m_spawnOffsetX = new Vector2(-5.0f, 5.0f);
    public Vector2 m_spawnOffsetZ = new Vector2(-5.0f, 5.0f);

    public float m_elapsedSpawningTime = 0.0f;
    public float m_spawningLength = 20.0f;
    public AnimationCurve m_zombieSpawnRate = new AnimationCurve();

    private Transform m_navTester;
    private NavMeshAgent m_agent;

    public GameObject m_mudPrefab;

    private IEnumerator m_delay;

    private bool m_hordeMode = false;
    
    void Start ()
    {
        CreateInstance();

        m_navTester = new GameObject("NavTester").transform;
        m_navTester.transform.SetParent(transform);
        m_agent = m_navTester.gameObject.AddComponent<NavMeshAgent>();

        m_zombieHolder = new GameObject("ZombieHolder").transform;
        m_zombieHolder.SetParent(transform);

        if (GameManager.m_instance != null)
        {
            if (GameManager.m_instance.m_enableTimer)
            {
                m_spawningLength = GameManager.m_instance.m_gameTime;
            }
        }

        SpawnZombiesRandom((int)(m_zombieSpawnRate.Evaluate(m_elapsedSpawningTime / m_spawningLength) * m_maxZombiePopulation));
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
	
	void Update ()
    {
        m_elapsedSpawningTime += Time.deltaTime;

        int zombiesToSpawn = (int)(m_zombieSpawnRate.Evaluate(m_elapsedSpawningTime / m_spawningLength) * m_maxZombiePopulation) - m_zombies.Count;

        if (zombiesToSpawn >= 1)
        {
            SpawnZombiesRandom(zombiesToSpawn);
        }

        if (GameManager.m_instance != null)
        {
            if (GameManager.m_instance.m_enableTimer)
            {
                if (!m_hordeMode)
                {
                    if (GameManager.m_instance.m_gameTimer < 0.0f)
                    {
                        m_hordeMode = true;
                        foreach (Zombie zom in m_zombies)
                        {
                            zom.SetHordeMode();
                        }
                    }
                }
            }
        }
    }

    public void SpawnZombiesRandom(int _zombiesToSpawn)
    {
        for(int zombieIter = 0; zombieIter < _zombiesToSpawn + 1; zombieIter++)
        {
            Vector3 pos = GetRandomPosition();
            Instantiate(m_mudPrefab, pos - new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(new Vector3(0, 0, 0)));
            GameObject zombie = Instantiate(m_zombiePrefab, pos, Quaternion.identity);
            m_zombies.Add(zombie.GetComponent<Zombie>());
            zombie.transform.SetParent(m_zombieHolder);
            zombie.name = "Zombie" + m_zombies.Count;

            if (m_hordeMode)
            {
                m_zombies[m_zombies.Count - 1].SetHordeMode();
            }

            GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_ZombieSpawned);
        }
    }

    public void SpawnZombieAtPosition(Vector3 _position)
    {
        Vector3 pos = GetRandomPosition();
        Instantiate(m_mudPrefab, pos - new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(new Vector3(0, 0, 0)));
        GameObject zombie = Instantiate(m_zombiePrefab, pos, Quaternion.identity);
        m_zombies.Add(zombie.GetComponent<Zombie>());
        zombie.transform.SetParent(m_zombieHolder);
        zombie.name = "Zombie" + m_zombies.Count;

        GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_ZombieSpawned);
    }

    public void SpawnZombieAtSpawnPoint(int _index, bool _useRand = false)
    {
        GameObject zombie = Instantiate(m_zombiePrefab, Vector3.zero, Quaternion.identity);
        if (_useRand)
        {
            zombie.transform.position = GetRandomPosition(_index);
        }
        else
        {
            zombie.transform.position = m_spawnPoints[_index].position;
        }
        Instantiate(m_mudPrefab, zombie.transform.position - new Vector3(0.0f, 1.0f, 0.0f), Quaternion.Euler(new Vector3(0, 0, 0)));

        m_zombies.Add(zombie.GetComponent<Zombie>());
        zombie.transform.SetParent(m_zombieHolder);
        zombie.name = "Zombie " + m_zombies.Count;

        GlobalEventBoard.Instance.AddRapidEvent(Event.ZOM_ZombieSpawned);
    }

    private Vector3 GetRandomPosition(int _index = -1)
    {
        int spawnIndex = _index;
        if (spawnIndex == -1)
        {
            spawnIndex = Random.Range(0, m_spawnPoints.Count);
        }

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

    public void RemoveZombie(Zombie _zombie)
    {
        m_zombies.Remove(_zombie);
        Destroy(_zombie.gameObject);

        SpawnZombiesRandom(1);
    }
}
