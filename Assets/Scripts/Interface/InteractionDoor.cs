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
        public bool isSlidingDoor;
        public string interactionMsg;

        public Transform mainDoor;
        public Transform subDoor;

        public enum DoorType { 
            DefaultDoor,
            StartPointDoor,
            StageStartDoor,
        }
        public DoorType currentDoor;

        public enum DoorOpenType
        { 
            Sliding,
            Rotating,
            Overhead
        }
        public DoorOpenType doorOpenType;

        private float openSpeed = 1f;
        private float doorWidth;
        private GameObject moveKey;
        private ParticleSystem doorParticleSystem;

        public bool IsAutoInteract => isAutoInteract;
        public string InteractionMsg => interactionMsg;

        private void Start()
        {
            doorWidth = GetComponent<Collider>().bounds.size.x * 0.4f;
            moveKey = GameObject.Find("MoveKey");
            doorParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(doorParticleSystem != null) doorParticleSystem.gameObject.SetActive(false);
        }

        public void Interact(CharacterBase character)
        {
            if (!isOpened) {
                switch (doorOpenType)
                {
                    case DoorOpenType.Sliding:
                        if (mainDoor != null) StartCoroutine(MoveSlidingDoor(mainDoor, -1));
                        if (subDoor != null) StartCoroutine(MoveSlidingDoor(subDoor, 1));
                        break;
                }

                isOpened = !isOpened;
            }

            switch (currentDoor) {
                case DoorType.StartPointDoor:
                    GetComponent<Collider>().isTrigger = true;
                    break;
                case DoorType.StageStartDoor:
                    doorParticleSystem.gameObject.SetActive(true);
                    GameManager.Singleton.IsGameStarted = true;
                    break;
                case DoorType.DefaultDoor:
                    break;
            }
        }

        IEnumerator MoveSlidingDoor(Transform door, float direction)
        {
            float time = 0f;
            Vector3 startPosition = door.localPosition;
            Vector3 endPosition = startPosition + new Vector3(direction * doorWidth, 0, 0);

            while (time < openSpeed) { 
                door.localPosition = Vector3.Lerp(startPosition, endPosition, time / openSpeed);
                time += Time.deltaTime * 3f;
                yield return null;
            }

            door.localPosition = endPosition;

            isOpened = true;
            if (currentDoor != DoorType.StartPointDoor) GetComponent<Collider>().enabled = !isOpened;
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
                    if(currentDoor != DoorType.StartPointDoor) GetComponent<Collider>().enabled = !isOpened;
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
                    if (currentDoor != DoorType.StartPointDoor) GetComponent<Collider>().enabled = !isOpened;
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

            if (other.TryGetComponent(out PlayerCharacter player)) 
            {
                DoorTriggerEvent();
            }
        }

        private void DoorTriggerEvent()
        {
            switch (gameObject.name) 
            {
                case "StartPointDoor":
                    if(moveKey.activeSelf) moveKey.SetActive(false);
                    if (GameManager.Singleton.IsCleanComplete)
                    { 
                        //청소 완료 상태일 경우 레벨 클리어

                    }

                    break;
            }
        }
    }
}
