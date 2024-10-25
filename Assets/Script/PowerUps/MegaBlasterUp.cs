using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBlasterUp : BasePickUp
{
    public override void Pickup()
    {
        Debug.Log("Picked up MegaBlasterUp");
        player.MegaBusterActive();
    }
}
