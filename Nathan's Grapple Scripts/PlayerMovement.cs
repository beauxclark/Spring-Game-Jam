using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float wallrunSpeed;
    public MovementState state;
    public float swingSpeed;
    public bool gameOver;
    public enum MovementState

    {
        freeze, 
        swinging,
        wallrunning,
    }
    
    private GameManager gameManagerScript;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public bool freeze;
    public bool floored;
    public bool wallrunning;
    public bool activeGrapple;
    public bool swinging;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public LayerMask whatIsFloor;
    public bool grounded;

    [Header("Camera Effects")]
    public PlayerCam cam;
    public float grappleFov = 95f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    
    private void StateHandler()
    {
        // Mode - Freeze 
          if(grounded)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
            gameOver = true;

        }                                                
        // Mode - Wallrunning
        if(wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }

        // Mode - Swinging
        else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        ResetJump();
    }
    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && floored)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
     }
        

     private void MovePlayer()
     {
        if(activeGrapple) return;
        if (swinging) return;
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // on Floor
        if(floored)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            // in air 
            else if(!floored)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

     }

        private void SpeedControl()
        {
            if(activeGrapple) return;
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //limit velocity if needed
            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        private void Jump() 
        {
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        //reset jump
        private void ResetJump() 
        {
            readyToJump = true;
        }
        private bool enableMovementOnNextTouch;

        public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
        {

            activeGrapple = true;
            //calculate the grappling velocity
            velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
            Invoke(nameof(SetVelocity), 0.1f);
            Invoke(nameof(ResetRestrictions), 2f);
        }

        private Vector3 velocityToSet;

        private void SetVelocity() 
        {
            enableMovementOnNextTouch = true;
            rb.velocity = velocityToSet;
            //zoom in the camera
            cam.DoFov(grappleFov);
        }

        public void ResetRestrictions()
        {
            activeGrapple = false;
            //undo camera effects
            cam.DoFov(85f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (enableMovementOnNextTouch)
            {
                enableMovementOnNextTouch = false;
                ResetRestrictions();

                GetComponent<Grappling>().StopGrapple();
            }
        }
    // Update is called once per frame
    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();
        // floor check 
        floored = Physics.Raycast(transform.position, Vector3.down, playerHeight * 1.2f + 1.3f, whatIsFloor);
        
    }
        void FixedUpdate()
        {
            MovePlayer();
        }

        public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
        {
            float gravity = Physics.gravity.y;
            float displacementY = endPoint.y - startPoint.y;
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
                + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
            
            return velocityXZ + velocityY;
        }
}
