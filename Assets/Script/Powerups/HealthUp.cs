using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : BasePickUp
{

    public override void Pickup()
    {
        Debug.Log("Picked up HealthUp");
        m_player.m_Health += 1;
        UIManager.Instance.UpdateHealthUI(m_player.m_Health);
    }
}
