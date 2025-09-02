using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DootEmUp.Gameplay.Enemy;

namespace DootEmUp.Gameplay.Player
{
    public class TrumpetController : MonoBehaviour
    {
        [SerializeField]
        private GameObject valve1;
        [SerializeField]
        private GameObject valve2;
        [SerializeField]
        private GameObject valve3;

        [SerializeField]
        private float valveOffset;

        [SerializeField]
        private List<AudioClip> clipList;

        [SerializeField]
        private float breath;
        [SerializeField]
        private float breathRefillRate;
        [SerializeField]
        private float breathEmptyRate;

        [SerializeField]
        private float dootForce;
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private Transform[] popupTransforms;

        [SerializeField]
        private GameObject[] noteSprites;

        private AudioSource audioSource;

        private float curBreath;
        private List<Valve> valves = new List<Valve>();
        private StringBuilder noteStr = new StringBuilder();
        private int noteNum;

        [SerializeField]
        private LayerMask dootLayerMask;

        private void Start()
        {
            valves.Add(new Valve(valve1, false, valve1.transform.localPosition));
            valves.Add(new Valve(valve2, false, valve2.transform.localPosition));
            valves.Add(new Valve(valve3, false, valve3.transform.localPosition));
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            HandleInput();
            HandleBreath();
        }

        private void HandleInput()
        {
            for (int i = 0; i < valves.Count; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    ValveDown(valves[i]);
                }
                else if (Input.GetMouseButtonUp(i))
                {
                    ValveUp(valves[i]);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && breath > 0)
            {
                Doot();
            }
        }

        private void Doot()
        {
            GetNoteString();
            BinaryToInt(noteStr.ToString());
            curBreath -= breathEmptyRate * Time.deltaTime;
            curBreath = Mathf.Clamp(curBreath, 0, breath);
            audioSource.clip = clipList[noteNum];
            audioSource.Play();

            ShowNoteSprite();

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, dootLayerMask))
            {
                hit.transform.root.GetComponent<EnemyController>().Die(noteStr.ToString());
            }
        }

        private void ShowNoteSprite()
        {
            int i = UnityEngine.Random.Range(0, 3);

            Instantiate(noteSprites[noteNum], popupTransforms[i].position, Quaternion.identity, transform);
        }

        private void HandleBreath()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                curBreath -= breathEmptyRate * Time.deltaTime;
                curBreath = Mathf.Clamp(curBreath, 0, breath);
            }
            else if (!Input.GetKeyDown(KeyCode.Space))
            {
                curBreath += breathRefillRate * Time.deltaTime;
                curBreath = Mathf.Clamp(curBreath, 0, breath);
            }
        }

        private void ValveDown(Valve curValve)
        {
            curValve.valve.transform.position -= new Vector3(0, valveOffset, 0);
            curValve.isPressed = true;
        }

        private void ValveUp(Valve curValve)
        {
            curValve.valve.transform.localPosition = curValve.valveOrigin;
            curValve.isPressed = false;
        }

        private void GetNoteString()
        {
            noteStr.Clear();
            foreach (var curValve in valves)
            {
                noteStr.Append(curValve.isPressed ? '1' : '0');
            }
        }

        private void BinaryToInt(string str)
        {
            noteNum = Convert.ToInt32(str, 2);
        }
    }

    public class Valve
    {
        public GameObject valve;
        public bool isPressed;
        public Vector3 valveOrigin;

        public Valve(GameObject obj, bool pressed, Vector3 origin)
        {
            valve = obj;
            isPressed = pressed;
            valveOrigin = origin;
        }
    }
}

