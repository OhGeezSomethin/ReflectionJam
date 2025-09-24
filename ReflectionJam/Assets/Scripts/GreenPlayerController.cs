using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GreenPlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnimator;

    private Vector2 moveDirection;
    private bool isAirborne;
    private bool hasDashed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private int maxJump = 1;
    private int jumpCount;

    public bool isActivePlayer = true;

    public InputAction playerMove;
    public InputAction playerAbilityOne;
    public InputAction playerAbilityTwo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = playerMove.ReadValue<Vector2>();
        
        if (moveDirection.magnitude > 1) moveDirection.Normalize(); // Stops diagonal movement from being faster

        // Changes movement animation based on speed and if on ground (Idle/Walk)
        playerAnimator.SetFloat("Speed", moveDirection.magnitude);
        playerAnimator.SetBool("IsGrounded", !isAirborne);

        if (isActivePlayer)
        {
            PlayerJump();
        }
    }

    void FixedUpdate()
    {
        if (isActivePlayer)
        {
            PlayerMove();
        }
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
        if (playerAbilityOne.triggered && jumpCount < maxJump)
        {
            playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCount++;
            isAirborne = true;

            // Triggers the jumping animation
            playerAnimator.SetTrigger("Jump");
            playerAnimator.SetBool("IsGrounded", false);

            Debug.Log("Jump Test");
        }
    }

    void PlayerDash() // Unused but saved here for possible future use in another project
    {
        if (playerAbilityTwo.triggered)
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

                    // Update animation
                    playerAnimator.SetBool("IsGrounded", true);
                    break;
                }
            }
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveDirection;
    }

    void OnEnable()
    {
        playerMove.Enable();
        playerAbilityOne.Enable();
        playerAbilityTwo.Enable();
    }

    void OnDisable()
    {
        playerMove.Disable();
        playerAbilityOne.Disable();
        playerAbilityTwo.Disable();
    }
}
