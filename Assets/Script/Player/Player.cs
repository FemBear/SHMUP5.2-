using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Plane
{
    [Header("MegaBuster Settings")]
    [SerializeField]
    private bool m_CanMegaBuster = false;
    [SerializeField]
    private float m_MegaBusterCooldown = 5;
    PlayerControls m_InputControls;
    void Awake()
    {
        SetUpControlls();
    }

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_BulletSpawn = transform.GetChild(0);
        transform.position = new Vector3(8.8886f, -4, 0);
        StartCoroutine(MegaBusterCooldown());
    }

    private void Movement(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        m_Rb.velocity = new Vector2(direction.x * m_Speed, direction.y * m_Speed);
    }

    private void StopMovemnt(InputAction.CallbackContext context)
    {
        m_Rb.velocity = Vector2.zero;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (m_CanFire <= 0)
            {
                Fire();
            }
        }
    }

    private void MegaBuster(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (m_CanMegaBuster)
            {
                Debug.Log("MegaBuster");
                m_CanMegaBuster = false;
                StartCoroutine(MegaBusterCooldown());
            }
        }
    }

    IEnumerator MegaBusterCooldown()
    {
        yield return new WaitForSeconds(m_MegaBusterCooldown);
        m_CanMegaBuster = true;
    }

    private void SetUpControlls()
    {
        m_InputControls = new PlayerControls();
        m_InputControls.Player.Move.Enable();
        m_InputControls.Player.WASD.Enable();
        m_InputControls.Player.Shoot.Enable();
        m_InputControls.Player.MegaBuster.Enable();
        m_InputControls.Player.Shoot.performed += Shoot;
        m_InputControls.Player.MegaBuster.performed += MegaBuster;
        m_InputControls.Player.WASD.performed += Movement;
        m_InputControls.Player.Move.performed += Movement;
        m_InputControls.Player.WASD.canceled += StopMovemnt;
        m_InputControls.Player.Move.canceled += StopMovemnt;
    }
}