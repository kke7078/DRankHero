using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

namespace KGY
{
    public class InteractionDoor : MonoBehaviour, IInteractable
    {
        public bool isAutoInteract;
        public bool isOpened;
        public bool isSlidingDoor;
        public Transform mainDoor;
        public Transform subDoor;

        private float openSpeed = 1f;
        private float doorWidth;

        private void Start()
        {
            doorWidth = GetComponent<Collider>().bounds.size.x * 0.225f;
        }

        public string Message => "문을 열기";

        public bool IsAutoInteract => isAutoInteract;

        public string InteractionMsg => "문 열기";

        public void Interact(CharacterBase character)
        {
            if (!isOpened) {
                //슬라이딩 도어라면
                if (isSlidingDoor) {
                    //양쪽으로 열리는 문
                    if (subDoor != null) {
                        StartCoroutine(MoveDoor("mainDoor"));
                        StartCoroutine(MoveDoor("subDoor"));
                    }
                }
            }
        }

        IEnumerator MoveDoor(string door) {
            float openDoorTime = 0f;
            if (door == "mainDoor")
            {
                Vector3 doorInitPosition = mainDoor.localPosition;
                Vector3 doorOpenPosition = new Vector3(doorInitPosition.x + doorWidth, doorInitPosition.y, doorInitPosition.z);

                while (openDoorTime < openSpeed)
                {
                    mainDoor.localPosition = Vector3.Lerp(doorInitPosition, doorOpenPosition, openDoorTime / openSpeed);
                    openDoorTime += Time.deltaTime * 3f;

                    yield return null;
                }

                mainDoor.localPosition = doorOpenPosition;

                //문 콜라이더 제거
                isOpened = true;
                GetComponent<Collider>().enabled = !isOpened;
            }
            else if (door == "subDoor") {
                Vector3 doorInitPosition = subDoor.localPosition;
                Vector3 doorOpenPosition = new Vector3(doorInitPosition.x - doorWidth, doorInitPosition.y, doorInitPosition.z);

                while (openDoorTime < openSpeed)
                {
                    subDoor.localPosition = Vector3.Lerp(doorInitPosition, doorOpenPosition, openDoorTime / openSpeed);
                    openDoorTime += Time.deltaTime * 3f;

                    yield return null;
                }

                subDoor.localPosition = doorOpenPosition;
            }
        }

        //상호작용 가능한 오브젝트의 위치 반환 -> 상호작용 UI 표시를 위해 사용
        public Transform GetTransform()
        {
            return transform;
        }
    }
}
