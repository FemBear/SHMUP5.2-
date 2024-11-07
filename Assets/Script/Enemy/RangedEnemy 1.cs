using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangedEnemy1 : BaseEnemy
{
    #region Variables
    private Vector2 m_ScreenSpace;
    private int m_RandomShot;
    private int m_RandomMove;
    private float m_MinDistanceToPlayer = 5.0f;
    private int m_BulletCount = 1;
    private int m_Spread = 5;
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
        if (m_EnemyState == EnemyState.Active)
        {
            RandomAttack();
        }
        if (m_EnemyState == EnemyState.Active && !m_IsMoving)
        {
            RandomMove();
        }
    }
    #endregion

    #region Attack
    private void Shoot()
    {
        m_Rb.freezeRotation = true;
        if (m_CanFire == 0)
        {
            m_CanFire = m_FireRate;
            Fire(180);
            AudioManager.Instance.PlaySFX(m_ShootClip);
        }
    }

    private void ShootAndAim()
    {
        m_Rb.freezeRotation = false;
        if (m_CanFire == 0)
        {
            Quaternion lookAt = Quaternion.LookRotation(Vector3.forward, m_Target.transform.position - transform.position);
            transform.rotation = lookAt;
            m_CanFire = m_FireRate;
            Fire(lookAt.eulerAngles.z);
            AudioManager.Instance.PlaySFX(m_ShootClip);
            ResetRotation();
        }
    }

    private void Fire(float baseAngle)
    {
        if (m_Bullet == null || m_BulletSpawn == null)
        {
            Debug.LogError("Bullet or BulletSpawn is not assigned.");
            return;
        }

        if (m_BulletCount > 1)
        {
            float spreadAngle = m_Spread / (m_BulletCount - 1);
            for (int i = 0; i < m_BulletCount; i++)
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
    private IEnumerator MoveSideToSide()
    {
        // Choose a random Y position once
        float randomY = Random.Range(-m_ScreenSpace.y, m_ScreenSpace.y - 2);
        Vector2 targetPosition = new Vector2(transform.position.x, randomY);
        
        // Move to the random Y position
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_Speed * Time.deltaTime);
            yield return null;
        }

        // Start moving side to side at the chosen Y position
        while (m_Health > 0)
        {
            float targetX = m_ScreenSpace.x;
            while (transform.position.x < targetX)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }

            targetX = -m_ScreenSpace.x;
            while (transform.position.x > targetX)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }
        }
    }

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
    }
    #endregion

    #region Utility/Setup
    private void RandomAttack()
    {
        m_RandomShot = Random.Range(0, 2);
        switch (m_RandomShot)
        {
            case 0:
                Shoot();
                break;
            case 1:
                ShootAndAim();
                break;
        }
    }

    private void RandomMove()
    {
        m_RandomMove = Random.Range(0, 2);
        switch (m_RandomMove)
        {
            case 0:
                StartCoroutine(MoveSideToSide());
                break;
            case 1:
                StartCoroutine(MoveToTarget());
                m_IsMoving = true;
                break;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        m_BulletCount = 1 + Mathf.RoundToInt(GameManager.Instance.m_Wave / 2);
        m_Speed = Mathf.Min(1 + (GameManager.Instance.m_Wave / 4), 10);
        m_FireRate = Mathf.Max(0.1f, m_FireRate - (GameManager.Instance.m_Wave * 0.1f));
        m_ScreenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        if (m_FireRate < 0.25f)
        {
            m_FireRate = 0.25f;
        }
    }
    #endregion
}