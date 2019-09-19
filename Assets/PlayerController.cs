using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private AnimationCurve JumpCurve;

    public float JumpMultiplier = 1;

    private CharacterController Controller;

    private bool isjumping;

    private float XAxisClamp;

    private void Awake() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        
    }


    void Update()
    {
        CameraRotation();
        Movement();
    }

    private void Movement() 
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 fwdMovement = transform.forward * vInput;
        Vector3 rMovement = transform.right * hInput;

        Controller.SimpleMove((fwdMovement + rMovement) * movementSpeed);

        JumpInput();
    }

    private void JumpInput() 
    {
        if (Input.GetAxis("Jump") == 1 && !isjumping)
        {
            isjumping = true;
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine() 
    {
        float timeInAir = 0f;
        do
        {
            float forceToApply = JumpCurve.Evaluate(timeInAir);
            Controller.Move(Vector3.up * forceToApply * JumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;

        } while (!Controller.isGrounded && Controller.collisionFlags != CollisionFlags.Above);

        isjumping = false;
    }

    private void CameraRotation() {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        XAxisClamp += mouseY;
        if (XAxisClamp > 90f)
        {
            XAxisClamp = 90f;
            mouseY = 0f;
            SetRotationOnXAxis(270f);
        }
        else if (XAxisClamp < -90f)
        {
            XAxisClamp = -90f;
            mouseY = 0f;
            SetRotationOnXAxis(90f);
        }

        playerCamera.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetRotationOnXAxis(float value) 
    {
        Vector3 eulerRotation = playerCamera.transform.eulerAngles;
        eulerRotation.x = value;
        playerCamera.transform.eulerAngles = eulerRotation;
    }
}
