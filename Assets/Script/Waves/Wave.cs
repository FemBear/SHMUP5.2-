using System.Collections;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField]
    internal EnemyGroup[] m_EnemyGroups;

    [SerializeField]
    internal int m_EnemyCount = 0;
    [SerializeField]
    internal float m_SpawnInterval = 30f;
    void Start()
    {
        //find all enemies on the scene
        m_EnemyCount = FindObjectsOfType<BaseEnemy>().Length;
        if (m_EnemyCount == 0)
        {
            Debug.LogError("No enemies found");
        }
        StartWave();
    }

    private void StartWave()
    {
        StartCoroutine(SpawnEnemies());
    }


    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < m_EnemyGroups.Length; i++)
        {
            m_EnemyGroups[i].SpawnGroup();
            yield return new WaitForSeconds(m_SpawnInterval);
            if (i == m_EnemyGroups.Length - 1)
            {
                WaveManager.Instance.NextWave();
                Destroy(this.gameObject);
            }
        }
    }
}
