using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControler : MonoBehaviour
{

    private Material test;
    public InputActionReference moveAction;
    public InputActionReference rollAction;

    public float moveSpeed = 5f;

    public bool isRolling = false;
    public bool isWalking = true;

   

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private Rigidbody rb;

     enum PlayerSate
    {
        Walking,
        Rolling
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        test = GetComponent<Renderer>().material;

        //test.color = Color.red;
        test.color = new Color(0.5f, 0f, 0f);
        isRolling = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        rollAction?.action.Enable();
        if (rollAction != null)
        {
            rollAction.action.performed += OnRollPressed;  // Shift down -> start roll
            rollAction.action.canceled += OnRollReleased; // Shift up   -> stop roll
            rollAction.action.Enable();
        }

    }
    void OnDisable()
    {
        moveAction?.action.Disable();
        if (rollAction != null)
        {
            rollAction.action.performed -= OnRollPressed;
            rollAction.action.canceled -= OnRollReleased;
            rollAction.action.Disable();
        }

    }

    void OnRollPressed(InputAction.CallbackContext _)
    {
        isRolling = true;
        Debug.Log("Roll started");
    }

    void OnRollReleased(InputAction.CallbackContext _)
    {
        isRolling = false;
        Debug.Log("Roll ended");
    }


    private void HandleMovement()
    {
        if (isRolling == false)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
            if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();
            // move relative to the player’s facing

            Vector3 move = transform.TransformDirection(inputDir) * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            // movement in physics step
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
            if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();
            // move relative to the player’s facing

            Vector3 move = transform.TransformDirection(inputDir) * moveSpeed * Time.fixedDeltaTime;
            rb.AddForce(move * 10f);

            //rb.MovePosition(rb.position + move);
            // movement in physics step
            StartCoroutine(RollDuration(new WaitForSeconds(5f)));

        }

    }

    private IEnumerator RollDuration(WaitForSeconds wait)
    {
        yield return new WaitForSeconds(5f);
        isRolling = false;
    }

    private void SateSwitcher()
    {
        switch(currentState)
        {
            case PlayerSate.Walking:
                HandleMovement();
                break;
            case PlayerSate.Rolling:
                HandleRolling();
                break;
        }
      
    }

    private void HandleRolling()
    {
        
          Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
                
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        // Rolling logic here
    }
}
