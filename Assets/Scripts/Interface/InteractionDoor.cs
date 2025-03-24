using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

namespace KGY
{
    public class InteractionDoor : MonoBehaviour, IInteractable
    {
        public bool isAutoInteract;
        public bool isOpened;
        public bool isKeepOut;
        public bool isSlidingDoor;
        public string interactionMsg;
        public Transform mainDoor;
        public Transform subDoor;
        public enum DoorType { 
            defaultDoor,
            startPointDoor,
            stageStartDoor,
        }
        public DoorType currentDoor;

        private float openSpeed = 1f;
        private float doorWidth;
        private GameObject moveKey;
        private ParticleSystem doorParticleSystem;

        private void Start()
        {
            doorWidth = GetComponent<Collider>().bounds.size.x * 0.4f;
            moveKey = GameObject.Find("MoveKey");
            doorParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(doorParticleSystem != null) doorParticleSystem.gameObject.SetActive(false);
        }

        public bool IsAutoInteract => isAutoInteract;

        public bool IsOffLimit => isKeepOut;

        public string InteractionMsg => interactionMsg;

        public void Interact(CharacterBase character)
        {
            if (!isOpened || !isKeepOut) {
                StartCoroutine(MoveDoor("mainDoor"));
                if (subDoor != null) StartCoroutine(MoveDoor("subDoor"));
            }

            switch (currentDoor) {
                case DoorType.startPointDoor:
                    GetComponent<Collider>().isTrigger = true;
                    break;
                case DoorType.stageStartDoor:
                    doorParticleSystem.gameObject.SetActive(true);
                    GameManager.Singleton.IsGameStarted = true;
                    break;
                case DoorType.defaultDoor:
                    break;
            }
        }

        IEnumerator MoveDoor(string door) {
            float openDoorTime = 0f;
            if (isSlidingDoor)
            {
                if (door == "mainDoor")
                {
                    Vector3 doorInitPosition = mainDoor.localPosition;
                    Vector3 doorOpenPosition = new Vector3(doorInitPosition.x - doorWidth, doorInitPosition.y, doorInitPosition.z);

                    while (openDoorTime < openSpeed)
                    {
                        mainDoor.localPosition = Vector3.Lerp(doorInitPosition, doorOpenPosition, openDoorTime / openSpeed);
                        openDoorTime += Time.deltaTime * 3f;

                        yield return null;
                    }

                    mainDoor.localPosition = doorOpenPosition;
                    
                    isOpened = true;
                    if(currentDoor != DoorType.startPointDoor) GetComponent<Collider>().enabled = !isOpened;
                }
                else if (door == "subDoor")
                {
                    Vector3 doorInitPosition = subDoor.localPosition;
                    Vector3 doorOpenPosition = new Vector3(doorInitPosition.x + doorWidth, doorInitPosition.y, doorInitPosition.z);

                    while (openDoorTime < openSpeed)
                    {
                        subDoor.localPosition = Vector3.Lerp(doorInitPosition, doorOpenPosition, openDoorTime / openSpeed);
                        openDoorTime += Time.deltaTime * 3f;

                        yield return null;
                    }

                    subDoor.localPosition = doorOpenPosition;
                }
            }
            else {
                if (door == "mainDoor") {
                    Quaternion doorInitRoction = mainDoor.localRotation;
                    Vector3 doorOpenRoction = new Vector3(doorInitRoction.x, doorInitRoction.y + -90f, doorInitRoction.z);

                    while (openDoorTime < openSpeed) 
                    { 
                        mainDoor.localRotation = Quaternion.Lerp(doorInitRoction, Quaternion.Euler(doorOpenRoction), openDoorTime / openSpeed);
                        openDoorTime += Time.deltaTime * 3f;

                        yield return null;
                    }

                    mainDoor.localRotation = Quaternion.Euler(doorOpenRoction);

                    isOpened = true;
                    if (currentDoor != DoorType.startPointDoor) GetComponent<Collider>().enabled = !isOpened;
                }
                else
                {
                    Quaternion doorInitRoction = subDoor.localRotation;
                    Vector3 doorOpenRoction = new Vector3(doorInitRoction.x, doorInitRoction.y + 90f, doorInitRoction.z);

                    while (openDoorTime < openSpeed)
                    {
                        subDoor.localRotation = Quaternion.Lerp(doorInitRoction, Quaternion.Euler(doorOpenRoction), openDoorTime / openSpeed);
                        openDoorTime += Time.deltaTime * 3f;

                        yield return null;
                    }

                    subDoor.localRotation = Quaternion.Euler(doorOpenRoction);
                }
            }
        }

        //상호작용 가능한 오브젝트의 위치 반환 -> 상호작용 UI 표시를 위해 사용
        public Transform GetTransform()
        {
            return transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCharacter player)) moveKey.SetActive(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerCharacter player)) {
                //대화창 호출

                //콜라이더 삭제
                GetComponent<Collider>().enabled = !isOpened;
            }
        }
    }
}
