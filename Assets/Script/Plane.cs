using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
public class Plane : MonoBehaviour
{
    [Header("Plane Settings")]
    [SerializeField]
    protected int m_Health = 3;
    [SerializeField]
    protected float m_Speed = 5;
    [SerializeField]
    protected float m_FireRate = 0.15f;
    [SerializeField]
    protected float m_CanFire = 0.0f;
    protected BoxCollider2D m_Collider;
    [SerializeField]
    protected int padding = 1;
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
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            m_CanFire = m_FireRate;
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().owner = gameObject;
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
    public IEnumerator Flash(SpriteRenderer sprite)
    {
        m_Collider.enabled = false;
        for (int i = 0; i < 3; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
        }
        m_Collider.enabled = true;
    }
}
