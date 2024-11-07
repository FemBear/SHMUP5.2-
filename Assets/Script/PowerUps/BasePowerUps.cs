using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickUp : MonoBehaviour, IPickup
{
    protected Player m_player;
    protected AudioClip m_pickupClip;
    protected void Start()
    {
        m_player = GameObject.FindObjectOfType<Player>();
        m_pickupClip = Resources.Load<AudioClip>($"Sound/Effects/PowerUp");
    }
    public virtual void Pickup()
    {
        AudioManager.Instance.PlaySFX(m_pickupClip);
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet" && other.gameObject.GetComponent<Bullet>().b_owner == m_player.gameObject || other.gameObject.tag == "Player")
        {
            Pickup();
            Destroy(gameObject);
        }
    }
}
