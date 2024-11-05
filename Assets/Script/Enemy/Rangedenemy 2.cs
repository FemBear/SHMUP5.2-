using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Callbacks;
using UnityEngine;

public class RangedMovingEnemy : BaseEnemy
{
    private Vector2 m_screenSpace;
    private Vector2 m_enemyPosition;
    private int m_ProjectileCount = 1;
    private int m_Spread = 5;
    private float m_timer = 1.0f;
    [SerializeField]
    float m_SpeedY = 1.0f;
    private float m_MinDistanceToPlayer = 1.0f; // Minimum distance to maintain from the player
    private bool m_IsMovingToTarget = false; // Flag to check if coroutine is running

    public override void Start()
    {
        base.Start();
        m_ProjectileCount = 1 + Mathf.RoundToInt(GameManager.Instance.m_Wave / 2);
        m_Speed = 1 + (GameManager.Instance.m_Wave / 4);
        m_SpeedY = 1 + (GameManager.Instance.m_Wave / 4);
        if (m_Speed > 10)
        {
            m_Speed = 10;
        }
        if (m_SpeedY > 10)
        {
            m_SpeedY = 10;
        }
        m_FireRate = Mathf.Max(0.1f, m_FireRate - (GameManager.Instance.m_Wave * 0.1f));
        if (m_FireRate < 0.25f)
        {
            m_FireRate = 0.25f;
        }
        m_enemyPosition = transform.position;
        m_screenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        m_BulletSpawn = transform.GetChild(0);

        if (m_Target == null)
        {
            m_Target = GameObject.Find("Player");
            if (m_Target == null)
            {
                Debug.LogError("Target not assigned and Player object not found for " + gameObject.name);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        m_timer += Time.deltaTime;
        m_enemyPosition = transform.position;

        if (m_EnemyState == EnemyState.Active && !m_IsMovingToTarget)
        {
            m_IsMovingToTarget = true;
            StartCoroutine(MovementSideToTarget());
            Shoot();
        }
    }

    private void Shoot()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            m_Rb.freezeRotation = true;
            m_CanFire = m_FireRate;
            if (m_ProjectileCount > 1)
            {
                float spreadAngle = m_Spread / (m_ProjectileCount - 1);
                for (int i = 0; i < m_ProjectileCount; i++)
                {
                    float angle = -m_Spread / 2 + spreadAngle * i;
                    Quaternion rotation = Quaternion.Euler(0, 0, 180 + angle);
                    GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, rotation);
                    bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
                }
            }
            else
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 180);
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, rotation);
                bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            }
            AudioManager.Instance.PlaySFX(attackClip);
        }
    }


    private IEnumerator MovementSideToTarget()
    {
        m_Rb.freezeRotation = true;
        Transform targetTransform = m_Target.transform;
        while (m_Health > 0)
        {
            if (m_Target != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, targetTransform.position);
                if (distanceToPlayer > m_MinDistanceToPlayer)
                {
                    Vector2 newTargetPosition = targetTransform.position;
                    float step = m_Speed * Time.deltaTime;
                    transform.position = Vector2.MoveTowards(transform.position, newTargetPosition, step);
                }
            }
            yield return null;
        }
        m_IsMovingToTarget = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>().m_isInvulnerable == false)
        {
            other.gameObject.GetComponent<Player>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}

