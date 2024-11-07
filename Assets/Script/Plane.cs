using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Plane : MonoBehaviour
{
    #region Plane Variables
    [Header("Plane Settings")]
    [SerializeField]
    public int m_Health = 3;
    [SerializeField]
    protected float m_Speed = 5;
    [SerializeField]
    protected float m_FireRate = 0.15f;
    [SerializeField]
    public float m_CanFire = 0.0f;
    protected Rigidbody2D m_Rb;
    protected SpriteRenderer m_SpriteRenderer;
    [SerializeField]
    [CanBeNull]
    protected GameObject m_ExplosionFX;
    protected BoxCollider2D m_Collider;
    [SerializeField]
    protected int m_padding = 1;
    [SerializeField]
    public bool m_isInvulnerable = false;
    [SerializeField]
    protected int m_iFrames = 1;
    [SerializeField]
    protected float m_iFrameDuration = 0.1f;
    #endregion

    #region Bullet Variables
    [SerializeField]
    [Header("Bullet Settings")]
    protected GameObject m_Bullet;
    [SerializeField]
    internal int m_Damage = 1;
    [SerializeField]
    internal int m_BulletSpeed = 10;
    [SerializeField]
    internal float m_LifeTime = 5;
    [CanBeNull]
    protected Transform m_BulletSpawn;
    #endregion

    [Header("Audio")]
    [SerializeField]
    protected AudioClip m_ShootClip;
    [SerializeField]
    protected AudioClip m_DeathClip;

    #region Health/Damage
    internal virtual void TakeDamage(int damage)
    {
        if (!m_isInvulnerable)
        {
            m_Health -= damage;
            if (m_Health <= 0 && gameObject.tag != "Player")
            {
                if (m_ExplosionFX != null)
                {
                    Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
            if (gameObject.tag == "Player")
            {
                UIManager.Instance.UpdateHealthUI(m_Health);
            }
            StartCoroutine(Invulnerability(m_iFrames, m_iFrameDuration));
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
    #endregion

    #region Shooting
    public virtual void Fire()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            m_CanFire = m_FireRate; // Set m_CanFire to m_FireRate when firing
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
    }
    #endregion
}

