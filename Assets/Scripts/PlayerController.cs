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

    private Rigidbody rb;
    private bool isGrabbing = false;
    private Transform grabbedPole;
    private Vector3 grabPoint;
    private Vector3 releaseVelocity;
    private PlayerInputActions playerInput;
    private InputAction grabAction;
    private InputAction shootAction;
    private InputAction moveAction;
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
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log(isGrabbing);
    }

    void FixedUpdate()
    {
        if (!isGrabbing)
        {
            rb.position += releaseVelocity.normalized * speed * Time.deltaTime;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
        float oldDist = 9999f;
        GameObject closetsObject = null;
        foreach (Collider collider in colliders)
        {
            if (collider.transform.GetChild(0).CompareTag("Pole"))
            {
                float dist = Vector3.Distance(this.gameObject.transform.position, collider.transform.position);
                if (dist < oldDist)
                {
                    closetsObject = collider.gameObject;
                    oldDist = dist;
                }
            }
        }
        if (closetsObject != null)
        {
            grabbedPole = closetsObject.transform.GetChild(0).transform;
            grabPoint = grabbedPole.position;
            gameObject.transform.SetParent(grabbedPole, true);
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
        Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    
}
