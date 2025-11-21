using UnityEditor.U2D;
using UnityEngine;

public class HedgehogController : MonoBehaviour
{

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    private Rigidbody rb;
    private Vector3 currentVelocity;

    [SerializeField]
    public Transform cameraTransform;
    private Quaternion initialRotation;

    private MeshRenderer meshRendererBall;
    public MeshRenderer meshRendererHog0;
    public MeshRenderer meshRendererHog1;
    public MeshRenderer meshRendererHog2;
    public MeshRenderer meshRendererHog3;
    public GameObject hedgehogModel;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        meshRendererBall = GetComponent<MeshRenderer>();



        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        meshRendererBall.enabled = false;
        meshRendererHog0.enabled = true;
        meshRendererHog1.enabled = true;
        meshRendererHog2.enabled = true;
        meshRendererHog3.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        initialRotation = this.transform.rotation;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleRotationContrainsts();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotation;
            this.transform.rotation = initialRotation;

            meshRendererBall.enabled = false;

            meshRendererHog0.enabled = true;
            meshRendererHog1.enabled = true;
            meshRendererHog2.enabled = true;
            meshRendererHog3.enabled = true;
        }


    }
    void FixedUpdate()
    {


        Movement();
        LookAtCamera();
    }

    private void Movement()
    {
        //this.transform.rotation = Camera.main.transform.rotation;
        

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

    private void LookAtCamera()
    {
        if (cameraTransform == null || hedgehogModel == null)
        return;

    // If ball is active → rolling → don't rotate hedgehog model
    if (meshRendererBall != null && meshRendererBall.enabled)
        return;

    // Face the camera direction (but keep the model upright)
    Vector3 forward = cameraTransform.forward;
    forward.y = 0f;

    if (forward.sqrMagnitude < 0.01f)
        return;

    Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

    hedgehogModel.transform.rotation = Quaternion.Slerp(
        hedgehogModel.transform.rotation,
        targetRotation,
        10f * Time.deltaTime
    );
    }

    private void ToggleRotationContrainsts()
    {
        if (rb == null) return;

        bool isRotationFrozen = (rb.constraints & RigidbodyConstraints.FreezeRotation) == RigidbodyConstraints.FreezeRotation;

        meshRendererBall.enabled = true;
        meshRendererHog0.enabled = false;
        meshRendererHog1.enabled = false;
        meshRendererHog2.enabled = false;
        meshRendererHog3.enabled = false;

        if (isRotationFrozen)
        {
            rb.constraints &= ~RigidbodyConstraints.FreezeRotation;
        }

    }
}
