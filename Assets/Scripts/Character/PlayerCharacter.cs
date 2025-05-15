using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace KGY
{
    //PlayerCharacter Ŭ���� : �÷��̾� ĳ������ �Ӽ� �� ������ �����ϴ� Ŭ���� (CharacterBase Ŭ������ ��ӹ޾Ƽ� Ȯ��)
    public class PlayerCharacter : CharacterBase
    {
        public static PlayerCharacter instance;

        //��ȣ�ۿ� ����
        public InteractionSensor InteractionSensor => interactionSensor;
        [SerializeField] private InteractionSensor interactionSensor;

        //���� ��ȣ�ۿ� ������ ������Ʈ ����Ʈ
        public List<IInteractable> CurrentInteractionItems => currentInteractionItems;
        private List<IInteractable> currentInteractionItems = new List<IInteractable>();

        //���� ���� ����� ��ȣ�ۿ� ������
        public IInteractable ClosestInteractable { get; private set; }

        [SerializeField] private Transform backToolHolder;         //� ��ġ�� û�ҵ��� Ȧ��
        [SerializeField] private Transform handToolHolder;         //�տ� ��ġ�� û�ҵ��� Ȧ��
        [SerializeField] private TwoBoneIKConstraint rightHandIK;  //������ IK
        [SerializeField] private TwoBoneIKConstraint leftHandIK;   //�޼� IK
        [SerializeField] private InteractionUI interactionUI;      //��ȣ�ۿ� UI

        private bool isCleaning = false;        //�÷��̾��� û�� ����
        private bool isEquipping = false;       //�÷��̾��� ��� ����
        private RigBuilder rigBuilder;          //�÷��̾��� RigBuilder ������Ʈ
        private CleanToolManager currentTool;   //���� ������ û�ҵ���
        
        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
            InputSystem.Singleton.onInteract += Interact;
        }

        private void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            rigBuilder = GetComponent<RigBuilder>();
            currentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //�ʱ� û�ҵ��� ����

            //��ȣ�ۿ� ���� ������Ʈ ����
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;

            //interactionUI.HideUI(); //�÷��̾��� ��ȣ�ۿ� UI ��Ȱ��ȭ
        }

        private void Update()
        {
            Direction = InputSystem.Singleton.MoveInput;    //�÷��̾��� �̵� ���� ����
            animator.SetFloat("isMove", Direction.magnitude);

            //�÷��̾��� ȸ�� ���� ����
            if (isCleaning)
            {
                //Ŭ���ϴ� �������� �÷��̾� ȸ��
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 1000f))
                {
                    Vector3 direction = hitInfo.point - transform.position;

                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    targetRot.eulerAngles = new Vector3(0, targetRot.eulerAngles.y, 0);
                    transform.rotation = targetRot;
                }
            }
        }

        private void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        //��ȣ�ۿ� ������ ���� ��ȣ�ۿ� ������Ʈ�� �����Ǿ��� �� ȣ��Ǵ� �޼���
        private void OnDetectedInteraction(IInteractable interactable)
        {
            if(interactable.InteractionInfos.Count == 0) interactable.Interact(this); //��ȣ�ۿ� ������ ���� ��� �ٷ� ��ȣ�ۿ� ����
            else currentInteractionItems.Add(interactable);
        }

        //��ȣ�ۿ� ������ ���� ��ȣ�ۿ� ������Ʈ�� ��ȣ�� ������� �� ȣ��Ǵ� �޼���
        private void OnLostSignalInteraction(IInteractable interactable)
        {
            currentInteractionItems.Remove(interactable);
        }

        //ĳ���Ϳ� ���� ����� ��ȣ�ۿ� ������Ʈ ã��
        private void FindClosestinteractable()
        {
            //���� ����� ��ȣ�ۿ� ������Ʈ ã��
            if (currentInteractionItems.Count > 0)
            {
                IInteractable closest = null;
                float closestDistance = float.MaxValue;

                foreach (IInteractable interactable in currentInteractionItems)
                {
                    float distance = Vector3.Distance(transform.position, interactable.GetTransform().position);
                    if (distance < closestDistance)
                    {
                        closest = interactable;
                        closestDistance = distance;
                    }
                }

                ClosestInteractable = closest;
            }
            else ClosestInteractable = null;
        }

        //�÷��̾��� û�� ������ ���� ��ȭ üũ
        private void Clean(bool isClean)
        {
            isCleaning = isClean;

            if (isClean)
            {
                SetSpeed(3.0f); //�÷��̾��� �̵��ӵ��� 3.0f�� ����
                Equip();
            }
            else
            {
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����
                UnEquip();

                //Hand IK �ʱ�ȭ
                HandIKControl();
            }
        }

        private void Equip() {
            if (isEquipping) return;

            isEquipping = true;

            //�÷��̾��� UnEquip �ִϸ��̼� �ʱ�ȭ
            animator.SetBool("isUnEquip", !isEquipping);
            animator.ResetTrigger("UnEquipTrigger");

            //�÷��̾��� Equip �ִϸ��̼� ����
            animator.SetBool("isEquip", isEquipping);
            animator.SetTrigger("EquipTrigger");
        }

        private void UnEquip() {
            isEquipping = false;

            //�÷��̾��� Equip �ִϸ��̼� �ʱ�ȭ
            animator.SetBool("isEquip", isEquipping);
            animator.ResetTrigger("EquipTrigger");

            //�÷��̾��� UnEquip �ִϸ��̼� ����
            animator.SetBool("isUnEquip", !isEquipping);
            animator.SetTrigger("UnEquipTrigger");
        }

        public void EquipControl(string status)
        {
            if (status == "equip")
            {
                //û�ҵ��� �տ� ����
                ToolEquip();
            }
            else if (status == "equipEnd")
            {
                //equip �ִϸ��̼� ����
                animator.SetBool("isEquip", false);

                //clean �ִϸ��̼� ����
                animator.SetFloat("isClean", 1);

                //Hand IK ����
                Invoke("HandIKControl", 0.03f);
            }
            else if (status == "unEquip")
            {
                //û�ҵ��� � ����
                ToolEquip();

                //Idle �ִϸ��̼� ����
                animator.SetFloat("isClean", 0);
            }
            else if (status == "unEquipEnd") {
                //unEquip �ִϸ��̼� ����
                animator.SetBool("isUnEquip", false);
            }
        }

        //û�� ���� ���� �� ����
        private void ToolEquip()
        {
            if (isCleaning)
            {
                //û�ҵ��� �տ� ����
                currentTool.transform.SetParent(handToolHolder);
                currentTool.transform.localRotation = Quaternion.identity;
                currentTool.transform.localPosition = Vector3.zero;
            }
            else
            {
                //û�ҵ����� � ����
                currentTool.transform.SetParent(backToolHolder);
                currentTool.transform.localPosition = currentTool.toolBackPosition;
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, currentTool.toolBackRotation.z);
            }
        }

        //Hand IK ����
        private void HandIKControl()
        {
            if (isCleaning)
            {
                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");  //������ IK Ÿ�� ����
                leftHandIK.data.target = currentTool.transform.Find("LeftHandGrip");    //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder�� ���̾� Ȱ��ȭ

                currentTool.toolMainEffect.SetActive(isCleaning); //û�ҵ��� ����Ʈ Ȱ��ȭ
                currentTool.toolSubEffext.SetActive(isCleaning); //û�ҵ��� ���� ����Ʈ Ȱ��ȭ
            }
            else
            {
                rightHandIK.data.target = null; //������ IK Ÿ�� �ʱ�ȭ
                leftHandIK.data.target = null;  //�޼� IK Ÿ�� �ʱ�ȭ
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder�� ���̾� ��Ȱ��ȭ

                currentTool.toolMainEffect.SetActive(isCleaning); //û�ҵ��� ����Ʈ ��Ȱ��ȭ
                currentTool.toolSubEffext.SetActive(isCleaning); //û�ҵ��� ���� ����Ʈ ��Ȱ��ȭ
            }

            rigBuilder.Build(); //RigBuilder �籸��
        }

        //ĳ���� �̵� �޼���
        public override void Move(Vector2 direction, float speed)
        {
            base.Move(direction, speed);

            FindClosestinteractable();
        }
        
        //�÷��̾��� ������ ���� ����
        public void SetPlayerMovementState(bool moving)
        {
            if (!moving)
            {
                Clean(moving);
                animator.SetFloat("isMove", 0);
            }
        }

        //�÷��̾� ��ȣ�ۿ� ���� �޼���
        private void Interact()
        {
            if (CurrentInteractionItems.Count <= 0) return;

            ClosestInteractable.Interact(this);
            CurrentInteractionItems.Remove(ClosestInteractable);

            //interactionUI.interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
