using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementWOPhysics : MonoBehaviour
{
      
    [Header("Player Properties")]
    [SerializeField] private bool useCoyoteTime = true;
    [SerializeField] private bool useJumpBuffers = true;
    [SerializeField] private bool useApexModifiers = true;
    [SerializeField] private bool useLedgePush = true;
    [SerializeField] private bool useFreeFallVelocity = true;
    private PlayerInput playerInput;
    private Vector3 playerTotalVelocity;
    private Vector3 lastPosition;
    public Vector3 rawMovement {get; private set;}


    [Header("Player Speed Properties")]

    public Vector3 playerVelocity;
    public Vector3 playerAcceleration;
    public Vector2 playerDirection;
    [SerializeField] private float currentVerticalSpeed;
    [SerializeField] private float currentHorizontalSpeed;
    [SerializeField] private float currentVerticalAccel;
    [SerializeField] private float currentHorizontalAccel;
    [SerializeField] private Vector2 terminalVelocity;
    [SerializeField] private float moveClamp = 13f;
    [SerializeField] private float deAcceleration = 60f;
    [SerializeField] private float apexBonus = 2f;

    [Header("Collision Properties")]
    public Bounds characterBounds;
    private bool collisionUp;
    private bool collisionLeft;
    private bool collisionRight;
    private bool collisionDown;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int detectorCount = 3;
    [SerializeField] private float detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    private float timeLeftGrounded;


    
    [Header("Jump Properties")] 
    [SerializeField] private float jumpHeight = 30;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float coyoteTimeThreshold = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private float jumpEndEarlyGravityModifier = 3;
    private bool coyoteUsable;
    private bool endedJumpEarly = true;
    private float apexPoint; // Becomes 1 at the apex of a jump
    private float lastJumpPressed;
    [SerializeField] private bool canUseCoyote => useCoyoteTime && coyoteUsable && !collisionDown && timeLeftGrounded + coyoteTimeThreshold > Time.time;
    private bool hasBufferedJump => collisionDown && lastJumpPressed + jumpBuffer > Time.time;
    private bool jumpingThisFrame;



    [Header("Ground Properties")]
    public int frictionModifier;


    [Header("World Properties")]
    public int gravityModifier;

    [Header("Gravity")]
    [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 80f;
    [SerializeField] private float maxFallSpeed = 120f;
    public float currentFallSpeed;

    public struct PlayerInput
    {
        public bool jumpUp;
        public bool jumpDown;
        public float horizontalMovement;
    }
    




    void FixedUpdate()
    {

    }

    void Update()
    { 
        playerTotalVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;


        CalculatePlayerInputs();
        CalculateCollisions();

        CalculateHorizontalMovement();
        CalculateJumpApex();
        CalculateGravity();
        CalculateJump();
        MoveCharacter();
    }



    private void CalculatePlayerInputs()
    {
        //playerDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerInput = new PlayerInput
        {
            jumpDown = Input.GetButtonDown("Jump"),
            jumpUp = Input.GetButtonUp("Jump"),
            horizontalMovement = Input.GetAxisRaw("Horizontal")
        };
        
        if (playerInput.jumpDown)
        {
            lastJumpPressed = Time.time;
            Debug.Log("Collision Down: " + collisionDown);
            Debug.Log("Can uSe Coyote: " + canUseCoyote);
        }
    }

    RaycastHit hit;
    #region Collision
    private void CalculateCollisions()
    {
        for (int i = 0; i < freeColliderIterations; i++)
        {
            collisionUp = Physics.BoxCast(transform.position + Vector3.up, transform.localScale, transform.forward, out hit, transform.rotation, detectionRayLength, groundLayer );
            collisionDown = Physics.BoxCast(transform.position + new Vector3(-1, -1, 0), transform.localScale, transform.forward, out hit, transform.rotation, detectionRayLength, groundLayer );
            collisionLeft = Physics.BoxCast(transform.position + Vector3.left, transform.localScale, transform.forward, out hit, transform.rotation, detectionRayLength, groundLayer );
            collisionRight = Physics.BoxCast(transform.position + Vector3.right, transform.localScale, transform.forward, out hit, transform.rotation, detectionRayLength, groundLayer );
        }

        Debug.Log(collisionDown);
        Debug.Log(collisionUp);
        Debug.Log(collisionLeft);
        Debug.Log(collisionRight);
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one * detectionRayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(-0.5f, -1, 0), Vector3.one * detectionRayLength);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.left, Vector3.one * detectionRayLength);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.right, Vector3.one * detectionRayLength);
    }


    #endregion
    



    #region Horizontal Movement
    private void CalculateHorizontalMovement()
    {   
        if (playerInput.horizontalMovement != 0) 
        {
            // Set horizontal move speed
            currentHorizontalSpeed += playerInput.horizontalMovement * playerAcceleration.x * Time.deltaTime;

            // clamped by max frame movement
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

            // Apply bonus at the apex of a jump
            var _apexBonus = Mathf.Sign(playerInput.horizontalMovement) * apexBonus * apexPoint;
            currentHorizontalSpeed += _apexBonus * Time.deltaTime;
        }
        else 
        {
            // No input. Let's slow the character down
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, deAcceleration * Time.deltaTime);
        }

        if (currentHorizontalSpeed > 0 && collisionRight || currentHorizontalSpeed < 0 && collisionLeft) 
        {
            // Don't walk through walls
            currentHorizontalSpeed = 0;
        }
    }

     private int freeColliderIterations = 10;
    private void MoveCharacter()
    {
        var pos = transform.position + characterBounds.center;
        rawMovement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed); // Used externally
        var move = rawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks  
        var hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
        if (!hit) {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < freeColliderIterations; i++) 
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, characterBounds.size, 0, groundLayer)) 
            {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                /*
                if (i == 1) 
                {
                    if (currentVerticalSpeed < 0) currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    transform.position += dir.normalized * move.magnitude;
                }
                */

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion

    #region Vertical Movement

    private void CalculateGravity()
    {
        if (collisionDown)
        {
            if (currentVerticalSpeed < 0)
            {
                currentVerticalSpeed = 0;
            }
        }
        else
        {
            var fallSpeed = endedJumpEarly && currentVerticalSpeed > 0 ? currentFallSpeed * jumpEndEarlyGravityModifier : currentFallSpeed;
              
            // Fall
            currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            // Clamp
            if (currentVerticalSpeed < terminalVelocity.y)
            {
                currentVerticalSpeed = terminalVelocity.y;
            } 
            
        }
    }


        private void CalculateJumpApex() {
            if (!collisionDown) {
                // Gets stronger the closer to the top of the jump
                apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(playerTotalVelocity.y));
                currentFallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
            }
            else {
                apexPoint = 0;
            }
        }

        private void CalculateJump() {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (playerInput.jumpDown) {
                currentVerticalSpeed = jumpHeight;
                endedJumpEarly = false;
                coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                jumpingThisFrame = true;
            }
            else 
            {
                jumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!collisionDown && playerInput.jumpUp && !endedJumpEarly && playerTotalVelocity.y > 0) {
                // _currentVerticalSpeed = 0;
                endedJumpEarly = true;
            }

            if (collisionUp) {
                if (currentVerticalSpeed > 0)
                {
                    currentVerticalSpeed = 0;
                } 
            }
        }
        

    #endregion
    
}
