using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangedEnemy1 : BaseEnemy
{
    private Vector2 m_screenSpace;
    private Vector2 m_enemyPosition;
    private float m_timer = 1.0f;
    private int m_ProjectileCount = 1;
    private int m_Spread = 5;
    private bool m_IsMoving = false;

    public override void Start()
    {
        base.Start();
        m_Speed = 1 + (GameManager.Instance.m_Wave / 4);
        m_ProjectileCount = 1 + Mathf.RoundToInt(GameManager.Instance.m_Wave / 2);
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

        if (m_EnemyState == EnemyState.Active && !m_IsMoving)
        {
            m_IsMoving = true;
            StartCoroutine(MovementSideToSide());
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    Shoot();
                    break;
                case 1:
                    ShootAndAim();
                    break;
            }
        }
    }


    private void Shoot()
    {
        m_Rb.freezeRotation = true;
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
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

    private void ShootAndAim()
    {
        m_Rb.freezeRotation = false;
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            Quaternion _lookAt = Quaternion.LookRotation(Vector3.forward, m_Target.transform.position - transform.position);
            transform.rotation = _lookAt;
            m_CanFire = m_FireRate;
            if (m_ProjectileCount > 1)
            {
                float spreadAngle = m_Spread / (m_ProjectileCount - 1);
                for (int i = 0; i < m_ProjectileCount; i++)
                {
                    float angle = -m_Spread / 2 + spreadAngle * i;
                    Quaternion bulletRotation = Quaternion.Euler(_lookAt.eulerAngles.x, _lookAt.eulerAngles.y, _lookAt.eulerAngles.z + angle);
                    GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, bulletRotation);
                    bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
                }
            }
            else
            {
                Quaternion bulletRotation = Quaternion.Euler(_lookAt.eulerAngles.x, _lookAt.eulerAngles.y, _lookAt.eulerAngles.z);
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, bulletRotation);
                bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            }
            AudioManager.Instance.PlaySFX(attackClip);
            ResetRotation();
        }
    }


    private IEnumerator MovementSideToSide()
    {
        m_Rb.freezeRotation = true;
        bool isAtHeight = false;
        // Choose a random position on the Y-axis on screen
        float randomY = Random.Range(0, m_screenSpace.y);
        Vector2 newTargetPosition = new Vector2(transform.position.x, randomY);

        while (Vector2.Distance(transform.position, newTargetPosition) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, newTargetPosition, Time.deltaTime * m_Speed);
            //make it wait until it reaches the height
            if (Mathf.Abs(transform.position.y - randomY) < 0.1f)
            {
                isAtHeight = true;
            }
            yield return null;
        }
        while (m_Health > 0 && isAtHeight == true)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            Vector2 newPosition = new Vector2(pingPongX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime * m_Speed);
            yield return null;
        }
    }

}