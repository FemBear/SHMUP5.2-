using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class BaseEnemy : Plane
{
    [SerializeField]
    protected enum EnemyState
    {
        Spawning,
        Active,
        Dead
    }
    protected EnemyState m_EnemyState;
    int m_Down = 5;
    [SerializeField]
    protected GameObject m_Target;
    [SerializeField][CanBeNull]
    protected GameObject m_ExplosionFX;
    [SerializeField]
    protected int m_Score = 10;
    [Header("PowerUp Settings")]
    [SerializeField][CanBeNull]
    protected GameObject[] m_PowerUp;
    [SerializeField]
    protected int m_DropRate = 10;
    protected ScreenWrapper m_ScreenWrapper;
    public virtual void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_ScreenWrapper = GetComponent<ScreenWrapper>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Target = GameObject.Find("Player");
        m_Speed = 1f + (GameManager.Instance.m_Wave / 4f);
        transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y - m_Down), Time.deltaTime);
    }

    protected void PowerUp()
    {
        if (Random.Range(0, 100) < m_DropRate && m_PowerUp.Length > 0)
        {
            Instantiate(m_PowerUp[Random.Range(0, m_PowerUp.Length)], transform.position, Quaternion.identity);
        }
    }

    private void AddScore()
    {
        GameManager.Instance.m_Score += m_Score;
    }

    protected void OnDestroy()
    {
        if (m_ExplosionFX != null)
        {
            Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
        }
        AddScore();
        PowerUp();
    }
}