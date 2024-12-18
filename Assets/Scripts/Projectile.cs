using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    [NonSerialized]
    public GameObject myFather;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("PlayerModel") && collision.gameObject!=myFather)
        {
            string shooterName = myFather.GetComponent<PlayerControllerLocal>().playerName;

            collision.gameObject.GetComponentInParent<PlayerControllerLocal>().TakeDamage(shooterName);
            Destroy(gameObject);
        }
    }
}