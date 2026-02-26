using UnityEngine;

namespace BirthdayGame.FinalCharacterController
{
    public class PlayerController:MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Animator animator;

        [Header("Base Movement")]
        public float runAcceleration = 50f;
        public float runSpeed = 4f;
        public float drag = 10f;
        public float gravity = 25f;
        public float jumpSpeed = 1.0f;
        
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;
        public TypewriterText typewriterText;
        public float pickupRadius = 1.5f;
        
        private PlayerLocomotionInput playerLocomotionInput;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerTargetRotation = Vector2.zero;

        private Vector3 currentVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        private void Update()
        {
            CheckForCollectibles();
            UpdateMovement();
            UpdateJump();
            UpdateAnimator();
        }

        private void CheckForCollectibles()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);
            foreach (Collider col in hits)
            {
                CollectibleItem item = col.GetComponent<CollectibleItem>();
                if (item != null)
                {
                    Debug.Log("Picked up: " + item.gameObject.name);
                    typewriterText.ShowText(item.text);
                    Destroy(item.gameObject);
                    break;
                }
            }
        }

        private void UpdateMovement()
        {
            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraForwardXZ * playerLocomotionInput.MovementInput.y + cameraRightXZ * playerLocomotionInput.MovementInput.x;

            if (movementDirection.magnitude > 0f)
            {
                currentVelocity += movementDirection.normalized * runAcceleration * Time.deltaTime;
                currentVelocity = Vector3.ClampMagnitude(currentVelocity, runSpeed);
            }
            else
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, drag * Time.deltaTime);
            }

            Vector3 horizontalMovement = currentVelocity * Time.deltaTime;
            Vector3 verticalMovement = new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);
            characterController.Move(horizontalMovement + verticalMovement);
        }

        private void UpdateJump()
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = -1f;

                if (playerLocomotionInput.JumpPressed)
                {
                    verticalVelocity = jumpSpeed;
                    if (animator != null)
                        animator.SetTrigger("Jump");
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            float speed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude / runSpeed;
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }

        private void LateUpdate()
        {
            cameraRotation.x += lookSenseH * playerLocomotionInput.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);
        
            playerTargetRotation.x += lookSenseH * playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);

            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
        }

    }
}