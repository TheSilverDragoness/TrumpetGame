using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DootEmUp.UI
{
    public class marker : MonoBehaviour
    {
        public Sprite icon;
        public Image image;

        public Vector2 position
        {
            get { return new Vector2(transform.position.x, transform.position.z); }
        }
    }
}