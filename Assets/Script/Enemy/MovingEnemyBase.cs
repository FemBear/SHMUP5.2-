using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Callbacks;

public class MovingEnemyBase : BaseEnemy
{
    private Vector2 m_screenSpace;
    private Vector2 m_enemyPosition;
    private float m_timer = 1.0f;
    [SerializeField]
    float m_SpeedY = 1.0f;
    private bool m_IsMoving = false;

    public override void Start()
    {
        base.Start();
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
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                StartCoroutine(MovementSideToSide());
            }
            else
            {
                StartCoroutine(MovementSideToTarget());
            }
        }
    }

    private IEnumerator MovementSideToSide()
    {
        m_Rb.freezeRotation = true;
        bool isAtHeight = false;
        // Choose a random position on the Y-axis on screen
        float randomY = Random.Range(0, m_screenSpace.y - 3);
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

    private IEnumerator MovementSideToTarget()
    {
        m_Rb.freezeRotation = true;
        while (m_Health > 0)
        {
            if (m_Target != null)
            {
                if (Vector2.Distance(transform.position, m_Target.transform.position) > 0.1f)
                {
                    Vector2 newTargetPosition = m_Target.transform.position;
                    transform.position = Vector2.Lerp(transform.position, newTargetPosition, Time.deltaTime * m_Speed / 3);
                }
            }
            yield return null;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>().m_isInvulnerable == false)
        {
            other.gameObject.GetComponent<Player>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}