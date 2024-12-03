using Photon.Pun;
using System;
using UnityEngine;

public class ProjectileOnline : MonoBehaviourPun
{
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
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("PlayerModel") && collision.gameObject != myFather)
            {
                string shooterName = myFather.GetComponent<PlayerControllerOnline>().playerName;
                collision.gameObject.GetComponentInParent<PlayerControllerOnline>().photonView.RPC("TakeDamage", RpcTarget.All, shooterName);
                //collision.gameObject.GetComponentInParent<PlayerControllerOnline>().TakeDamage(shooterName);
                if (gameObject!=null) PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void Init(GameObject father)
    {
        myFather = father;
    }
}
