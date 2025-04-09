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

        public bool isMinimapHide = false;

        private void Start()
        {
            standardShader = Shader.Find("Standard");
            
        }

        void OnWillRenderObject()
        {
            var renderer = GetComponent<Renderer>();
            var mat = renderer.material;

            if (Camera.current == minimapCamera) {
                mat.shader = minimapShader;

                if (isMinimapHide) mat.SetFloat("_IsMinimapHide", 1f);
                else mat.SetFloat("_IsMinimapHide", 0f);
            }
            else GetComponent<Renderer>().material.shader = standardShader;
        }
    }
}
