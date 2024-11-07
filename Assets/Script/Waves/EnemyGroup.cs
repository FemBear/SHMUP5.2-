using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_enemyTypes;
    [SerializeField]
    private bool m_IsBossGroup = false;

    private float m_SpawnHeight;
    private List<GameObject> enemies = new List<GameObject>();
    public event System.Action OnAllEnemiesDestroyed;

    private void Awake()
    {
        m_SpawnHeight = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 5;
    }

    public void StartGroup()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float offSetX = 0f;

        if (m_IsBossGroup)
        {
            if (m_enemyTypes.Length > 0 && m_enemyTypes[0] != null)
            {
                GameObject boss = Instantiate(m_enemyTypes[0], new Vector3(offSetX, m_SpawnHeight, 0), Quaternion.identity);
                boss.GetComponent<BaseEnemy>().SetActive();
                enemies.Add(boss);
            }
            yield break;
        }
        else
        {
            int formationIndex = Random.Range(0, 5);
            CreateFormation(formationIndex, offSetX);
        }

        StartCoroutine(CheckForAllEnemiesDestroyed());
    }

    private void CreateFormation(int formationIndex, float offSetX)
    {
        switch (formationIndex)
        {
            case 0:
                CreateLineFormation(offSetX);
                break;
            case 1:
                CreateVFormation(offSetX);
                break;
            case 2:
                CreateCircleFormation(offSetX);
                break;
            case 3:
                CreateSquareFormation(offSetX);
                break;
            case 4:
                CreateTriangleFormation(offSetX);
                break;
        }
    }

    private void CreateLineFormation(float offSetX)
    {
        int enemyCount = Random.Range(3, 6);
        GameObject formationParent = new GameObject("LineFormation");
        formationParent.transform.position = new Vector3(offSetX, m_SpawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, i => new Vector3(offSetX, m_SpawnHeight - i * 2.5f, 0)));
    }

    private void CreateVFormation(float offSetX)
    {
        int enemyCount = Random.Range(3, 6);
        GameObject formationParent = new GameObject("VFormation");
        formationParent.transform.position = new Vector3(offSetX, m_SpawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, i => new Vector3(offSetX + (i - enemyCount / 2) * 2.5f, m_SpawnHeight - Mathf.Abs(i - enemyCount / 2) * 2.5f, 0)));
    }

    private void CreateCircleFormation(float offSetX)
    {
        int enemyCount = Random.Range(3, 6);
        GameObject formationParent = new GameObject("CircleFormation");
        formationParent.transform.position = new Vector3(offSetX, m_SpawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, i => new Vector3(offSetX + Mathf.Cos(2 * Mathf.PI * i / enemyCount) * 3, m_SpawnHeight + Mathf.Sin(2 * Mathf.PI * i / enemyCount) * 3, 0)));
    }

    private void CreateSquareFormation(float offSetX)
    {
        int enemyCount = 4;
        GameObject formationParent = new GameObject("SquareFormation");
        formationParent.transform.position = new Vector3(offSetX, m_SpawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, i => new Vector3(offSetX + i % 2 * 3, m_SpawnHeight - i / 2 * 3, 0)));
    }

    private void CreateTriangleFormation(float offSetX)
    {
        int enemyCount = 6;
        GameObject formationParent = new GameObject("TriangleFormation");
        formationParent.transform.position = new Vector3(offSetX, m_SpawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, i => new Vector3(offSetX + i % 3 * 2.5f, m_SpawnHeight - i / 3 * 2.5f, 0))); // Increased spacing
    }

    private IEnumerator CheckForAllEnemiesDestroyed()
    {
        while (enemies.Count > 0)
        {
            yield return new WaitForSeconds(1.0f);
        }
        OnAllEnemiesDestroyed?.Invoke();
        Destroy(this.gameObject);
    }

    private GameObject GetRandomEnemyType()
    {
        if (m_enemyTypes.Length > 1)
        {
            int randomIndex = Random.Range(1, m_enemyTypes.Length);
            return m_enemyTypes[randomIndex];
        }
        return null;
    }

    private IEnumerator SpawnEnemiesWithDelay(int enemyCount, GameObject formationParent, System.Func<int, Vector3> positionFunc)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = positionFunc(i);
            GameObject enemy = Instantiate(GetRandomEnemyType(), spawnPosition, Quaternion.identity, formationParent.transform);
            enemies.Add(enemy);

            Collider2D collider = enemy.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(EnableCollisions(enemies));
        }
    }

    private IEnumerator EnableCollisions(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            Collider2D collider = enemy.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
        yield return null;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
}
