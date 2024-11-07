using Mirror;
using System;
using Unity.VisualScripting;
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
    [ServerCallback]
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ShootFunc");
        if (inited && isServer)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                NetworkServer.Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("PlayerModel") && collision.gameObject != myFather)
            {
                Debug.Log("PlayerTakeDamage");

                collision.gameObject.GetComponentInParent<PlayerControllerOnline>().TakeDamage(gameObject);
                Debug.Log("BulletDestroy");

                NetworkServer.Destroy(gameObject);
            }
        }
    }
}