using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private LayerMask groundMask;
        [SerializeField]
        private float drag;
        [SerializeField]
        private float jumpForce;
        [SerializeField]
        private float jumpDelay;

        private bool canJump;
        private bool isOnGround;

        private Rigidbody rb;

        private float inputX;
        private float inputY;
        Vector3 direction;

        private void Start()
        {
            canJump = true;
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void Update()
        {
            isOnGround = Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);
            
            if (isOnGround)
            {
                rb.drag = drag;
                canJump = true;
            }
            else
            {
                rb.drag = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space) && canJump && isOnGround)
            {
                JumpHandler();
            }

            InputHandler();
        }

        private void FixedUpdate()
        {
            MovementHandler();
        }

        private void InputHandler()
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");
        }

        private void MovementHandler()
        {
            direction = transform.right * inputX + transform.forward * inputY;
            if (isOnGround)
            {
                rb.AddForce(direction.normalized * speed, ForceMode.Force);
            }
        }

        private void JumpHandler()
        {
            canJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);

            rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.transform.tag == "Ground")
            {
                isOnGround = true;
            }
        }
    }
}