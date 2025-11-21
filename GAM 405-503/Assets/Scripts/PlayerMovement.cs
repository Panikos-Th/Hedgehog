using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public GameObject currentInteractable;

    public float sphereRadius;
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

        if(Physics.CheckSphere(transform.position, sphereRadius, LayerMask.GetMask("Interactable")))
        {
            Debug.Log("Player in range to interact with something");

            if (currentInteractable != null)
            {
                Debug.Log("Interacting with " + currentInteractable.name);
                // Example interaction: pick up the object
                currentInteractable.transform.SetParent(hands.transform);
                currentInteractable.transform.localPosition = Vector3.zero;
                currentInteractable.transform.localRotation = Quaternion.identity;
            }
        }

        

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
