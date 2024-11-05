using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickUp : MonoBehaviour,IPickup
{
    protected Player player;
    protected AudioClip pickupClip;
    protected void Start() 
    {
        player = GameObject.FindObjectOfType<Player>();
        pickupClip = Resources.Load<AudioClip>($"Sound/Effects/PowerUp");
    }
    public virtual void Pickup()
    {
        AudioManager.Instance.PlaySFX(pickupClip);
    }

    public virtual void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Bullet" && other.gameObject.GetComponent<Bullet>().b_owner == player.gameObject || other.gameObject.tag == "Player")
        {
            Pickup();
            Destroy(gameObject);
        }
    }
}
