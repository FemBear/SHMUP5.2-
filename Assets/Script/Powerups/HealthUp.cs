using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : BasePickUp
{

    public override void Pickup()
    {
        Debug.Log("Picked up HealthUp");
        player.m_Health += 1;
    }
}
