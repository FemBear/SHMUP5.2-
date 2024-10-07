using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Plane
{

    PlayerControls m_InputControls;
    void Awake()
    {
        SetUpControlls();
    }

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(0, -4, 0);
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


    private void SetUpControlls()
    {
        m_InputControls = new PlayerControls();
        m_InputControls.Player.Move.Enable();
        m_InputControls.Player.WASD.Enable();
        m_InputControls.Player.Shoot.Enable();
        m_InputControls.Player.Shoot.performed += Shoot;
        m_InputControls.Player.WASD.performed += Movement;
        m_InputControls.Player.Move.performed += Movement;
        m_InputControls.Player.WASD.canceled += StopMovemnt;
        m_InputControls.Player.Move.canceled += StopMovemnt;
    }
}