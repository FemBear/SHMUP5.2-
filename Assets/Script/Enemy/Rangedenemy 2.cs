using System.Collections;
using UnityEngine;

public class RangedMovingEnemy : BaseEnemy
{
    #region Variables
    private int m_ProjectileCount = 1;
    private int m_Spread = 5;
    [SerializeField]
    private float m_MinDistanceToPlayer = 1.0f;
    private bool m_IsMoving = false;
    #endregion

    #region Basics
    public override void Start()
    {
        base.Start();
        m_CanFire = m_FireRate;
    }
    public override void Update()
    {
        base.Update();
        FireRate();
        if (m_EnemyState == EnemyState.Active && !m_IsMoving)
        {
            m_IsMoving = true;
            StartCoroutine(MoveToTarget());
        }
        if (m_EnemyState == EnemyState.Active)
        {
            Shoot();
        }
    }
    #endregion

    #region Attack
    private void Shoot()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            m_Rb.freezeRotation = true;
            m_CanFire = m_FireRate;
            Fire(180);
            AudioManager.Instance.PlaySFX(m_ShootClip);
        }
    }

    private void Fire(float baseAngle)
    {
        if (m_ProjectileCount > 1)
        {
            float spreadAngle = m_Spread / (m_ProjectileCount - 1);
            for (int i = 0; i < m_ProjectileCount; i++)
            {
                float angle = -m_Spread / 2 + spreadAngle * i;
                Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + angle);
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, rotation);
                bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            }
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle);
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, rotation);
            bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
        }
    }
    #endregion

    #region Movement
    private IEnumerator MoveToTarget()
    {
        m_Rb.freezeRotation = true;
        while (m_Health > 0)
        {
            if (m_Target != null)
            {
                Transform targetTransform = m_Target.transform;
                float distanceToPlayer = Vector2.Distance(transform.position, targetTransform.position);
                if (distanceToPlayer > m_MinDistanceToPlayer)
                {
                    Vector2 newTargetPosition = targetTransform.position;
                    float step = m_Speed * Time.deltaTime;
                    Vector2 newPosition = Vector2.MoveTowards(transform.position, newTargetPosition, step);
                    if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y))
                    {
                        transform.position = newPosition;
                    }
                }
            }
            yield return null;
        }
        m_IsMoving = false;
    }
    #endregion

    #region Utility/Setup
    protected override void Initialize()
    {
        base.Initialize();
        m_ProjectileCount = 1 + Mathf.RoundToInt(GameManager.Instance.m_Wave / 2);
        if (m_ProjectileCount < 1)
        {
            m_ProjectileCount = 1;
        }
        m_Speed = Mathf.Min(1 + (GameManager.Instance.m_Wave / 4), 10 / 2);
        if (m_Speed < 1)
        {
            m_Speed = 1;
        }
        m_FireRate = Mathf.Max(0.1f, m_FireRate - (GameManager.Instance.m_Wave * 0.1f));
        if (m_FireRate < 0.25f)
        {
            m_FireRate = 0.25f;
        }
        #endregion
    }
}