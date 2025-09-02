using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DootEmUp.Gameplay
{
    public class NoteController : MonoBehaviour
    {
        [SerializeField]
        private float duration;

        private Camera cam;

        private void Start()
        {
            Destroy(gameObject, duration);
            cam = GameObject.FindGameObjectWithTag("PlayerCam").GetComponent<Camera>();
        }

        private void Update()
        {
            transform.LookAt(cam.transform);
        }
    }
}