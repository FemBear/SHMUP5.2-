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
    internal GameObject owner;
    


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
        Debug.Log("Collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player") && other.gameObject != owner)
        {
            other.gameObject.GetComponent<Plane>().TakeDamage(b_Damage);
            SpriteRenderer playerSprite = other.gameObject.GetComponent<SpriteRenderer>();
            other.gameObject.GetComponent<Plane>().StartCoroutine(other.gameObject.GetComponent<Plane>().Flash(playerSprite));
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy") && other.gameObject != owner)
        {
            other.gameObject.GetComponent<Plane>().TakeDamage(b_Damage);
            SpriteRenderer enemySprite = other.gameObject.GetComponent<SpriteRenderer>();
            other.gameObject.GetComponent<Plane>().StartCoroutine(other.gameObject.GetComponent<Plane>().Flash(enemySprite));
            Destroy(gameObject);
        }
    }
}