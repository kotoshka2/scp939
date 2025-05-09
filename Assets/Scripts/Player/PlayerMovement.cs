using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;


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

        [Header("Audio")] 
        private AudioSource[] AudioSources;
        private AudioSource stepMaker;
        private AudioSource voiceMaker;
        [SerializeField] private List<AudioClip> concreteSteps;
        [SerializeField] private List<AudioClip> grassSteps;
        private List<AudioClip> usedSteps;
        [SerializeField] private float stepInterval = 0.5f;
        private float stepTimer = 0f;
        bool wasGrounded;
        
        [Header("Microphone")]
        public string microphoneDevice;
        public int recordingLength = 10; // в секундах
        private AudioClip recordedClip;
        private bool isRecording = false;
        private float timer;
 

        void Start()
        {
            controller = GetComponent<CharacterController>();
            AudioSources = GetComponents<AudioSource>();
            stepMaker = AudioSources[0];
            voiceMaker = AudioSources[1];
            Cursor.lockState = CursorLockMode.Locked;
            usedSteps = concreteSteps;
            if (Microphone.devices.Length > 0)
            {
                microphoneDevice = Microphone.devices[0];
            }
            else
            {
                Debug.LogWarning("Микрофон не найден.");
            }

        }

        void Update()
        {
            
            stepTimer -= Time.deltaTime;
            // Чтение ввода
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                jumpRequested = true;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                timer = Time.time + recordingLength;
                StartRecording();
            }

            if (Time.time > timer && isRecording)
            {
                StopRecording();
            }

            if (Input.GetKeyDown(KeyCode.V) && !isRecording)
            {
                PlayRecordedAudio();
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

            
           
            ChooseTheSound();
            MoveCharacter();
            if (!wasGrounded && isGrounded)
            {
                PlayRandomFootstep();
            }
            wasGrounded = isGrounded;
            if (jumpRequested && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                MakeSound(SoundType.Loud, transform.position);
            }
            jumpRequested = false;

            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }

        private void MoveCharacter()
        {   
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (move != Vector3.zero)
                {
                    MakeSound(SoundType.Quiet, transform.position);
                    if (stepTimer <= 0)
                    {
                        PlayRandomFootstep();
                        stepTimer = stepInterval;
                    }
                }
                controller.Move(move * speed /2 * Time.fixedDeltaTime);
                
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (move != Vector3.zero)
                    {
                        MakeSound(SoundType.Loud, transform.position);
                        if (stepTimer <= 0)
                        {
                            PlayRandomFootstep();
                            stepTimer = stepInterval;
                        }
                    }
                    controller.Move(move * speed * 1.5f * Time.fixedDeltaTime);
                }
                else
                {
                    if (move != Vector3.zero)
                    {
                        MakeSound(SoundType.Normal, transform.position);
                        if (stepTimer <= 0)
                        {
                            PlayRandomFootstep();
                            stepTimer = stepInterval;
                        }
                    }
                    controller.Move(move * speed * Time.fixedDeltaTime);
                }
            }
            
        }

        void MakeSound(SoundType type, Vector3 position)
        {
            GameManager.SendPlayerMadeSound(type,position);
        }

        private SurfaceType CheckSurface()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f)) {
                string surfaceTag = hit.collider.tag;

                switch (surfaceTag) {
                    case "Grass":
                        return SurfaceType.Grass;
                        break;
                    case "Concrete":
                        return SurfaceType.Concrete;
                    default:
                        return SurfaceType.Concrete;
                }
            }

            return SurfaceType.Concrete;
        }

        private void ChooseTheSound()
        {
            switch (CheckSurface())
            {
                case SurfaceType.Grass:
                    usedSteps = grassSteps;
                    break;
                case SurfaceType.Concrete:
                    usedSteps = concreteSteps;
                    break;
                default:
                    usedSteps = concreteSteps;
                    break;
            }
            
        }
        private void PlayRandomFootstep()
        {
            if (usedSteps.Count == 0 || stepMaker == null || !isGrounded)
                return;
            float randPitch = Random.Range(0.8f, 1.2f);
            stepMaker.pitch = randPitch;
            int index = Random.Range(0, usedSteps.Count);
            stepMaker.PlayOneShot(usedSteps[index]);
        }
        public void StartRecording()
        {
            if (microphoneDevice == null || isRecording) return;

            Debug.Log("Начало записи...");
            recordedClip = Microphone.Start(microphoneDevice, false, recordingLength, 44100);
            isRecording = true;
            
        }

        public void StopRecording()
        {
            if (!isRecording) return;

            Microphone.End(microphoneDevice);
            isRecording = false;
            Debug.Log("Запись остановлена.");
        }

        public void PlayRecordedAudio()
        {
            if (recordedClip == null) return;

            voiceMaker.clip = recordedClip;
            voiceMaker.Play();
            Debug.Log("Воспроизведение записи...");
        }
    }
}
