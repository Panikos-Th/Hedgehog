using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{

    private Material test;
    public InputActionReference moveAction;

    public float moveSpeed = 5f;

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        test = GetComponent<Renderer>().material;

        //test.color = Color.red;
        test.color = new Color(0.5f, 0f, 0f);
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

    }
    void OnDisable()
    {
        moveAction?.action.Disable();

    }


    private void HandleMovement()
    {
        // movement in physics step
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        // move relative to the player’s facing
        Vector3 move = transform.TransformDirection(inputDir) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
