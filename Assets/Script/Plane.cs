using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Plane : MonoBehaviour
{
    [Header("Plane Settings")]
    [SerializeField]
    protected int m_Health = 3;
    [SerializeField]
    protected int m_Speed = 5;
    [SerializeField]
    protected float m_FireRate = 0.15f;
    [SerializeField]
    protected float m_CanFire = 0.0f;
    [SerializeField]
    protected int padding = 1;
    protected Rigidbody2D m_Rb;
    [SerializeField]
    [Header("Bullet Settings")]
    protected GameObject m_Bullet;
    [SerializeField]
    internal int m_Damage = 1;
    [SerializeField]
    internal int m_BulletSpeed = 10;
    [SerializeField]
    internal float m_LifeTime = 5;


     public virtual void Update()
    {
        FireRate();
    }

    internal void TakeDamage(int damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    virtual protected void Fire()
    {
        if (m_CanFire <= 0)
        {
            m_CanFire = m_FireRate;
            GameObject bullet = Instantiate(m_Bullet, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().b_Damage = m_Damage;
            bullet.GetComponent<Bullet>().b_Speed = m_BulletSpeed;
            bullet.GetComponent<Bullet>().b_LifeTime = m_LifeTime;
        }
    }

    protected void FireRate()
    {
        if (m_CanFire > 0)
        {
            m_CanFire -= Time.deltaTime;
        }
        if (m_CanFire < 0)
        {
            m_CanFire = 0;
        }
    }
}
