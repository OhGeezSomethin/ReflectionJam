using UnityEngine;
using UnityEngine.InputSystem;

public class RedPlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnimator;

    private Vector2 moveDirection;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 10f;

    public bool isActivePlayer;

    public InputAction playerMove;
    public InputAction playerAbilityOne;

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

        if(moveDirection.magnitude > 1) moveDirection.Normalize(); // Stops diagonal movement from being faster

        playerAnimator.SetFloat("Speed", moveDirection.magnitude);
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
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 move = forward * moveDirection.y + right * moveDirection.x;
        playerRb.MovePosition(playerRb.position + move * speed * Time.fixedDeltaTime);

        // Player model rotates based on camera movement (jittery right now)
        Vector3 camViewDirection = forward;
        if (camViewDirection != Vector3.zero)
        {
            Quaternion tRotation = Quaternion.LookRotation(camViewDirection);
            playerRb.MoveRotation(Quaternion.Slerp(playerRb.rotation, tRotation, 10f * Time.fixedDeltaTime));
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveDirection;
    }

    void OnEnable()
    {
        playerMove.Enable();
    }

    void OnDisable()
    {
        playerMove.Disable();
    }
}
