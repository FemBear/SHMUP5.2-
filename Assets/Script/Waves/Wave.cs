using System.Collections;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField]
    private EnemyGroup[] enemyGroups;
    [SerializeField]
    private EnemyGroup bossGroup;
    [SerializeField]
    private bool isBossWave = false;
    [SerializeField]
    private float spawnInterval = 10f;

    private int currentGroupIndex = 0;
    public event System.Action OnWaveCompleted;

    public void StartWave()
    {
        StartCoroutine(SpawnEnemyGroups());
    }

    private IEnumerator SpawnEnemyGroups()
    {
        for (currentGroupIndex = 0; currentGroupIndex < enemyGroups.Length; currentGroupIndex++)
        {
            yield return new WaitForSeconds(1f);
            SpawnGroup(enemyGroups[currentGroupIndex]);
            yield return new WaitForSeconds(spawnInterval);
        }
        OnAllEnemyGroupsCompleted();
    }

    private void SpawnGroup(EnemyGroup group)
    {
        EnemyGroup enemyGroup = Instantiate(group);
        enemyGroup.transform.SetParent(transform);
        enemyGroup.OnAllEnemiesDestroyed += HandleAllEnemiesDestroyed;
        enemyGroup.StartGroup();
    }

    private void HandleAllEnemiesDestroyed()
    {
        currentGroupIndex++;
        if (currentGroupIndex >= enemyGroups.Length)
        {
            isBossWave = true;
            OnAllEnemyGroupsCompleted();
        }
        else
        {
            SpawnGroup(enemyGroups[currentGroupIndex]);
        }
    }

    private void OnAllEnemyGroupsCompleted()
    {
        if (isBossWave)
        {
            SpawnGroup(bossGroup);
            isBossWave = false;
        }
        else
        {
            OnWaveCompleted?.Invoke();
        }
    }
}