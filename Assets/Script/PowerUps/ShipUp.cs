using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUp : BasePickUp
{
    public override void Pickup()
    {
        Debug.Log("Picked up ShipUp");
        m_player.ShipUpgrade();
    }
}
