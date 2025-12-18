using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private float PlayerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool isGrounded;

    [Header("SLope Handling")]
    [SerializeField] float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Jump & Air Control")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCoolDown;
    [SerializeField] private float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    private float startYScale;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    public Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        crouching,
        sprinting,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        // check Ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (isGrounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    void FixedUpdate()
    {
        movePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // when to jump 
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    void movePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)    
        {
            rb.AddForce(GetMoveSlopeDirection() * 20f * moveSpeed, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        // on ground
        if (isGrounded)
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!isGrounded)
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }    
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;   
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetMoveSlopeDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
