using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class InteractionDoor : InteractionMsg, IInteractable
    {
        public bool isOpened;

        public Transform mainDoor;
        public Transform subDoor;

        public enum DoorType { 
            DefaultDoor,
            StartPointDoor,
            StageStartDoor,
        }
        [SerializeField] private DoorType currentDoor;

        public enum DoorOpenType
        { 
            Sliding,
            Rotating,
            Overhead
        }
        private DoorOpenType doorOpenType;

        private const float openSpeed = 1f;
        private float doorWidth;
        private GameObject moveKey;
        private ParticleSystem doorParticleSystem;

        private void Start()
        {
            moveKey = GameObject.Find("MoveKey");

            doorWidth = GetComponent<Collider>().bounds.size.x * 0.4f;

            doorParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(doorParticleSystem != null) doorParticleSystem.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.TryGetComponent(out PlayerCharacter player))
            {
                DoorTriggerEvent();
            }
        }

        IEnumerator MoveSlidingDoor(Transform door, float direction)
        {
            float time = 0f;
            Vector3 startPosition = door.localPosition;
            Vector3 endPosition = startPosition + new Vector3(direction * doorWidth, 0, 0);

            while (time < openSpeed)
            {
                door.localPosition = Vector3.Lerp(startPosition, endPosition, time / openSpeed);
                time += Time.deltaTime * 3f;
                yield return null;
            }

            door.localPosition = endPosition;

            if (currentDoor != DoorType.StartPointDoor) GetComponent<Collider>().enabled = !isOpened;
        }

        public void Interact()
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

            HandleDoorAction();
        }

        //��ȣ�ۿ� ������ ������Ʈ�� ��ġ ��ȯ -> ��ȣ�ۿ� UI ǥ�ø� ���� ���
        public Transform GetTransform()
        {
            return transform;
        }

        private void HandleDoorAction()
        {
            switch (currentDoor)
            {
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

        private void DoorTriggerEvent()
        {
            if (gameObject.name == "StartPointDoor")
            {
                if (moveKey.activeSelf) moveKey.SetActive(false);

                if (GameManager.Singleton.IsCleanComplete)
                {
                    //û�� �Ϸ� ������ ��� ���� Ŭ����
                    GameManager.Singleton.GameClear();
                }
            }
        }
    }
}
