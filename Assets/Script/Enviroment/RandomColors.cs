using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColors : MonoBehaviour
{
    private ParticleSystem[] m_ParticleSystems;
    private Color[] m_Colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black };
    [SerializeField]
    private float m_ChangeRate = 10f;

    void Start()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
        ChangeColor();
        StartCoroutine(ColorChange());
    }
    private void ChangeColor()
    {
        Color newColor = m_Colors[Random.Range(0, m_Colors.Length)];
        foreach (ParticleSystem ps in m_ParticleSystems)
        {
            var main = ps.main;
            main.startColor = newColor;
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
