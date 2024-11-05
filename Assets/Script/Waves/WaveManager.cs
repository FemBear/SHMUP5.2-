using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField]
    private Wave[] waves;

    private int currentWaveIndex = 0;

    new void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        if (waves.Length > 0)
        {
            StartCoroutine(WaveRoutine());
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            StartWave();
            yield return new WaitForSeconds(30f);
        }
    }

    public void NextWave()
    {
        currentWaveIndex = (currentWaveIndex + 1) % waves.Length;
        Destroy(transform.GetChild(0).gameObject);
        StartWave();
    }

    private void StartWave()
    {
        Wave currentWave = Instantiate(waves[currentWaveIndex], transform.position, Quaternion.identity, transform.parent);
        currentWave.OnWaveCompleted += HandleWaveCompleted;
        currentWave.StartWave();
        GameManager.Instance.m_Wave++;
    }

    private void HandleWaveCompleted()
    {
        NextWave();
    }
}