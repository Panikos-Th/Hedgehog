using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    private bool rollRequested = false;
    private Coroutine rollCoroutine = null;
    [SerializeField] private float rollForce = 5f;
    [SerializeField] private float rollDuration = 5f;

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

        // record roll request (don't call physics here)
        rollRequested = (rollAction != null && rollAction.action.WasPressedThisFrame());

        //HandleLook();
    }

    void FixedUpdate()
    {
        HandleMovement();

        if (rollRequested)
        {
            rollRequested = false;
            HandleRoll();
        }
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
        if (rollCoroutine != null) return; // prevent stacking rolls
        rollCoroutine = StartCoroutine(DoRoll());
    }

    private IEnumerator DoRoll()
    {
        var originalConstraints = rb.constraints;
        // allow rotation/movement during the roll if desired
        rb.constraints = RigidbodyConstraints.None;

        rb.AddForce(transform.forward * rollForce, ForceMode.VelocityChange);
        Debug.Log("Roll action performed");

        yield return new WaitForSeconds(rollDuration);

        rb.constraints = originalConstraints;
        rollCoroutine = null;
    }
}
