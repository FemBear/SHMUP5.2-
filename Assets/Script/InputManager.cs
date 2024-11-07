using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerControls m_InputControls;

    void OnEnable()
    {
        m_InputControls = new PlayerControls();
        m_InputControls.Enable();
    }

    void OnDisable()
    {
        m_InputControls.Disable();
    }

    public PlayerControls GetPlayerControls()
    {
        return m_InputControls;
    }

    public override void Awake()
    {
        base.Awake();
    }
}
