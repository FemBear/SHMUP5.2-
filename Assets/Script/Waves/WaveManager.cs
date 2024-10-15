using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{

    [SerializeField]
    private Wave[] m_Waves;
    [SerializeField]
    private int m_CurrentWave = 0;
    [SerializeField]
    private int m_WaveCount = 0;
    private Wave m_Wave;


    void Start()
    {
        m_WaveCount = m_Waves.Length;
        if (m_Waves.Length == 0)
        {
            Debug.LogError("No waves found");
        }
        m_Wave = m_Waves[m_CurrentWave];
        StartWave(m_CurrentWave);
    }
    

    private void StartWave(int waveIndex)
    {
        Instantiate(m_Waves[waveIndex]);
    }

    public void NextWave()
    {
        m_CurrentWave++;
        if (m_CurrentWave < m_WaveCount)
        {
            m_Wave = m_Waves[m_CurrentWave];
            StartWave(m_CurrentWave);
        }
        else
        {
            //End game
            Debug.Log("No more waves");
        }
    }
}

