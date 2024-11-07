using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Plane
{
    #region Variables
    [Header("Fuel")]
    [SerializeField]
    public float m_Fuel = 100;
    [SerializeField]
    public float m_MaxFuel = 100;
    [SerializeField]
    public float m_FuelConsumption = 1;

    [Header("ShipUpgrades")]
    [SerializeField]
    private Sprite[] m_ShipForms;
    [SerializeField]
    private int m_ShipForm = 0;
    [SerializeField]
    private int m_BulletCount = 1;
    [SerializeField]
    private float m_Spread = 0;
    [SerializeField]
    private float m_RepeatRate = 0.5f;

    [Header("PowerUp Settings")]
    [SerializeField]
    private bool m_CanMegaBuster = false;
    [SerializeField]
    private float m_shieldTime = 3;
    private GameObject m_Shield;
    PlayerControls m_InputControls;
    private bool isShooting = false;
    #endregion

    #region Basics
    private void Start()
    {
        Initialize();
        SetUpControlls();
    }

    public void Update()
    {
        FireRate();
        if (m_Fuel > 0)
        {
            m_Fuel -= m_FuelConsumption * Time.deltaTime;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateFuelBar(m_Fuel);
            }
        }
        else
        {
            GameManager.Instance.GameOver();
        }

        if (isShooting && m_CanFire <= 0)
        {
            Fire();
            m_CanFire = m_FireRate + m_RepeatRate;
        }
        else if (m_CanFire > 0)
        {
            m_CanFire -= Time.deltaTime;
        }
    }
    #endregion

    #region Input
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
            isShooting = true;
        }
        else if (context.canceled)
        {
            isShooting = false;
        }
    }

    private void MegaBuster(InputAction.CallbackContext context)
    {
        if (context.performed && m_CanMegaBuster)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            AudioManager.Instance.PlaySFX(m_ShootClip);
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<BaseEnemy>().TakeDamage(1000);
                m_CanMegaBuster = false;
            }
        }
    }
    #endregion

    #region Shooting
    public override void Fire()
    {
        if (m_BulletSpawn != null)
        {
            AudioManager.Instance.PlaySFX(m_ShootClip);
            if (m_BulletCount > 1)
            {
                float spreadAngle = m_Spread / (m_BulletCount - 1);
                for (int i = 0; i < m_BulletCount; i++)
                {
                    float angle = -m_Spread / 2 + spreadAngle * i;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);
                    GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, rotation);
                    bullet.GetComponent<Bullet>().SetBullet(gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
                }
            }
            else
            {
                GameObject bullet = Instantiate(m_Bullet, m_BulletSpawn.position, Quaternion.identity);
                bullet.GetComponent<Bullet>().SetBullet(gameObject, m_Damage, m_BulletSpeed, m_LifeTime);
            }
        }
    }
    #endregion

    #region Damage

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && !m_isInvulnerable)
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
    internal override void TakeDamage(int damage)
    {
        if (!m_isInvulnerable)
        {
            m_Health -= damage;
            if (m_Health <= 0)
            {
                if (m_ExplosionFX != null)
                {
                    Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
                }
                AudioManager.Instance.PlaySFX(m_DeathClip);
                GameManager.Instance.GameOver();

                Destroy(gameObject);
            }
            UIManager.Instance.UpdateHealthUI(m_Health);
            StartCoroutine(Invulnerability(m_iFrames, m_iFrameDuration));
        }
    }
    #endregion

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
                m_Spread = 0;
                m_BulletSpeed = 10;
                m_Damage = 1;
                break;
            case 1:
                m_BulletCount = 2;
                m_Spread = 15;
                m_BulletSpeed = 15;
                m_Damage = 2;
                break;
            case 2:
                m_BulletCount = 3;
                m_Spread = 30;
                m_BulletSpeed = 20;
                m_Damage = 3;
                break;
        }
    }

    public IEnumerator Shield()
    {
        m_Shield.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(m_shieldTime);
        m_Shield.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void AddFuel(float fuel)
    {
        m_Fuel += fuel;
        if (m_Fuel > m_MaxFuel)
        {
            m_Fuel = m_MaxFuel;
        }
        UIManager.Instance.UpdateFuelBar(m_Fuel);
    }

    public void MegaBusterActive()
    {
        m_CanMegaBuster = true;
    }
    #endregion

    #region SetUp
    private void SetUpControlls()
    {
        m_InputControls = InputManager.Instance.GetPlayerControls();
        m_InputControls.Player.Shoot.performed += Shoot;
        m_InputControls.Player.Shoot.canceled += Shoot;
        m_InputControls.Player.MegaBuster.performed += MegaBuster;
        m_InputControls.Player.WASD.performed += Movement;
        m_InputControls.Player.Move.performed += Movement;
        m_InputControls.Player.WASD.canceled += Movement;
        m_InputControls.Player.Move.canceled += Movement;
    }

    private void Initialize()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_ShootClip = Resources.Load<AudioClip>($"Sound/Effects/Shoot");
        m_DeathClip = Resources.Load<AudioClip>($"Sound/Effects/Death");
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowCursor(false);
            }
            m_BulletSpawn = transform.GetChild(0).transform;
            m_Shield = transform.GetChild(1).gameObject;
            m_Shield.SetActive(false);
            transform.position = new Vector3(0, -3.5f, 0);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthUI(m_Health);
                UIManager.Instance.UpdateScoreAndComboUI(0, 1);
                UIManager.Instance.UpdateFuelBar(m_Fuel);
            }
        }
    }
    #endregion
}