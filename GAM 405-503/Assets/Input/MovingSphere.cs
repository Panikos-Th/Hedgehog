using UnityEngine;

public class MovingSphere : MonoBehaviour
{
 
    [SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f;
    float maxAcceleration = 10f;
    private Rigidbody rb;
    private Vector3 velocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocity = rb.linearVelocity;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {

    }
    void FixedUpdate()
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


    }
}
