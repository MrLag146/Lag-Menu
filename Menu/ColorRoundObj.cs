using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LagMenu.Menu
{

    public class ColorRoundObj : MonoBehaviour
    {
        public Renderer targetRenderer;
        private Renderer myRenderer;

        public void Start()
        {
            myRenderer = GetComponent<Renderer>();
            if (targetRenderer != null && myRenderer != null)
                myRenderer.material.color = targetRenderer.material.color;
        }

        public void Update()
        {
            if (targetRenderer != null && myRenderer != null)
                myRenderer.material.color = targetRenderer.material.color;
        }
    }
}
