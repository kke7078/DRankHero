using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class InteractionDoor : MonoBehaviour, IInteractable
    {
        public bool IsOpened { get; set; }

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
        [SerializeField] private DoorOpenType doorOpenType;

        [SerializeField] private InteractionData interactinData;//상호작용 데이터
        [SerializeField] private List<string> InteractionIds;   //상호작용 ID 리스트
        [SerializeField] private Transform mainDoor;
        [SerializeField] private Transform subDoor;
        
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

            if (currentDoor != DoorType.StartPointDoor) GetComponent<Collider>().enabled = !IsOpened;
        }

        public void Interact()
        {
            if (!IsOpened) {
                switch (doorOpenType)
                {
                    case DoorOpenType.Sliding:
                        if (mainDoor != null) StartCoroutine(MoveSlidingDoor(mainDoor, -1));
                        if (subDoor != null) StartCoroutine(MoveSlidingDoor(subDoor, 1));
                        break;
                }

                IsOpened = !IsOpened;
            }

            HandleDoorAction();
        }

        //상호작용 가능한 오브젝트의 위치 반환 -> 상호작용 UI 표시를 위해 사용
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
                    //청소 완료 상태일 경우 레벨 클리어
                    GameManager.Singleton.GameClear();
                }
            }
        }
    }
}
