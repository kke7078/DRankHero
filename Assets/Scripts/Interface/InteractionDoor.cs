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
                    gameManager.EndGame(true);//û�� �Ϸ� ������ ��� ���� Ŭ����
                }
                else gameManager.GameHUD.HideMoveKeyUI(); //�ƴ� ��� �̵�Ű UI ����
            }
        }
    }
}
