using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : BaseEnemy
{
    [SerializeField]
    int m_Spread = 5;
    [SerializeField]
    int m_BulletCount = 5;
    [SerializeField]
    float m_MoveSpeed = 2.0f;
    [SerializeField]
    GameObject m_Ships;
    [SerializeField]
    int m_SmallEnemyCount = 3;

    private enum MovementPattern { Static, LeftRight, Circular }
    private MovementPattern m_MovementPattern;

    private Vector3 m_StartPosition;
    private AudioClip SpawnClip;

    public override void Start()
    {
        base.Start();
        SpawnClip = Resources.Load<AudioClip>("Sound/Effects/Deploy");
        m_Health = (4 * GameManager.Instance.m_Wave) + 12;
        m_Speed = 1.5f;
        m_Damage = 10;
        m_BulletSpeed = 5;
        m_BulletCount = GameManager.Instance.m_Wave + 2;
        m_StartPosition = transform.position;
        m_MovementPattern = (MovementPattern)Random.Range(0, 2);
    }

    public override void Update()
    {
        base.Update();
        if (m_EnemyState == EnemyState.Active)
        {
            Move();
            Fire();
        }
    }

    private void Move()
    {
        switch (m_MovementPattern)
        {
            case MovementPattern.Static:
                break;
            case MovementPattern.LeftRight:
                float screenspacex = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
                transform.position = m_StartPosition + new Vector3(Mathf.PingPong(Time.time * m_MoveSpeed, screenspacex), 0, 0);
                break;
        }
    }
    

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
        if (m_Target != null)
        {
            m_Rb.freezeRotation = true;
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * m_BulletSpeed;
            AudioManager.Instance.PlaySFX(attackClip);
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
            AudioManager.Instance.PlaySFX(attackClip);
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
            AudioManager.Instance.PlaySFX(attackClip);
        }
    }

    private void Deploy()
    {
        for (int i = 0; i < m_SmallEnemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + i, transform.position.y, transform.position.z);
            Instantiate(m_Ships, spawnPosition, Quaternion.identity);
        }
        AudioManager.Instance.PlaySFX(SpawnClip);
    }
}