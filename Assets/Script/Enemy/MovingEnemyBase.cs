using UnityEngine;
using System.Collections;
using Vector2 = UnityEngine.Vector2;


public class MovingEnemyBase : BaseEnemy
{
    private Vector2 m_screenSpace;
    private Vector2 m_enemyPosition;
    private float m_timer = 1.0f;
    [SerializeField]
    float m_SpeedY = 1.0f;

    private EnemyState m_enemyState;

   override public void Start()
    {
        base.Start();
        m_enemyPosition = transform.position;
        m_screenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
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
                // if (!m_ScreenWrapper.OnScreen())
                // {
                //     StartCoroutine(MoveOnScreen());
                // }
                {
                    m_enemyState = EnemyState.Active;
                }
                break;
            case EnemyState.Active:
                int RandomNumber = Random.Range(0, 1);
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
        while (m_Health > 0)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            Vector2 newPosition = new Vector2(pingPongX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime);

            yield return null;
        }
    }


    private IEnumerator MovementSideToTarget()
    {
        while (m_Health > 0)
        {
            float pingPongX = Mathf.PingPong(m_timer * m_Speed, m_screenSpace.x);
            float newY = transform.position.y;
            if (newY >= m_Target.transform.position.y)
            {
                newY = transform.position.y - m_SpeedY * Time.deltaTime;
            } else {
                newY = m_Target.transform.position.y;
            }
            Vector2 newPosition = new Vector2(pingPongX, newY);
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime);
            yield return null;
        }
    }
}