using UnityEngine;

public class MovingSphere : MonoBehaviour
{
 
     [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] 
    float maxAcceleration = 10f;
    
    private Rigidbody rb;
    private Vector3 currentVelocity; 

    [SerializeField] 
    public Transform cameraTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();


        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleRotationContrainsts();
        }
    }
    void FixedUpdate()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);


        if (rb == null || cameraTransform == null)
            return;


        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;


        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();


        Vector3 desiredVelocity = (camRight * playerInput.x + camForward * playerInput.y) * maxSpeed;


        currentVelocity = rb.linearVelocity;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;


        currentVelocity.x = Mathf.MoveTowards(
            currentVelocity.x,
            desiredVelocity.x,
            maxSpeedChange
        );
        currentVelocity.z = Mathf.MoveTowards(
            currentVelocity.z,
            desiredVelocity.z,
            maxSpeedChange
        );


        rb.linearVelocity = currentVelocity;
    }
    


    private void ToggleRotationContrainsts ()
    {
        if (rb == null) return;

        bool isRotationFrozen = (rb.constraints & RigidbodyConstraints.FreezeRotation) == RigidbodyConstraints.FreezeRotation;

        if (isRotationFrozen)
        {
            rb.constraints &= ~RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotation;
        }
    }
}
