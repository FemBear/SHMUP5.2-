using System.Collections;
using UnityEngine;

public class BaseEnemy : Plane
{
    public enum EnemyState
    {
        Spawning,
        Active,
        Dead
    }

    public EnemyState m_EnemyState;
    [SerializeField]
    protected GameObject m_Target;
    [SerializeField]
    protected int m_Score = 10;
    [Header("PowerUp Settings")]
    [SerializeField]
    protected GameObject[] m_PowerUp;
    [SerializeField]
    protected int m_DropRate = 10;
    protected ScreenWrapper m_ScreenWrapper;
    private int m_RandomDropRate;

    public virtual void Start()
    {
        Initialize();
    }

    public virtual void Update()
    {
        HandleEnemyState();
        if(m_CanFire <= 0)
        {
            m_CanFire = 0;
        }
    }

    protected void OnDestroy()
    {
        AudioManager.Instance.PlaySFX(m_DeathClip);
        EnemyGroup enemyGroup = FindObjectOfType<EnemyGroup>();
        if (enemyGroup != null)
        {
            enemyGroup.RemoveEnemy(gameObject);
        }
        GameManager.Instance.AddScore(m_Score);
        PowerUp();
    }

    protected void PowerUp()
    {
        m_RandomDropRate = Random.Range(0, 100);
        if (m_RandomDropRate < m_DropRate && m_PowerUp.Length > 0)
        {
            Instantiate(m_PowerUp[Random.Range(0, m_PowerUp.Length)], transform.position, Quaternion.identity);
        }
    }

    private void HandleEnemyState()
    {
        switch (m_EnemyState)
        {
            case EnemyState.Spawning:
                if (EnemyIsOnScreen(gameObject))
                {
                    m_EnemyState = EnemyState.Active;
                }
                else
                {
                    StartCoroutine(MoveOnScreen(gameObject, new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height - 2, 0)).y)));
                }
                break;
            case EnemyState.Active:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    public void SetActive()
    {
        m_EnemyState = EnemyState.Active;
    }

    private bool EnemyIsOnScreen(GameObject enemy)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(enemy.transform.position);
        Vector3 spriteBounds = enemy.GetComponent<SpriteRenderer>().bounds.extents;
        return screenPoint.x > 0 + spriteBounds.x / Screen.width && screenPoint.x < 1 - spriteBounds.x / Screen.width &&
               screenPoint.y > 0 + spriteBounds.y / Screen.height && screenPoint.y < 1 - spriteBounds.y / Screen.height;
    }

    private IEnumerator MoveOnScreen(GameObject enemy, Vector3 targetPosition)
    {
        float speed = 0.5f;
        while (!EnemyIsOnScreen(enemy))
        {
            enemy.transform.position = Vector3.Lerp(enemy.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    protected virtual IEnumerator ResetRotation()
    {
        while (Mathf.Abs(transform.rotation.z) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 180), Time.deltaTime * 5);
            yield return null;
        }
    }

    protected virtual void Initialize()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_BulletSpawn = transform.GetChild(0);
        m_ShootClip = Resources.Load<AudioClip>($"Sound/Effects/Shoot");
        m_DeathClip = Resources.Load<AudioClip>($"Sound/Effects/Death");
        m_Target = GameObject.FindGameObjectWithTag("Player");
        transform.rotation = Quaternion.Euler(0, 0, 180);
        m_EnemyState = EnemyState.Spawning;
        m_CanFire = m_FireRate;
    }
}