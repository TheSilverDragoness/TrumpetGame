using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpetController : MonoBehaviour
{
    private GameObject valve1;
    private GameObject valve2;
    private GameObject valve3;

    [SerializeField]
    private float minValvePos;
    [SerializeField]
    private float maxValvePos;
    [SerializeField]
    private float dootCoolDown;
}