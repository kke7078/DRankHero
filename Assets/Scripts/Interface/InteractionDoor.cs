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

        public string Message => "���� ����";

        public bool IsAutoInteract => isAutoInteract;

        public string InteractionMsg => "�� ����";

        public void Interact(CharacterBase character)
        {
            if (!isOpened) {
                //�����̵� ������
                if (isSlidingDoor) {
                    //�������� ������ ��
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

                //�� �ݶ��̴� ����
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

        //��ȣ�ۿ� ������ ������Ʈ�� ��ġ ��ȯ -> ��ȣ�ۿ� UI ǥ�ø� ���� ���
        public Transform GetTransform()
        {
            return transform;
        }
    }
}
