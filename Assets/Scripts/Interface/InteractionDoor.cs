using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class InteractionDoor : MonoBehaviour, IInteractable, IHasInteractionIds
    {
        public bool IsOpened {
            get => isOpened;
            set => isOpened = value;
        }
        [SerializeField] private bool isOpened;

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

        private ParticleSystem doorParticleSystem;
        private Animator animator;

        public List<InteractionData.MsgId> InteractionIdList => interactionIdList;
        [SerializeField] private List<InteractionData.MsgId> interactionIdList = new List<InteractionData.MsgId>();


        private void Start()
        {
            doorParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(doorParticleSystem != null) doorParticleSystem.gameObject.SetActive(false);

            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.TryGetComponent(out PlayerCharacter player))
            {
                DoorTriggerEvent();
            }
        }

        public void Interact()
        {
            if (!IsOpened) 
            {
                switch (doorOpenType)
                {
                    case DoorOpenType.Sliding:
                        animator.SetBool("isOpen", true);
                        animator.SetTrigger("slidingOpenTrigger");

                        animator.SetBool("isClosed", false);
                        animator.ResetTrigger("slidingCloseTrigger");
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
                    GetComponent<Collider>().enabled = !IsOpened;
                    break;
                case DoorType.DefaultDoor:
                    GetComponent<Collider>().enabled = !IsOpened;
                    break;
            }
        }

        private void DoorTriggerEvent()
        {
            if(currentDoor == DoorType.StartPointDoor)
            {
                GameManager gameManager = GameManager.Singleton;
                if (gameManager.IsCleanComplete)
                {
                    gameManager.EndGame(true);//청소 완료 상태일 경우 레벨 클리어
                }
                else gameManager.GameHUD.HideMoveKeyUI(); //아닐 경우 이동키 UI 숨김
            }
        }
    }
}
