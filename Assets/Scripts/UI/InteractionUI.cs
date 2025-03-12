using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class InteractionUI : MonoBehaviour
    {
        public Transform targetObject;
        public GameObject interactionObj;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            interactionObj.SetActive(false);
        }
    }
}
