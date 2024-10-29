using Mirror;
using System;
using UnityEngine;

public class ProjectileOnline : NetworkBehaviour
{
    bool inited;


    [Server]
    public void Init(GameObject myFather)
    {
        this.myFather = myFather;
        inited = true;
    }



    public float speed = 50f;
    public float lifetime = 2f;
    [NonSerialized]
    public GameObject myFather;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (inited && isServer)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (inited && isServer)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                NetworkServer.Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("PlayerModel") && collision.gameObject != myFather)
            {
                collision.gameObject.GetComponentInParent<PlayerControllerLocal>().TakeDamage(gameObject);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}