using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShieldUp : BasePickUp
{
    public override void Pickup()
    {
        Debug.Log("Picked up ShieldUp");
        player.Shield();
    }
}
