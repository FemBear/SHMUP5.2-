using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MovingEnemyBase : BaseEnemy
{
    #region Variables
    private Vector2 m_ScreenSpace;
    private float m_Timer = 1.0f;
    private int m_RandomMove;
    private bool m_IsMoving;
    private float m_MinDistanceToPlayer = 0f;
    #endregion

    #region Basics
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        FireRate();
        m_Timer += Time.deltaTime;
        if (m_EnemyState == EnemyState.Active)
        {
            if (!m_IsMoving)
            {
                RandomMovement();
            }
        }
    }
    #endregion

    #region Movement
    private void RandomMovement()
    {
        m_IsMoving = true;
        switch (m_RandomMove)
        {
            case 0:
                StartCoroutine(MoveSideToSide());
                break;
            case 1:
                StartCoroutine(MoveToTarget());
                break;
        }
    }

    private IEnumerator MoveSideToSide()
    {
        // Choose a random Y position once
        float randomY = Random.Range(-m_ScreenSpace.y, m_ScreenSpace.y);
        Vector2 targetPosition = new Vector2(transform.position.x, randomY);

        // Move to the random Y position
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, m_Speed * Time.deltaTime);
            yield return null;
        }

        // Start moving side to side at the chosen Y position
        while (m_Health > 0)
        {
            float targetX = m_ScreenSpace.x;
            while (transform.position.x < targetX)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }

            targetX = -m_ScreenSpace.x;
            while (transform.position.x > targetX)
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        m_Rb.freezeRotation = true;
        while (m_Health > 0)
        {
            if (m_Target != null)
            {
                Transform targetTransform = m_Target.transform;
                float distanceToPlayer = Vector2.Distance(transform.position, targetTransform.position);
                if (distanceToPlayer > m_MinDistanceToPlayer)
                {
                    Vector2 newTargetPosition = targetTransform.position;
                    float step = m_Speed * Time.deltaTime;
                    Vector2 newPosition = Vector2.MoveTowards(transform.position, newTargetPosition, step);
                    if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y))
                    {
                        transform.position = newPosition;
                    }
                }
            }
            yield return null;
        }
    }


    #endregion

    #region Utility/Setup
    protected override void Initialize()
    {
        base.Initialize();
        m_ScreenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        m_Speed = Mathf.Min(1 + (GameManager.Instance.m_Wave / 4), 10);
        m_RandomMove = Random.Range(0, 2);
        m_Target = GameObject.FindGameObjectWithTag("Player");
        m_IsMoving = false; 
    }
    #endregion
}