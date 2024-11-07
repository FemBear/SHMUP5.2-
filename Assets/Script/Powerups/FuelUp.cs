using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelUp : BasePickUp
{
    public float m_fuelAmount = 10;
    public override void Pickup()
    {
        base.Pickup();
        m_player.AddFuel(m_fuelAmount);
    }
}
