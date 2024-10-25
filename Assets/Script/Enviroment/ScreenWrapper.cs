using JetBrains.Annotations;
using UnityEngine;
public class ScreenWrapper : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private bool clamped = false;
    [SerializeField]
    private bool wrapX = true;
    [SerializeField]
    private bool wrapY = false;
    internal Vector3 m_ScreenPosition
    {
        get
        {
            return mainCamera.WorldToScreenPoint(transform.position);
        }
    }

    internal Vector3 m_ScreenBounds
    {
        get
        {
            return new Vector3(Screen.width, Screen.height, 0);
        }
    }
    private SpriteRenderer m_SpriteRenderer;
    internal int m_SpriteWidth;
    internal int m_SpriteHeight;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteWidth = m_SpriteRenderer.sprite.texture.width;
        m_SpriteHeight = m_SpriteRenderer.sprite.texture.height; 
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        if (wrapX)
        {
            if (screenPosition.x > Screen.width)
            {
                screenPosition.x = 0;
            }
            if (screenPosition.x < 0)
            {
                screenPosition.x = Screen.width;
            }
        }
        if (wrapY)
        {
            if (screenPosition.y > Screen.height)
            {
                screenPosition.y = 0;
            }
            if (screenPosition.y < 0)
            {
                screenPosition.y = Screen.height;
            }
        }
        transform.position = mainCamera.ScreenToWorldPoint(screenPosition);
        if (clamped)
        {
            Clamp();
        }
    }
    internal void Clamp()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        if (wrapX)
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, m_SpriteWidth, Screen.width - m_SpriteWidth + 2);
        }
        if (wrapY)
        {
            screenPosition.y = Mathf.Clamp(screenPosition.y, m_SpriteHeight, Screen.height - m_SpriteHeight + 2);
        }
        transform.position = mainCamera.ScreenToWorldPoint(screenPosition);
    }
}
