using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColors : MonoBehaviour
{
    private ParticleSystem[] m_ParticleSystems;

    [SerializeField]
    private float m_ChangeRate = 1;

    void Start()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
        ChangeColor();
        StartCoroutine(ColorChange());
    }
    private void ChangeColor()
    {
        foreach (var ps in m_ParticleSystems)
        {
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Random.ColorHSV());
        }
    }
    IEnumerator ColorChange()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_ChangeRate);
            ChangeColor();
        }
    }
}
