using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //grab the damage from the plane class
    [SerializeField]
    internal int b_Damage;
    [SerializeField]
    internal int b_Speed = 10;
    [SerializeField]
    internal float b_LifeTime = 5;


    void Start()
    {
        Destroy(gameObject, b_LifeTime);
    }
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * b_Speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Plane>().TakeDamage(b_Damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Plane>().TakeDamage(b_Damage);
            Destroy(gameObject);
        }
    }
}
