using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HedgehogController : MonoBehaviour
{
    [Header("Input (assign from your .inputactions asset)")]
    public InputActionReference moveAction;   // Vector2 (WASD / left stick)
    public InputActionReference lookAction;   // Vector2 (mouse delta / right stick) [optional]
    public InputActionReference rollAction;   // Button (Shift)

    [Header("Camera Look (optional)")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    [Header("Walk")]
    public float walkSpeed = 5f;          // m/s
    public float turnSpeed = 10f;         // how quickly hedgehog turns to move dir

    [Header("Roll")]
    public float rollTorque = 40f;        // torque added each fixed step while rolling
    public float rollMaxSpeed = 12f;      // max planar speed while rolling
    public float rollEnterImpulse = 6f;   // initial burst when entering roll
    public float steeringWhileRolling = 0.8f; // 0..1 steer strength with input while rolling

    [Header("Friction (optional)")]
    public PhysicsMaterial walkFriction;   // higher friction to stand/walk
    public PhysicsMaterial rollFriction;   // medium/low friction to keep rolling

    Rigidbody rb;
    Collider col;
    Vector2 moveInput, lookInput;

    float pitch;
    bool isRolling = false;
    float worldRadius = 0.5f;
    const float MinRadius = 0.001f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.maxAngularVelocity = 100f;

        worldRadius = GetWorldRadius(col);

        EnterWalk(); // default
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();

        if (rollAction != null)
        {
            rollAction.action.performed += OnRollPressed;  // Shift down -> start roll
            rollAction.action.canceled  += OnRollReleased; // Shift up   -> stop roll
            rollAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (rollAction != null)
        {
            rollAction.action.performed -= OnRollPressed;
            rollAction.action.canceled  -= OnRollReleased;
            rollAction.action.Disable();
        }
        moveAction?.action.Disable();
        lookAction?.action.Disable();
    }

    void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        lookInput = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        if (cameraTransform) HandleLook();

        // if scaled at runtime, keep the radius fresh
        worldRadius = GetWorldRadius(col);
    }

    void FixedUpdate()
    {
        if (isRolling)
            FixedRoll();
        else
            FixedWalk();
    }

    // --------- Walk mode ---------
    void FixedWalk()
    {
        // desired movement in world space (relative to current facing)
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 targetVel = transform.TransformDirection(inputDir) * walkSpeed;
        rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);

        // rotate hedgehog body toward move direction (if moving)
        if (inputDir.sqrMagnitude > 0.0001f)
        {
            Vector3 faceDir = transform.TransformDirection(inputDir);
            Quaternion targetRot = Quaternion.LookRotation(faceDir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }
    }

    // --------- Roll mode ---------
    void FixedRoll()
    {
        // planar velocity and forward axis for torque
        Vector3 planarVel = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        Vector3 forward = planarVel.sqrMagnitude > 0.01f ? planarVel.normalized : transform.forward;

        // steer with input (subtle), by blending desired direction
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 0.0001f)
        {
            inputDir = transform.TransformDirection(inputDir).normalized;
            forward = Vector3.Slerp(forward, inputDir, steeringWhileRolling * Time.fixedDeltaTime).normalized;
        }

        // torque axis for rolling without slipping is up × direction
        Vector3 omegaAxis = Vector3.Cross(Vector3.up, forward).normalized;

        // apply torque to keep rolling
        rb.AddTorque(omegaAxis * rollTorque, ForceMode.Acceleration);

        // cap planar speed
        float speed = planarVel.magnitude;
        if (speed > rollMaxSpeed)
        {
            Vector3 capped = planarVel.normalized * rollMaxSpeed;
            rb.linearVelocity = new Vector3(capped.x, rb.linearVelocity.y, capped.z);
        }

        // keep angular velocity roughly consistent with v = ω r (feels “real”)
        float targetOmega = Mathf.Clamp(speed, 0f, rollMaxSpeed) / Mathf.Max(MinRadius, worldRadius);
        Vector3 currentOmega = rb.angularVelocity;
        // project current omega onto axis, then nudge toward target
        float along = Vector3.Dot(currentOmega, omegaAxis);
        float correction = (targetOmega - along);
        rb.angularVelocity = currentOmega + omegaAxis * correction;
    }

    // --------- State switches ---------
    void OnRollPressed(InputAction.CallbackContext _)
    {
        if (!isRolling)
            EnterRoll();
    }

    void OnRollReleased(InputAction.CallbackContext _)
    {
        if (isRolling)
            EnterWalk();
    }

    void EnterWalk()
    {
        isRolling = false;

        // stand up: stop spinning on X/Z so hedgehog doesn't topple while walking
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // friction for walking
        if (walkFriction) col.material = walkFriction;
    }

    void EnterRoll()
    {
        isRolling = true;

        // allow free spin
        rb.constraints = RigidbodyConstraints.None;

        // swap material for smoother rolling
        if (rollFriction) col.material = rollFriction;

        // initial burst in current facing
        Vector3 forward = transform.forward;
        Vector3 planar = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        Vector3 v = rb.linearVelocity;
        Vector3 burst = planar * rollEnterImpulse;
        rb.linearVelocity = new Vector3(burst.x, v.y, burst.z);

        // match initial spin to burst
        float omega = rollEnterImpulse / Mathf.Max(MinRadius, worldRadius);
        Vector3 omegaAxis = Vector3.Cross(Vector3.up, planar).normalized;
        rb.angularVelocity = omegaAxis * omega;
    }

    // --------- Look (optional) ---------
    void HandleLook()
    {
        // rotate body around Y
        float yaw = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * yaw, Space.World);

        // pitch camera only
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    // --------- Utility ---------
    static float GetWorldRadius(Collider c)
    {
        // robust under scaling; for a true sphere all extents are equal.
        var e = c.bounds.extents;
        return Mathf.Max(e.x, Mathf.Max(e.y, e.z));
    }
}
