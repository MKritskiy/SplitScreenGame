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
    public CameraScript Camera;
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
    
    public LineRenderer lineRenderer;
    GameObject closestObject = null;


    [SyncVar]
    private bool isGrabbingSync;

    [SyncVar]
    private Vector3 grabPointSync;

    [SyncVar]
    private Vector3 releaseVelocitySync;

    void Awake()
    {
        
        playerInput = new PlayerInputActions();
                    
        grabAction = playerInput.Player.Grab;
        shootAction = playerInput.Player.Shoot;

        playerName = "Player " + (NetworkManagerCustom.playerCount++ + 1);
        releaseVelocity = transform.forward;


    }

    void OnEnable()
    {
            grabAction.Enable();
            shootAction.Enable();
        
            grabAction.performed += CmdGrabPole;
            shootAction.performed += CmdShoot;

        
        
    }

    void OnDisable()
    {
        
            grabAction.performed -= CmdGrabPole;
            shootAction.performed -= CmdShoot;


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
        if (lineRenderer == null)
            return;
        if (!isGrabbingSync)
        //if (!isGrabbing)
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
            //transform.position += releaseVelocity.normalized * speed * Time.deltaTime;
            transform.position += releaseVelocitySync.normalized * speed * Time.deltaTime;
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
            RotateAroundPole();

        }
        
        
    }
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
    [ClientRpc]
    private void GrabPole(InputAction.CallbackContext obj)
    {
        if (isGrabbingSync)
        //if (isGrabbing)
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
            //grabPoint = grabbedPole.position;
            grabPointSync = grabbedPole.position;
            gameObject.transform.SetParent(grabbedPole);
            
            //isGrabbing = true;
            isGrabbingSync = true;
        }
        
    }

    [Command]
    private void CmdGrabPole(InputAction.CallbackContext obj)
    {
        
        GrabPole(obj);
    }
    
    void ReleasePole()
    {
        isGrabbingSync = false;
        //isGrabbing = false;
        gameObject.transform.SetParent(null);
        grabbedPole.GetComponentInParent<Renderer>().material = poleMaterial;
        grabbedPole = null;

        //Vector3 velocity = (grabPoint - transform.position);
        Vector3 velocity = (grabPointSync - transform.position);
        //releaseVelocity = Vector3.Cross(velocity, Vector3.up);
        releaseVelocitySync = Vector3.Cross(velocity, Vector3.up);
    }
    [Command]
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
                bullet.GetComponent<ProjectileOnline>().Init(gameObject);
                NetworkServer.Spawn(bullet);

                lastShootTime = Time.time;
            }
        
    }
    [Command]
    void CmdShoot(InputAction.CallbackContext obj)
    {
        Shoot(obj);
    }
    [ClientRpc]
    public void TakeDamage(string shooterName)
    {
        health--;
        Debug.Log("health--");

        if (health <= 0)
        {
            NetworkManagerCustom.playerCount--;
            Debug.Log("playerCount--");

            Camera.DieScreen.SetActive(true);
            Debug.Log("BeforeDelete");
            if (NetworkManagerCustom.playerCount <= 1)
            {
                SetWinner(shooterName);
                EndGame(shooterName);
            }

            Destroy(lineRenderer.gameObject);
        }
    }

    [Command]
    public void EndGame(string shooterName)
    {
        GameManager.Instance.EndGame(shooterName);
    }

    [Command]
    public void SetWinner(string shooterName)
    {
        PlayerPrefs.SetString("WinnerName", shooterName);
    }

}
