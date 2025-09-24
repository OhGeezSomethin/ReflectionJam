using UnityEngine;

public class CloneController : MonoBehaviour
{
    private Rigidbody currentRb;
    [SerializeField] private Transform other;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GreenPlayerController greenPlayerController;
    [SerializeField] private RedPlayerController redPlayerController;
    [SerializeField] private float speed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MirrorOther();
    }

    void MirrorOther()
    {
        // Camera relative movement
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector2 moveInput = Vector2.zero;

        if (greenPlayerController.isActivePlayer)
        {
            // Get the green players movement input
            moveInput = greenPlayerController.GetMoveInput();
        } else if (redPlayerController.isActivePlayer)
        {
            // Get the red players movement input
            moveInput = redPlayerController.GetMoveInput();
        }

            // Mirror input
            Vector3 mirrorMove = forward * -moveInput.y + right * -moveInput.x;
        currentRb.MovePosition(currentRb.position + mirrorMove * speed * Time.fixedDeltaTime);


        Vector3 mirrorForward = new Vector3(-other.forward.x, 0, -other.forward.z).normalized;
        if(mirrorForward != Vector3.zero)
        {
            Quaternion tRotation = Quaternion.LookRotation(mirrorForward);
            currentRb.MoveRotation(Quaternion.Slerp(currentRb.rotation,tRotation, 10f * Time.fixedDeltaTime));
        }
    }
}
