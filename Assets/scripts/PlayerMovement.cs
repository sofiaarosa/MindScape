using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // public float speed = 5;
    public float rotationSpeed = 780;
    public Transform cameraTransform;

    private Transform armatureComponent;
    private Animator animator;
    private CharacterController characterController;
    private float speed;
    private float animatorNormalSpeed;
    private float animatorSlowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();
        if(animator == null){
            armatureComponent = transform.Find("Armature");
            animator = armatureComponent.GetComponent<Animator>();
        }

        speed = PlayerStatus.Speed;
        animatorNormalSpeed = animator.speed;
        animatorSlowSpeed = animatorNormalSpeed/2;
    }

    // Update is called once per frame
    void Update()
    {
        speed = PlayerStatus.Speed;
        if(speed < PlayerStatus.MAX_SPEED){
            animator.speed = animatorSlowSpeed;
        }
        else{
            animator.speed = animatorNormalSpeed;
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // if(horizontalInput != 0 || verticalInput != 0){
        //     Debug.Log("Horizontal: " + horizontalInput + " Vertical: " + verticalInput);
        // }

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        // transform.Translate(movementDirection * magnitude * Time.deltaTime, Space.World);
        characterController.SimpleMove(movementDirection * magnitude);

        if(movementDirection != Vector3.zero){
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else{
            animator.SetBool("isMoving", false);
        }

    }
    private void OnApplicationFocus(bool focus){
        if(focus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
