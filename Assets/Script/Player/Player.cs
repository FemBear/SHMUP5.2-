using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Plane
{
    [Header("ShipUpgrades")]
    [SerializeField]
    private Sprite[] m_ShipForms;
    [SerializeField]
    private int m_ShipForm = 0;
    [SerializeField]
    private int m_BulletCount = 1;
    [SerializeField]
    private float m_SpreadAngle = 0;
    [SerializeField]
    private float m_RepeatRate = 0.5f;

    [Header("PowerUp Settings")]
    [SerializeField]
    private bool m_CanMegaBuster = false;
    [SerializeField]
    private float m_shieldTime = 3;
    private GameObject m_Shield;
    PlayerControls m_InputControls;
    private Coroutine shootingCoroutine;

    [Header("Audio")]
    [SerializeField]
    private AudioClip shootClip;
    [SerializeField]
    private AudioClip deathClip;

    void Start()
    {
        SetUpControlls();
        SetUp();
    }

    private void Movement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            if (m_Rb != null)
            {
                m_Rb.velocity = new Vector2(direction.x * m_Speed, direction.y * m_Speed);
            }
        }
        else if (context.canceled)
        {
            if (m_Rb != null)
            {
                m_Rb.velocity = Vector2.zero;
            }
        }
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shootingCoroutine ??= StartCoroutine(ShootingRoutine());
        }
        else if (context.canceled)
        {
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (true)
        {
            if (m_CanFire <= 0)
            {
                Fire();
                m_CanFire = m_FireRate + m_RepeatRate;
            }
            else
            {
                m_CanFire -= Time.deltaTime;
            }
            yield return null;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && !m_isInvulnerable)
        {
            TakeDamage(1);
        }
    }

    public override void Fire()
    {
        m_CanFire = m_FireRate * Time.deltaTime + m_RepeatRate;
        for (int i = 0; i < m_BulletCount; i++)
        {
            GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
            bullet.GetComponent<Bullet>().SetBullet(this.gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            if (m_BulletCount > 1)
            {
                bullet.transform.Rotate(0, 0, -m_SpreadAngle / 2 + m_SpreadAngle * i / (m_BulletCount - 1));
            }
        }
        AudioManager.Instance.PlaySFX(shootClip);
    }

    #region ShipUpgradesAndPowerUps
    [ContextMenu("ShipUpgrade")]
    public void ShipUpgrade()
    {
        if (m_ShipForm < m_ShipForms.Length - 1)
        {
            m_ShipForm++;
            m_SpriteRenderer.sprite = m_ShipForms[m_ShipForm];
        }
        switch (m_ShipForm)
        {
            case 0:
                m_BulletCount = 1;
                m_SpreadAngle = 0;
                m_BulletSpeed = 10;
                m_Damage = 1;
                break;
            case 1:
                m_BulletCount = 2;
                m_SpreadAngle = 15;
                m_BulletSpeed = 15;
                m_Damage = 2;
                break;
            case 2:
                m_BulletCount = 3;
                m_SpreadAngle = 30;
                m_BulletSpeed = 20;
                m_Damage = 3;
                break;
        }
    }

    public IEnumerator Shield()
    {
        m_Shield.SetActive(true);
        m_isInvulnerable = true;
        yield return new WaitForSeconds(m_shieldTime);
        m_Shield.SetActive(false);
        m_isInvulnerable = false;
    }

    #endregion

    #region MegaBuster
    private void MegaBuster(InputAction.CallbackContext context)
    {
        if (context.performed && m_CanMegaBuster)
        {
            Debug.Log("MegaBuster");
            m_CanMegaBuster = false;
        }
    }

    public void MegaBusterActive()
    {
        m_CanMegaBuster = true;
    }
    #endregion

    #region SetUp
    private void SetUpControlls()
    {
        m_InputControls = new PlayerControls();
        m_InputControls.Player.Move.Enable();
        m_InputControls.Player.WASD.Enable();
        m_InputControls.Player.Shoot.Enable();
        m_InputControls.Player.MegaBuster.Enable();
        m_InputControls.Player.Pauze.Enable();
        m_InputControls.Player.Shoot.performed += Shoot;
        m_InputControls.Player.Shoot.canceled += Shoot;
        m_InputControls.Player.MegaBuster.performed += MegaBuster;
        m_InputControls.Player.WASD.performed += Movement;
        m_InputControls.Player.Move.performed += Movement;
        m_InputControls.Player.WASD.canceled += Movement;
        m_InputControls.Player.Pauze.performed += PauzeGame;
    }

    private void SetUp()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        shootClip = Resources.Load<AudioClip>("$Sound/Effects/Shoot");
        deathClip = Resources.Load<AudioClip>("$Sound/Effects/Death");
        m_BulletSpawn = transform.GetChild(0);
        m_Shield = transform.GetChild(1).gameObject;
        m_Shield.SetActive(false);
        transform.position = new Vector3(8.8886f, -4, 0);
    }

    private void PauzeGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.PauzeGame();
        }
    }

    void OnDestroy()
    {
        if (m_ExplosionFX != null)
        {
            Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
        }
        AudioManager.Instance.PlaySFX(deathClip);
        GameManager.Instance.GameOver();

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
    }
    #endregion
}