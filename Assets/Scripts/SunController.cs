using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] public float mouseSensitivity;
    [SerializeField] float jumpHeight = 100;
    [SerializeField] float gravity = 1;
    float velY = 0;
    const float maxCoyoteTimer = 1;
    float coyoteTimer = 0;
    

    [SerializeField] Transform cameraTransform;
    CharacterController controller;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        coyoteTimer = maxCoyoteTimer;
    }

    public void SunUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.Rotate(Vector3.left * mouseY);


        velY -= gravity * Time.deltaTime;
        controller.Move(Vector3.up * velY * Time.deltaTime);
        velY -= gravity * Time.deltaTime;

        if (controller.isGrounded)
        {
            coyoteTimer = maxCoyoteTimer;
            velY = 0;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (coyoteTimer > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                coyoteTimer = 0;
                velY = jumpHeight;
            }
        }
        


        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveVector = (transform.right * moveX) + (transform.forward * moveZ);
        controller.Move(moveVector.normalized * speed * Time.deltaTime);
    }
}
