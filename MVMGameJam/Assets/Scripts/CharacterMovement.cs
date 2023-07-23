using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    CharacterController c_controller;
    Vector3 moveDirection = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        c_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (c_controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            moveDirection *= speed;

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        } 
        else
        {
            moveDirection.x = Input.GetAxis("Horizontal") * speed;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            float xScale = Mathf.Abs(transform.localScale.x);
            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.localScale = new Vector3(-xScale, transform.localScale.y, transform.localScale.z);
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        c_controller.Move(moveDirection * Time.deltaTime);
    }
}
