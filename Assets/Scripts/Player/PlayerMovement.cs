using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
       [Header("Movement")]
    public float speed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraHolder; // Пустой объект, в который вложена Main Camera
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Vector2 moveInput;
    private bool jumpRequested;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Скрыть курсор
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Ввод движения
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // Прыжок
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }

        // Мышка — вращение
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Поворот вверх/вниз (камера)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Поворот влево/вправо (персонаж)
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        // Проверка на землю
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        // Движение
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.fixedDeltaTime);

        // Прыжок
        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpRequested = false;

        // Гравитация
        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
    }
    }
}
