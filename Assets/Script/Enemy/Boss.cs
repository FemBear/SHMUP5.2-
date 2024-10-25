using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : BaseEnemy
{
    [SerializeField]
    int m_Spread = 5;
    [SerializeField]
    int m_BulletCount = 5;
    public override void Start()
    {
        base.Start();
        m_Health = 100;
        m_Speed = 1.5f;
        m_Damage = 10;
        m_BulletSpeed = 5;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Fire()
    {
        int RandomNumber = Random.Range(0, 3);
        switch (RandomNumber)
        {
            case 0:
                Shoot();
                break;
            case 1:
                ShootatPlayer();
                break;
            case 2:
                ShootSpread();
                break;
        }
    }
    private void Shoot()
    {
        if (m_Target != null)
        {
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * m_BulletSpeed;
        }
    }

    private void ShootatPlayer()
    {
        if (m_Target != null)
        {
            Vector3 direction = m_Target.transform.position - transform.position;
            direction.Normalize();
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * m_BulletSpeed;
        }
    }

    private void ShootSpread()
    {
        if (m_Target != null)
        {
            Vector3 direction = m_Target.transform.position - transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            for (int i = 0; i < m_BulletCount; i++)
            {
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.transform.position, Quaternion.identity);
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle + (i - m_BulletCount / 2) * m_Spread);
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * m_BulletSpeed;
            }
        }
    }
}
