using System.Collections;
using UnityEngine;

public class RangedEnemy1 : BaseEnemy
{
    private Vector2 m_screenSpace;
    private Vector2 m_enemyPosition;
    private float m_timer = 1.0f;
    private EnemyState m_enemyState;

    override public void Start()
    {
        base.Start();
        m_enemyPosition = transform.position;
        m_screenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        m_enemyState = EnemyState.Spawning;
        Debug.Log(m_screenSpace.x);
        //make it shoot down instead of up
        m_BulletSpawn = transform.GetChild(0);

        // Ensure m_Target is assigned
        if (m_Target == null)
        {
            m_Target = GameObject.Find("Player");
            if (m_Target == null)
            {
                Debug.LogError("Target not assigned and Player object not found for " + gameObject.name);
            }
        }
        m_enemyState = EnemyState.Active;
    }

    new void Update()
    {
        base.Update();
        m_timer += Time.deltaTime;
        m_enemyPosition = transform.position;

        switch (m_enemyState)
        {
            case EnemyState.Spawning:
                {
                    m_enemyState = EnemyState.Active;
                }
                break;
            case EnemyState.Active:
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
                break;
            case EnemyState.Dead:
                break;
        }
    }

    private void Shoot()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            //make it shoot down instead of up
            m_CanFire = m_FireRate;
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, Quaternion.Euler(0, 0, 180));
            bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
        }
    }

    private void ShootAndAim()
    {
        if (m_CanFire <= 0 && m_BulletSpawn != null)
        {
            Quaternion _lookAt = Quaternion.LookRotation(Vector3.forward, m_Target.transform.position - transform.position);
            transform.rotation = _lookAt;
            m_CanFire = m_FireRate;
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, _lookAt);
            bullet.GetComponent<Bullet>().b_owner = gameObject;
            bullet.GetComponent<Bullet>().SetBullet(this.gameObject,m_Damage, m_BulletSpeed, m_LifeTime);
        }
    }

    private IEnumerator MovementSideToSide()
    {
        while (m_Health > 0)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            Debug.Log(pingPongX);
            Vector2 newPosition = new Vector2(pingPongX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime);

            yield return null;
        }
    }

}
