using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference interactAction;

    public GameObject hands;

    public float moveSpeed = 5f;

    [SerializeField] private Vector2 moveInput;

    public bool isActive;
    [SerializeField] private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPressed;
            interactAction.action.canceled += OnInteractReleased;
            
        }
    }

    void OnDisable()
    {
        moveAction?.action.Disable();

        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPressed;
            interactAction.action.canceled -= OnInteractReleased;
            interactAction.action.Disable();
        }
    }

    void OnInteractPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact Pressed");

        // Pick up or interact logic here
        
    }

    void OnInteractReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Interact Released");
        
    }

    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 move = transform.TransformDirection(inputDir) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            Debug.Log("Player in range to interact");
        }
    }






}
