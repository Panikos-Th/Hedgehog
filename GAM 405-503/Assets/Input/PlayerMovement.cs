using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction; // Vector2 (WASD / stick)
    public InputActionReference lookAction; // Vector2 (Mouse delta / right stick)

    public InputActionReference rollAction; // button

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform; // assign Main Camera here

    Rigidbody rb;
    Vector2 moveInput;
    Vector2 lookInput;
    float pitch = 0f; // up/down

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();
        rollAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();
        rollAction?.action.Disable();
    }

    void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        lookInput = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        rollAction?.action.WasPerformedThisFrame();

        HandleLook();
        HandleRoll();
    }

    void FixedUpdate()
    {
        HandleMovement();
        
    }

    void HandleLook()
    {
        // horizontal look (yaw) rotates the player
        float yaw = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * yaw);

        // vertical look (pitch) rotates the camera only
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // movement in physics step
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        //rb.AddForce(moveSpeed * inputDir * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // move relative to the player’s facing
        Vector3 move = transform.TransformDirection(inputDir) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
    

    private void HandleRoll()
    {
        if (rollAction != null && rollAction.action.WasPerformedThisFrame())
        {
            // perform roll action
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
            Debug.Log("Roll action performed");
        }
    }
}
