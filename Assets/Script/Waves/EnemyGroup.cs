using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyTypes;
    [SerializeField]
    private bool isBossGroup = false;

    private float spawnHeight;
    private List<GameObject> enemies = new List<GameObject>();
    public event System.Action OnAllEnemiesDestroyed;

    private void Awake()
    {
        spawnHeight = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 2;
    }

    public void StartGroup()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float offSetX = 8.886f;

        if (isBossGroup)
        {
            if (enemyTypes.Length > 0 && enemyTypes[0] != null)
            {
                GameObject boss = Instantiate(enemyTypes[0], new Vector3(offSetX, spawnHeight, 0), Quaternion.identity);
                boss.GetComponent<BaseEnemy>().SetActive();
                enemies.Add(boss);
            }
            yield break;
        }
        else
        {
            int formationIndex = Random.Range(0, 10);
            CreateFormation(formationIndex, offSetX);
        }

        StartCoroutine(CheckForAllEnemiesDestroyed());
    }

    private void CreateFormation(int formationIndex, float offSetX)
    {
        System.Func<int, Vector3> positionFunc = formationIndex switch
        {
            0 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight, 0),
            1 => (i) => new Vector3(offSetX, spawnHeight + i * 2.0f, 0),
            2 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight + i * 2.0f, 0),
            3 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight, 0),
            4 => (i) => new Vector3(offSetX + Mathf.Cos(i * 2.0f) * 2.0f, spawnHeight, 0),
            5 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight, 0),
            6 => (i) => new Vector3(offSetX + Mathf.Sin(i * 2.0f) * 2.0f, spawnHeight, 0),
            7 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight, 0),
            8 => (i) => new Vector3(offSetX + Mathf.Cos(i * 2.0f) * 2.0f, spawnHeight, 0),
            9 => (i) => new Vector3(offSetX + i * 2.0f, spawnHeight, 0),
            _ => (i) => new Vector3(offSetX, spawnHeight, 0)
        };

        int enemyCount = Random.Range(3, 6);
        GameObject formationParent = new GameObject("FormationParent");
        formationParent.transform.position = new Vector3(offSetX, spawnHeight, 0);
        formationParent.transform.SetParent(this.transform);
        StartCoroutine(SpawnEnemiesWithDelay(enemyCount, formationParent, positionFunc));
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
        if (enemyTypes.Length > 1)
        {
            int randomIndex = Random.Range(1, enemyTypes.Length);
            return enemyTypes[randomIndex];
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

            // Disable collisions temporarily
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
        yield return new WaitForSeconds(1.0f);

        foreach (GameObject enemy in enemies)
        {
            Collider2D collider = enemy.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
}
