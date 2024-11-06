using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEnemy : Plane
{
    [SerializeField]
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
    [CanBeNull]
    protected GameObject[] m_PowerUp;
    [SerializeField]
    protected int m_DropRate = 10;
    protected ScreenWrapper m_ScreenWrapper;

    [Header("Audio")]
    [SerializeField]
    protected AudioClip attackClip;
    [SerializeField]
    protected AudioClip deathClip;

    public virtual void Start()
    {
        this.gameObject.tag = "Enemy";
        m_Rb = GetComponent<Rigidbody2D>();
        m_ScreenWrapper = GetComponent<ScreenWrapper>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Target = GameObject.Find("Player");
        m_EnemyState = EnemyState.Spawning;
        transform.rotation = Quaternion.Euler(0, 0, 180);
        m_PowerUp = Resources.LoadAll<GameObject>("Powerups");
        attackClip = Resources.Load<AudioClip>($"Sound/Effects/Shoot");
        deathClip = Resources.Load<AudioClip>($"Sound/Effects/Death");
    }


    public override void Update()
    {
        base.Update();
        switch (m_EnemyState)
        {
            case EnemyState.Spawning:
                if (EnemyIsOnScreen(gameObject))
                {
                    m_EnemyState = EnemyState.Active;
                }
                else
                {
                    StartCoroutine(MoveOnScreen(gameObject, new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - 2, 0)));
                }
                break;
            case EnemyState.Active:
                // Add active behavior here
                break;
            case EnemyState.Dead:
                // Add dead behavior here
                break;
        }
    }

    public void SetActive()
    {
        m_EnemyState = EnemyState.Active;
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

    private bool EnemyIsOnScreen(GameObject enemy)
    {
        // Check if the full enemy sprite is on screen
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            StartCoroutine(ResetRotation());
        }
    }
    protected void OnDestroy()
    {
        AudioManager.Instance.PlaySFX(deathClip);
        EnemyGroup enemyGroup = FindObjectOfType<EnemyGroup>();
        if (enemyGroup != null)
        {
            enemyGroup.RemoveEnemy(gameObject);
        }
        AddScore();
        PowerUp();
    }
}