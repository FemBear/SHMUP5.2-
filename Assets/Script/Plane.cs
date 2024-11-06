using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
public class Plane : MonoBehaviour
{
    [Header("Plane Settings")]
    [SerializeField]
    public int m_Health = 3;
    [SerializeField]
    protected float m_Speed = 5;
    [SerializeField]
    protected float m_FireRate = 0.15f;
    [SerializeField]
    protected float m_CanFire = 0.0f;
    protected BoxCollider2D m_Collider;
    [SerializeField]
    protected int m_padding = 1;
    [SerializeField]
    public bool m_isInvulnerable = false;
    [SerializeField]
    protected int m_iFrames = 1;
    [SerializeField]
    protected float m_iFrameDuration = 0.1f;

    [CanBeNull]
    protected Transform m_BulletSpawn;
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
    protected SpriteRenderer m_SpriteRenderer;
    [SerializeField]
    [CanBeNull]
    protected GameObject m_ExplosionFX;
    public virtual void Update()
    {
        FireRate();
    }
    internal void TakeDamage(int damage)
    {
        if (!m_isInvulnerable)
        {
            m_Health -= damage;
            if (m_Health <= 0)
            {
                if (m_ExplosionFX != null)
                {
                    Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
            StartCoroutine(Invulnerability(m_iFrames, m_iFrameDuration));
        }
    }

    public virtual void Fire()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            m_CanFire = m_FireRate;
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().b_owner = gameObject;
            bullet.GetComponent<Bullet>().b_Damage = m_Damage;
            bullet.GetComponent<Bullet>().b_Speed = m_BulletSpeed;
            bullet.GetComponent<Bullet>().b_LifeTime = m_LifeTime;
        }
    }

    protected virtual void FireRate()
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

    protected virtual IEnumerator Invulnerability(int iFrames, float iFrameDuration)
    {
        m_isInvulnerable = true;
        for (int i = 0; i < iFrames; i++)
        {
            m_SpriteRenderer.enabled = false;
            yield return new WaitForSeconds(iFrameDuration);
            m_SpriteRenderer.enabled = true;
            yield return new WaitForSeconds(iFrameDuration);
        }
        m_isInvulnerable = false;
    }
}

