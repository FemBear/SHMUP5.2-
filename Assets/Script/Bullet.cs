using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    internal int b_Damage;
    [SerializeField]
    internal int b_Speed = 10;
    [SerializeField]
    internal float b_LifeTime = 5;
    [SerializeField]
    public GameObject b_owner;



    void Start()
    {
        Destroy(gameObject, b_LifeTime);
    }
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * b_Speed);
    }

    internal void SetBullet(GameObject owner, int damage, int speed, float lifeTime)
    {
        this.b_owner = owner;
        b_Damage = damage;
        b_Speed = speed;
        b_LifeTime = lifeTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject != b_owner && b_owner.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(b_Damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy") && other.gameObject != b_owner && b_owner.CompareTag("Player"))
        {
            other.gameObject.GetComponent<BaseEnemy>().TakeDamage(b_Damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy") && b_owner.CompareTag("Enemy"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
    }
}