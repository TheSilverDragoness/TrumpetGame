using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
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
        private bool canWallJump;
        private bool isOnGround;
        private bool isAgainstWall;

        private Rigidbody rb;

        private float inputX;
        private float inputY;
        Vector3 direction;

        private Vector3 wallNormal;

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
                canWallJump = true;
                isAgainstWall = false;
                wallNormal = Vector3.zero;
            }
            else
            {
                rb.drag = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (canJump && isOnGround)
                {
                    JumpHandler();
                }
                if (canWallJump && !isOnGround && isAgainstWall)
                {
                    WallJumphandler();
                }
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
            Debug.Log("Wall Jump Handler called");
            canJump = false;

            rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        }

        private void WallJumphandler()
        {
            Debug.Log("Wall Jump Handler called");
            canWallJump = false;
            isAgainstWall = false;

            rb.AddForce((wallNormal) * jumpForce, ForceMode.Force);
            rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Ground")
            {
                isOnGround = true;
            }
            if (collision.transform.tag == "Wall")
            {
                ContactPoint contact = collision.GetContact(0);
                wallNormal = contact.normal;
                isAgainstWall = true;
                Debug.Log("Collided with wall");
            }
        }
    }
}