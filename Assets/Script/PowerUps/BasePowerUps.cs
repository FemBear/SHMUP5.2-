using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickUp : MonoBehaviour,IPickup
{
    protected Player player;
    protected void Start() 
    {
        player = GameObject.FindObjectOfType<Player>();
    }
    public virtual void Pickup()
    {
        Debug.Log("Picked up");
    }

    public virtual void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player")
        {
            Pickup();
            Destroy(gameObject);
        }
    }
}
