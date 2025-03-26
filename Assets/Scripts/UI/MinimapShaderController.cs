using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    public class MinimapShaderController : MonoBehaviour
    {
        public Camera minimapCamera;
        public Shader minimapShader;
        private Shader standardShader;

        private void Start()
        {
            standardShader = Shader.Find("Standard");
        }

        void OnWillRenderObject()
        {
            if (Camera.current == minimapCamera) GetComponent<Renderer>().material.shader = minimapShader;
            else GetComponent<Renderer>().material.shader = standardShader;
        }
    }
}
