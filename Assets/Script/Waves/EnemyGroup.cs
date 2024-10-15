using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : ScriptableObject
{
    [SerializeField]
    private GameObject EnemyGroupPrefab;

    public void SpawnGroup()
    {
        Instantiate(EnemyGroupPrefab);
    }
}
