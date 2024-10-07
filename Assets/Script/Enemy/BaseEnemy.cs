using System.Collections;
using JetBrains.Annotations;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : Plane
{
    [SerializeField]
    protected GameObject m_Target;
    [SerializeField][CanBeNull]
    protected GameObject m_ExplosionFX;
    [Header("PowerUp Settings")]
    [SerializeField][CanBeNull]
    protected GameObject[] m_PowerUp;
    [SerializeField]
    protected int m_DropRate = 10;
    protected void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Target = GameObject.Find("Player");
        m_Speed = 1 + (GameManager.Instance.m_Wave / 4);
    }

    protected void PowerUp()
    {
        if (Random.Range(0, 100) < m_DropRate && m_PowerUp.Length > 0)
        {
            Instantiate(m_PowerUp[Random.Range(0, m_PowerUp.Length)], transform.position, Quaternion.identity);
        }
    }

    void OnDestroy()
    {
        PowerUp();
        if (m_ExplosionFX != null)
        {
            Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
        }
    }
}