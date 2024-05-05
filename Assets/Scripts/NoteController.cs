using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class NoteController : MonoBehaviour
    {
        [SerializeField]
        private float duration;

        private Camera cam;

        private void Start()
        {
            Destroy(gameObject, duration);
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            transform.LookAt(cam.transform);
        }
    }
}