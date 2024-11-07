using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : BaseEnemy
{
    #region Variables
    [SerializeField]
    private int m_Spread = 5;
    [SerializeField]
    private int m_BulletCount = 5;
    [SerializeField]
    private float m_MoveSpeed = 2.0f;
    [SerializeField]
    private GameObject m_Ships;
    [SerializeField]
    private int m_SmallEnemyCount = 3;
    private int m_RandomMove;
    private Vector3 m_StartPosition;
    private AudioClip m_SpawnClip;
    #endregion
    public override void Start()
    {
        base.Start();
        m_CanFire = m_FireRate;
    }

    #region Basics
    public override void Update()
    {
        base.Update();
        if (m_EnemyState == EnemyState.Active)
        {
            Move();
            if (m_CanFire <= 0)
            {
                Fire();
            }
        }
    }
    #endregion

    #region Movement
    private void Move()
    {
        switch (m_RandomMove)
        {
            case 0:
                break;
            case 1:
                float screenspacex = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
                transform.position = m_StartPosition + new Vector3(Mathf.PingPong(Time.time * m_MoveSpeed, screenspacex), 0, 0);
                break;
        }
    }
    #endregion

    #region Attack
    public override void Fire()
    {
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                Shoot();
                break;
            case 1:
                ShootAtPlayer();
                break;
            case 2:
                ShootSpread();
                break;
            case 3:
                Deploy();
                break;
        }
    }

    private void Shoot()
    {
        if (m_BulletSpawn != null)
        {
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetBullet(gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            AudioManager.Instance.PlaySFX(m_ShootClip);
        }
    }


    private void ShootAtPlayer()
    {
        if (m_Target != null)
        {
            m_Rb.freezeRotation = false;
            Vector3 direction = m_Target.transform.position - transform.position;
            direction.Normalize();
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * m_BulletSpeed;
            StartCoroutine(ResetRotation());
            m_Rb.freezeRotation = true;
            AudioManager.Instance.PlaySFX(m_ShootClip);
        }
    }

    private void ShootSpread()
    {
        if (m_Target != null)
        {
            m_Rb.freezeRotation = true;
            Vector3 direction = Vector2.down;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            for (int i = 0; i < m_BulletCount; i++)
            {
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle + (i - m_BulletCount / 2) * m_Spread);
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * m_BulletSpeed;
            }
            StartCoroutine(ResetRotation());
            m_Rb.freezeRotation = true;
            AudioManager.Instance.PlaySFX(m_ShootClip);
        }
    }

    private void Deploy()
    {
        for (int i = 0; i < m_SmallEnemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + i, transform.position.y, transform.position.z);
            Instantiate(m_Ships, spawnPosition, Quaternion.identity);
        }
        AudioManager.Instance.PlaySFX(m_SpawnClip);
    }
    #endregion

    #region Utility/Setup
    protected override void Initialize()
    {
        base.Initialize();
        m_SpawnClip = Resources.Load<AudioClip>("Sound/Effects/Deploy");
        m_Health = (4 * GameManager.Instance.m_Wave) + 12;
        m_Speed = 1.5f;
        m_Damage = 10;
        m_BulletSpeed = 5;
        m_BulletCount = GameManager.Instance.m_Wave + 2;
        m_RandomMove = Random.Range(0, 2);
        m_StartPosition = transform.position;
    }
    #endregion
}