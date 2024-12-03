using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerOnline : MonoBehaviourPunCallbacks, PlayerController
{
    public float speed = 0f;
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

    void Awake()
    {
        playerInput = new PlayerInputActions();

        grabAction = playerInput.Player.Grab;
        shootAction = playerInput.Player.Shoot;

        playerName = "Player " + (PhotonNetwork.CurrentRoom.PlayerCount + 1);
        releaseVelocity = transform.forward;
    }

    void OnEnable()
    {
        if (photonView.IsMine)
        {
            grabAction.Enable();
            shootAction.Enable();

            grabAction.performed += GrabPole;
            shootAction.performed += Shoot;
        }
    }

    void OnDisable()
    {
        if (photonView.IsMine)
        {
            grabAction.performed -= GrabPole;
            shootAction.performed -= Shoot;

            grabAction.Disable();
            shootAction.Disable();
        }
    }

    void Start()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

        // јктивируем камеру только дл€ локального игрока
        if (photonView.IsMine)
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = true;
            localCamera.rect = new Rect(0, 0, 1, 1);
        }
    }

    void Update()
    {
    }
    void FixedUpdate()
    {
        if (lineRenderer == null)
            return;
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
    [PunRPC]
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

    void ReleasePole()
    {
        isGrabbing = false;
        gameObject.transform.SetParent(null);
        if (grabbedPole != null)
        {
            grabbedPole.GetComponentInParent<Renderer>().material = poleMaterial;
            grabbedPole = null;
        }
        Vector3 velocity = (grabPoint - transform.position);
        releaseVelocity = Vector3.Cross(velocity, Vector3.up);
    }
    
    void RotateAroundPole()
    {

        if (grabbedPole != null)
        {
            grabbedPole.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
        }
    }

    void Shoot(InputAction.CallbackContext obj)
    {

        if (Time.time - lastShootTime >= shootInterval)
        {
            var bullet = PhotonNetwork.Instantiate(projectilePrefab.name, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            bullet.GetComponent<ProjectileOnline>().Init(gameObject);
            lastShootTime = Time.time;
        }
    }

    [PunRPC]
    public void TakeDamage(string shooterName)
    {
        health--;
        if (health <= 0)
        {
            GameManager.Instance.playerCount--;
            Camera.DieScreen.SetActive(true);
            if (GameManager.Instance.playerCount <= 1)
            {
                GameManager.Instance.EndGame(shooterName);
            }
            lineRenderer.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (grabbedPole != null)
        {
            grabbedPole.GetComponentInParent<Renderer>().material = poleMaterial;
            grabbedPole = null;
        }
        if (lineRenderer != null)
        {
            lineRenderer.gameObject.SetActive(false);
        }
    }
}
