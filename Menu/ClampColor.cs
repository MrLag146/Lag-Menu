using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LagMenu.Menu
{
    public class ClampColor : MonoBehaviour
    {
        public void Start()
        {
            gameObjectRenderer = GetComponent<Renderer>();
            Update();
        }
        public void Update()
        {
            gameObjectRenderer.material.shader = targetRenderer.material.shader;
            gameObjectRenderer.material.renderQueue = targetRenderer.material.renderQueue;
            gameObjectRenderer.material.color = targetRenderer.material.color;


        }
        public Renderer gameObjectRenderer;
        public Renderer targetRenderer;
    }
}
