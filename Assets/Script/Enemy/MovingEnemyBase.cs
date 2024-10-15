using UnityEngine;
using System.Collections;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;


public class MovingEnemyBase : BaseEnemy
{
    public Vector2 m_screenSpace;
    public Vector2 m_enemyPosition;
    public float m_timer = 1.0f;

    private EnemyState m_enemyState;

   override public void Start()
    {
        base.Start();
        m_enemyPosition = transform.position;
        m_screenSpace = new Vector2(Screen.width, Screen.height);
        m_enemyState = EnemyState.Spawning;


        // Ensure m_Target is assigned
        if (m_Target == null)
        {
            m_Target = GameObject.Find("Player");
            if (m_Target == null)
            {
                Debug.LogError("Target not assigned and Player object not found for " + gameObject.name);
            }
        }
    }

   new void Update()
    {
        m_timer += Time.deltaTime;
        m_enemyPosition = transform.position;

        switch (m_enemyState)
        {
            case EnemyState.Spawning:
                if (!m_ScreenWrapper.OnScreen())
                {
                    StartCoroutine(MoveOnScreen());
                }
                else
                {
                    m_enemyState = EnemyState.Active;
                }
                break;
            case EnemyState.Active:
                int RandomNumber = Random.Range(0, 2);
                if (RandomNumber == 0)
                {
                    StartCoroutine(MovementSideToSide());
                }
                else
                {
                    StartCoroutine(MovementSideToTarget());
                }
                m_enemyState = EnemyState.Spawning;
                break;
            case EnemyState.Dead:
                break;
        }
    }

    private IEnumerator MovementSideToSide()
    {
        yield return new WaitForSeconds(0.5f);

        while (m_Health > 0)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            float targetY = m_Target.transform.position.y;
            Vector2 newPosition = new Vector2(pingPongX, targetY);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime);

            yield return null;
        }
    }


    private IEnumerator MovementSideToTarget()
    {
        yield return new WaitForSeconds(0.5f);

        while (m_Health > 0)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            Vector2 newPosition = new Vector2(pingPongX, m_enemyPosition.y);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime);
            yield return null;
        }
    }
}