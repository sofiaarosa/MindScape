using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimisticJump : MonoBehaviour, IAbility
{
    public string Name { get;} = "Salto do Otimismo";
    public string Description { get;} = "Alegria pula, evitando o primeiro bloqueio mental, poça de desinteresse ou espinho da culpa a frente.";
    public float CallCooldown { get;} = 5f;
    public bool CanCall { get; set;} = true;

    public float JumpHeight = 3f;
    public float JumpDistance = 5f;
    public const float NORMAL_JUMP_DISTANCE = 5f;
    public float LastCalled { get; set;}
    private PlayerMovement playerMovement;
    private CharacterController characterController;
    private Transform camTransform;
    private Animator animator;
    [SerializeField] private Vector3 velocity = Vector3.up;
    [SerializeField] private bool isJumping = false;

    public void Run(params object[] args)
    {
        if (characterController.isGrounded && !isJumping)
        {
            LastCalled = Time.time;
            Debug.Log("OptimisticJump: Starting Jump...");
            isJumping = true;
            animator.SetBool("isGrounded", false);
            animator.SetBool("isJumping", true);
            PlayerStatus.CanChange = false;

            // Disable player movement
            playerMovement.enabled = false;

            // Calculate initial jump velocity
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            // Calculate forward velocity based on desired distance
            Vector3 forward = transform.forward; // Get the forward direction of the character
            velocity.x = forward.x * JumpDistance / (Mathf.Sqrt(-2f * JumpHeight / Physics.gravity.y));
            velocity.z = forward.z * JumpDistance / (Mathf.Sqrt(-2f * JumpHeight / Physics.gravity.y));
        }
    }

    public void CooldownFinished(){
        CanCall = true;
        Debug.Log("OptimisticJump: Cooldown Finished!");
    }

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if(playerMovement != null){
            Debug.Log("OptimisticJump: PlayerMovement encontrado!");
            camTransform = playerMovement.cameraTransform;
        }

        characterController = GetComponent<CharacterController>();
        if(characterController != null){
            Debug.Log("OptimisticJump: CharacterController encontrado!");
        }

        animator = GetComponent<Animator>();
        if(animator == null){
            Transform armatureComponent = transform.Find("Armature");
            animator = armatureComponent.GetComponent<Animator>();
        }
    }

    void Update(){
        if (isJumping)
        {
            if(animator.GetBool("isFalling") == false){
                animator.SetBool("isFalling", true);
            }
            // Apply gravity
            velocity.y += Physics.gravity.y * Time.deltaTime;

            // Move character
            characterController.Move(velocity * Time.deltaTime);

            // Check if landed
            if (characterController.isGrounded && velocity.y <= 0)
            {
                Debug.Log("OptimisticJump: Landed!");
                isJumping = false;
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isGrounded", true);
                PlayerStatus.CanChange = true;
                playerMovement.enabled = true;
                JumpDistance = NORMAL_JUMP_DISTANCE;

                CanCall = false;
                Debug.Log("OptimisticJump: Starting Cooldown...");
                GameController.Instance.StartCooldown("OptimisticJumpCooldown", CallCooldown, CooldownFinished);
            }
        }
    }
}
