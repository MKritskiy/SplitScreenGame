using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public int health = 3;
    public float grabRadius = 10f;
    public float shootInterval = 0.5f;
    public Transform cameraSpawnPoint;


    private bool isGrabbing = false;
    private Transform grabbedPole;
    private Vector3 grabPoint;
    private Vector3 releaseVelocity;
    private PlayerInputActions playerInput;
    private InputAction grabAction;
    private InputAction shootAction;
    private InputAction moveAction;
    private float lastShootTime = 0f;
    void Awake()
    {
        
        playerInput = new PlayerInputActions();
        grabAction = playerInput.Player.Grab;
        shootAction = playerInput.Player.Shoot;
        releaseVelocity = transform.forward;
    }

    void OnEnable()
    {
        grabAction.Enable();
        shootAction.Enable();

        grabAction.performed += GrabPole;
        shootAction.performed += Shoot;
    }

    void OnDisable()
    {
        grabAction.performed -= GrabPole;
        shootAction.performed -= Shoot;

        grabAction.Disable();
        shootAction.Disable();
    }

    void Start()
    {
    }

    void Update()
    {
        //Debug.Log(isGrabbing);
    }

    void FixedUpdate()
    {
        if (!isGrabbing)
        {
            transform.position += releaseVelocity.normalized * speed * Time.deltaTime;
        }
        else { 
            RotateAroundPole();
        }
        
    }

    private void GrabPole(InputAction.CallbackContext obj)
    {
        if (isGrabbing)
        {
            ReleasePole();
            return;
        }

        float oldDist = 9999f;
        GameObject closestObject = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius);

        foreach (Collider collider in colliders)
        {
            Debug.Log("Found object: " + collider.gameObject.name);
            if (collider.transform.childCount > 0 && collider.transform.GetChild(0).CompareTag("Pole"))
            {
                float dist = Vector3.Distance(gameObject.transform.position, collider.transform.position);
                Debug.Log("Distance to pole: " + dist);
                if (dist < oldDist)
                {
                    closestObject = collider.gameObject;
                    oldDist = dist;
                }
            }
        }

        if (closestObject != null)
        {
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
        grabbedPole = null;

        Vector3 velocity = (grabPoint - transform.position);
        releaseVelocity = Vector3.Cross(velocity, Vector3.up);
    }

    void RotateAroundPole()
    {
        if (grabbedPole != null)
        {
            grabbedPole.Rotate(0, rotationSpeed*Time.deltaTime, 0, Space.Self);
            
        }
    }

    void Shoot(InputAction.CallbackContext obj)
    {
        if (Time.time - lastShootTime >= shootInterval)
        {
            var bullet = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            bullet.GetComponent<Projectile>().myFather = gameObject;
            lastShootTime = Time.time;
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
            GameManager.playerCount--;
            if (GameManager.playerCount <= 1)
            {
                GameManager.EndGame(gameObject);
            }
        }
    }


}
