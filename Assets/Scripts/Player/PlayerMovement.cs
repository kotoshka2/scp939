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
        public Transform cameraHolder;
        [Range(10f, 500f)] public float mouseSensitivity = 100f;
        [Range(0f, 1f)] public float mouseSmoothTime = 0.05f;

        private float xRotation = 0f;
        private Vector2 currentMouseDelta;
        private Vector2 currentMouseDeltaVelocity;

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
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // Чтение ввода
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                jumpRequested = true;
            }

            // Сглаживание мыши
            Vector2 targetMouseDelta = new Vector2(
                Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y")
            );
            currentMouseDelta = Vector2.SmoothDamp(
                currentMouseDelta,
                targetMouseDelta,
                ref currentMouseDeltaVelocity,
                mouseSmoothTime
            );

            // Поворот камеры вверх/вниз
            xRotation -= currentMouseDelta.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Поворот персонажа влево/вправо
            transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity * Time.deltaTime);
        }

        void FixedUpdate()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            controller.Move(move * speed * Time.fixedDeltaTime);

            if (jumpRequested && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpRequested = false;

            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
}
