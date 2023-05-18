using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private bool useCoyoteTime = true;
    [SerializeField] private bool useJumpBuffers = true;
    [SerializeField] private bool useApexModifiers = true;
    [SerializeField] private bool useLedgePush = true;
    [SerializeField] private bool useFreeFallVelocity = true;

    public Vector3 playerVelocity;
    public Vector3 playerAcceleration;
    public Vector2 playerDirection;
    
    public int jumpForce;
    public int maxJumpHeight;


    [Header("World Properties")]
    public int gravityModifier;
    public int frictionModifier;




    void FixedUpdate()
    {
        CalculateHorizontalVelocity();
        if (Physics.Raycast(transform.position, Vector3.down, 1.2f) && playerDirection.y > 0.01f)
        {
            CalculateJump();
        }
    }

    void Update()
    {
        CalculatePlayerInputs();
    }

    private void CalculateJump()
    {

        /*
        Coudnt get this to work in time so I will finish when I get the opportunity tomorrow 

        var currentPlayerSpeed = playerVelocity.y * Time.deltaTime;
        var currentPlayerAcceleration = (playerAcceleration.y * (1/2) * (Time.deltaTime * Time.deltaTime));

        var currentPlayerPosition = new Vector3(transform.position.x, transform.position.y + currentPlayerSpeed + currentPlayerAcceleration, transform.position.z);

        transform.position = currentPlayerPosition;

        transform.position = transform.position * (gravityModifier * Time.deltaTime);
        */

    }
      
    private void CalculateCoyoteTime()
    {

    }



    private void CalculateTerminalFreeFall()
    {

    }

    private void CalculateApexModifiers()
    {

    }

    private void CalculatePlayerInputs()
    {
        playerDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }



    #region Horizontal Movement
    private void CalculateHorizontalVelocity()
    {   
        

        //caluclates velocity
        var currentPlayerSpeed = playerDirection.x * playerVelocity.x * Time.deltaTime;
        var currentPlayerAcceleration = playerDirection.x * (playerAcceleration.x * (1/2) * (Time.deltaTime * Time.deltaTime));

        var currentPlayerPosition = new Vector3(transform.position.x + currentPlayerSpeed + currentPlayerAcceleration, transform.position.y, transform.position.z);

        transform.position = currentPlayerPosition;
    }

    #endregion
}
