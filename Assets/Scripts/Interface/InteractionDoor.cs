using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

namespace KGY
{
    public class InteractionDoor : MonoBehaviour, IInteractable
    {
        public bool IsAutoInteract { get; set; }
        public bool isAutoInteract;
        public bool isOpened;
        public bool isSlidingDoor;
        public bool isLeftDoor;

        private float doorWidth;

        private void Start()
        {
            doorWidth = GetComponent<Collider>().bounds.size.x;

            Debug.Log(doorWidth);
        }


        public string Message => "문을 열기";
        public void Interact(CharacterBase character)
        {
            if (!isOpened) {
                //슬라이딩 도어라면
                if (isSlidingDoor) {
                    //왼쪽 문이라면
                    //if (isLeftDoor) StartCoroutine(MoveDoor("leftDoor"));

                    //float newXPosition = Mathf.Lerp(transform.position.x, transform.position.x - doorWidth, Time.deltaTime * 2);
                    //transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
                }
            }
        }

        IEnumerator MoveDoor(string door) {
            if (door == "leftDoor") {
                float targetX = transform.position.x - doorWidth;


                yield return null;
            }
        }
    }
}
