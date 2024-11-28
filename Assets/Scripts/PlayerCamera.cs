using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
        private float cameraSensitivity;

        private float yRot;
        private float xRot;

        [SerializeField]
        private Transform playerTrans;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetMouseSensitivity(float sensitivity)
        {
            cameraSensitivity = sensitivity;
        }

        void LateUpdate()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * cameraSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * cameraSensitivity;

            yRot += mouseX;
            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, -90, 90);

            transform.rotation = Quaternion.Euler(xRot, yRot, 0);
            playerTrans.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }
}