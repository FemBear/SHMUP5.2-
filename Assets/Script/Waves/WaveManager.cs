using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;

    private int currentWaveIndex = 0;

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

            if (currentWaveIndex >= waves.Length - 1)
            {
                yield break;
            }
        }
    }

    public void NextWave()
    {
        currentWaveIndex = (currentWaveIndex + 1) % waves.Length;
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        StartWave();
    }

    private void StartWave()
    {
        Wave currentWave = Instantiate(waves[currentWaveIndex], transform.position, Quaternion.identity, transform);
        currentWave.OnWaveCompleted += HandleWaveCompleted;
        currentWave.StartWave();
        GameManager.Instance.m_Wave++;
    }

    private void HandleWaveCompleted()
    {
        NextWave();
    }
}