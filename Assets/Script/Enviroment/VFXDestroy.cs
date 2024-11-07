using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VFXDestroy : MonoBehaviour
{
    [SerializeField]
    private float m_DestroyTime = 1.0f;

    void Awake()
    {
        Destroy(gameObject, m_DestroyTime);
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene current)
    {
        Destroy(gameObject);
    }
}
