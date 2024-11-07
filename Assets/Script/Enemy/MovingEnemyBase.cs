using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MovingEnemyBase : BaseEnemy
{
    #region Variables
    private Vector2 m_ScreenSpace;
    private float m_Timer = 1.0f;
    private bool m_IsMoving = false;
    private int m_RandomMove;
    #endregion

    #region Basics
    public override void Start()
    {
        base.Start();
        m_CanFire = m_FireRate; // Initialize m_CanFire
    }
    public override void Update()
    {
        base.Update();
        FireRate();
        m_Timer += Time.deltaTime;
        if (m_EnemyState == EnemyState.Active && !m_IsMoving)
        {
            m_IsMoving = true;
            RandomMovement();
        }
    }
    #endregion

    #region Movement
    private void RandomMovement()
    {
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
        float randomY = Random.Range(-m_ScreenSpace.y, m_ScreenSpace.y - 2);
        Vector2 targetPosition = new Vector2(transform.position.x, randomY);

        // Move to the random Y position
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_Speed * Time.deltaTime);
            yield return null;
        }

        // Start moving side to side at the chosen Y position
        while (m_Health > 0)
        {
            float targetX = m_ScreenSpace.x;
            while (transform.position.x < targetX)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }

            targetX = -m_ScreenSpace.x;
            while (transform.position.x > targetX)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, randomY), m_Speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        // Use m_Target as the target position
        Vector2 targetPosition = m_Target.transform.position;

        // Move towards the target position
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_Speed * Time.deltaTime);
            yield return null;
        }

        m_IsMoving = false;
    }

    #endregion

    #region Utility/Setup
    protected override void Initialize()
    {
        base.Initialize();
        m_ScreenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        m_Speed = Mathf.Min(1 + (GameManager.Instance.m_Wave / 4), 10) / 3;
        m_RandomMove = Random.Range(0, 2);
    }
    #endregion
}