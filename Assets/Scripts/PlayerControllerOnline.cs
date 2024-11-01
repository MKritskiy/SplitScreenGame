using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerOnline : NetworkBehaviour, PlayerController
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public int health = 3;
    public float grabRadius = 10f;
    public float shootInterval = 0.5f;
    public Transform cameraSpawnPoint;
    [NonSerialized]
    public string playerName;
    public Material poleMaterial;
    private bool isGrabbing = false;
    private Transform grabbedPole;
    private Vector3 grabPoint;
    private Vector3 releaseVelocity;
    private PlayerInputActions playerInput;
    private InputAction grabAction;
    private InputAction shootAction;
    private InputAction moveAction;
    private float lastShootTime = 0f;
    private GameManager gameManager;
    public LineRenderer lineRenderer;
    GameObject closestObject = null;

    void Awake()
    {
        
        playerInput = new PlayerInputActions();
        gameManager = FindFirstObjectByType<GameManager>();            
        grabAction = playerInput.Player.Grab;
        shootAction = playerInput.Player.Shoot;
        

        releaseVelocity = transform.forward;


    }

    void OnEnable()
    {
            grabAction.Enable();
            shootAction.Enable();
        if (isServer)
        {
            grabAction.performed += GrabPole;
            shootAction.performed += Shoot;
        } else
        {
            grabAction.performed += CmdGrabPole;
            shootAction.performed += CmdShoot;
        }
        
        
    }

    void OnDisable()
    {
        if (isServer)
        {
            grabAction.performed -= GrabPole;
            shootAction.performed -= Shoot;
        }
        else
        {
            grabAction.performed -= CmdGrabPole;
            shootAction.performed -= CmdShoot;
        }

            grabAction.Disable();
            shootAction.Disable();
        
    }

    void Start()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

    }

    void Update()
    {
        
        //Debug.Log(isGrabbing);
    }
    
    void FixedUpdate()
    {
        
        if (isOwned)
        {
            if (!isGrabbing)
            {
                closestObject = FindClosestPole();
                if (closestObject != null)
                {
                    lineRenderer.enabled = true;
                    Color color = lineRenderer.material.color;
                    color.a = 0.3f;
                    lineRenderer.material.color = color;
                    lineRenderer.SetPosition(0, closestObject.transform.position);
                    lineRenderer.SetPosition(1, gameObject.transform.position);
                }
                else
                {
                    lineRenderer.enabled = false;

                }
                transform.position += releaseVelocity.normalized * speed * Time.deltaTime;
            }
            else
            {
                if (closestObject != null)
                {
                    Color color = lineRenderer.material.color;
                    color.a = 1f;
                    lineRenderer.material.color = color;
                    lineRenderer.SetPosition(0, closestObject.transform.position);
                    lineRenderer.SetPosition(1, gameObject.transform.position);
                }
                if (isServer) RotateAroundPole();
                else CmdRotateAroundPole();
            }
        }
        
    }
    [Server]
    private GameObject FindClosestPole()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius);
        GameObject tmpClosestObject = null;
        float oldDist = 9999f;
        foreach (Collider collider in colliders)
        {
            if (collider.transform.childCount > 0 && collider.transform.GetChild(0).CompareTag("Pole"))
            {
                float dist = Vector3.Distance(gameObject.transform.position, collider.transform.position);
                if (dist < oldDist)
                {
                    tmpClosestObject = collider.gameObject;
                    oldDist = dist;
                }
            }
        }
        return tmpClosestObject;
    }
    [Server]
    private void GrabPole(InputAction.CallbackContext obj)
    {
        
        if (isGrabbing)
        {
            ReleasePole();
            return;
        }
        if (closestObject == null)
        {
            closestObject = FindClosestPole();
        }

        if (closestObject != null)
        {
            closestObject.GetComponentInParent<Renderer>().material.color = Color.yellow;
            grabbedPole = closestObject.transform.GetChild(0).transform;
            grabPoint = grabbedPole.position;
            gameObject.transform.SetParent(grabbedPole);
            isGrabbing = true;
        }
        
    }
    [ClientRpc]
    private void RpcGrabPole(bool isGrabbing)
    {
        this.isGrabbing = isGrabbing;
    }
    [Command]
    private void CmdGrabPole(InputAction.CallbackContext obj)
    {
        GrabPole(obj);
        RpcGrabPole(isGrabbing);
    }
    [Server]
    void ReleasePole()
    {
        isGrabbing = false;
        gameObject.transform.SetParent(null);
        grabbedPole.GetComponentInParent<Renderer>().material = poleMaterial;
        grabbedPole = null;

        Vector3 velocity = (grabPoint - transform.position);
        releaseVelocity = Vector3.Cross(velocity, Vector3.up);
    }
    [Server]
    void RotateAroundPole()
    {
        if (grabbedPole != null)
        {
            grabbedPole.Rotate(0, rotationSpeed*Time.deltaTime, 0, Space.Self);
            
        }
    }
    [Command]
    void CmdRotateAroundPole()
    {
        RotateAroundPole();
    }
    [Server]
    void Shoot(InputAction.CallbackContext obj)
    {

        if (Time.time - lastShootTime >= shootInterval)
        {
            var bullet = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            NetworkServer.Spawn(bullet);
            bullet.GetComponent<ProjectileOnline>().Init(gameObject);

            lastShootTime = Time.time;
        }
        
    }
    [Command]
    void CmdShoot(InputAction.CallbackContext obj)
    {
        Shoot(obj);
    }

    public void TakeDamage(GameObject bullet)
    {
        health--;
        if (health <= 0)
        {

            GameManager.playerCount--;
            if (GameManager.playerCount <= 1)
            {
                gameManager.EndGame(bullet.GetComponent<Projectile>().myFather);
            }
            
            cameraSpawnPoint
                .gameObject
                .GetComponentInChildren<CameraScript>()
                .DieScreen.SetActive(true);

            Destroy(lineRenderer.gameObject);
        }
    }


}
