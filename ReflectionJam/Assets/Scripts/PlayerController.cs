using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Vector2 moveDirection;
    private bool isAirborne;
    private bool hasDashed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 50f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private int maxJump = 2;
    private int jumpCount;

    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerDash;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerMove.ReadValue<Vector2>();

        if (moveDirection.magnitude > 1) moveDirection.Normalize(); // Stops diagonal movement from being faster

        PlayerJump();
        PlayerDash();
    }

    void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        // Camera-relative movement
        Vector3 forward = new Vector3(cameraTransform.forward.x,0,cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x,0,cameraTransform.right.z).normalized;

        Vector3 move = forward * moveDirection.y + right * moveDirection.x;
        playerRb.MovePosition(playerRb.position + move * speed * Time.fixedDeltaTime);

        // Player model rotates based on camera movement (jittery right now)
        Vector3 camViewDirection = forward;
        if(camViewDirection != Vector3.zero)
        {
            Quaternion tRotation = Quaternion.LookRotation(camViewDirection);
            playerRb.MoveRotation(Quaternion.Slerp(playerRb.rotation, tRotation, 10f * Time.fixedDeltaTime));
        }
    }

    void PlayerJump()
    {
        if (playerJump.triggered && jumpCount < maxJump)
        {
            playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCount++;
            isAirborne = true;
            Debug.Log("Jump Test");
        }
    }

    void PlayerDash()
    {
        if (playerDash.triggered)
        {
            if (!isAirborne)
            {
                playerRb.AddForce(transform.forward * (dashPower - 5), ForceMode.Impulse);
                Debug.Log("Ground Dash Test");
            } else if (isAirborne && !hasDashed)
            {
                playerRb.AddForce(transform.forward * dashPower, ForceMode.Impulse);
                hasDashed = true;
                Debug.Log("Air Dash Test");
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            // Checks if player lands on TOP of ground
            foreach(ContactPoint contactPoint in other.contacts)
            {
                if(Vector3.Dot(contactPoint.normal, Vector3.up) > 0.5f)
                {
                    jumpCount = 0;
                    isAirborne = false;
                    hasDashed = false;
                    break;
                }
            }
        }
    }

    void OnEnable()
    {
        playerMove.Enable();
        playerJump.Enable();
        playerDash.Enable();
    }

    void OnDisable()
    {
        playerMove.Disable();
        playerJump.Disable();
        playerDash.Disable();
    }
}
