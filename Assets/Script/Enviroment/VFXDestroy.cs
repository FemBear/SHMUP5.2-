using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroy : MonoBehaviour
{
    [SerializeField]
    private float m_DestroyTime = 1.0f;
    void Awake()
    {
        Destroy(gameObject, m_DestroyTime);
    }
}
