using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject greenPlayer;
    [SerializeField] private GameObject redPlayer;
    [SerializeField] private CameraFollow cameraFollow;

    [SerializeField] private GameObject greenPlayerJumpUI;
    [SerializeField] private GameObject redPlayerAbsorbUI;

    public InputAction playerSwitchAction;

    private GameObject currentPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayer = greenPlayer;

        greenPlayer.GetComponent<GreenPlayerController>().isActivePlayer = true;
        greenPlayer.GetComponent<CloneController>().enabled = false;

        redPlayer.GetComponent<RedPlayerController>().isActivePlayer = false;
        redPlayer.GetComponent<CloneController>().enabled = true;
        
        UIGreenEnableRedDisable();

        cameraFollow.SetTarget(greenPlayer.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSwitchAction.triggered)
        {
            SwitchControl();
        }
    }

    void SwitchControl()
    {
        if (currentPlayer == greenPlayer)
        {
            greenPlayer.GetComponent<GreenPlayerController>().isActivePlayer = false;
            greenPlayer.GetComponent<CloneController>().enabled = true;

            redPlayer.GetComponent<RedPlayerController>().isActivePlayer = true;
            redPlayer.GetComponent<CloneController>().enabled = false;

            UIRedEnableGreenDisable();

            currentPlayer = redPlayer;
        } else
        {
            greenPlayer.GetComponent<GreenPlayerController>().isActivePlayer = true;
            greenPlayer.GetComponent<CloneController>().enabled = false;

            redPlayer.GetComponent<RedPlayerController>().isActivePlayer = false;
            redPlayer.GetComponent<CloneController>().enabled = true;

            UIGreenEnableRedDisable();

            currentPlayer = greenPlayer;
        }

        cameraFollow.SetTarget(currentPlayer.transform);

        // Sets the camera to the characters current facing position (removes awkward model snapping)
        Vector3 forward = currentPlayer.transform.forward;
        float yAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        cameraFollow.SetRotation(yAngle);
    }
    void UIGreenEnableRedDisable()
    {
        greenPlayerJumpUI.SetActive(true);
        redPlayerAbsorbUI.SetActive(false);
    }

    void UIRedEnableGreenDisable()
    {
        redPlayerAbsorbUI.SetActive(true);
        greenPlayerJumpUI.SetActive(false);
    }

    void OnEnable()
    {
        playerSwitchAction.Enable();
    }

    void OnDisable()
    {
        playerSwitchAction.Disable();
    }
}
