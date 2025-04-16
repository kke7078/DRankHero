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
        public Transform backToolHolder;        //�÷��̾��� � ��ġ�� û�ҵ��� Ȧ��
        public Transform handToolHolder;        //�÷��̾��� �տ� ��ġ�� û�ҵ��� Ȧ��
        public TwoBoneIKConstraint rightHandIK; //�÷��̾��� ������ IK
        public TwoBoneIKConstraint leftHandIK;  //�÷��̾��� �޼� IK
        public InteractionUI interactionUI;

        protected bool isCleaning = false;      //�÷��̾��� û�� ����

        private bool isEquipping = false;           //�÷��̾��� ��� ����
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

            SetSpeed(5.0f); //�÷��̾��� �⺻�̵� �ӵ� ����

            interactionUI.HideUI(); //�÷��̾��� ��ȣ�ۿ� UI ��Ȱ��ȭ
        }

        protected void Update()
        {
            if (!isMoving) return;

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

            //Interaction UI ������Ʈ
            UpdateInteractionUI();
        }

        public void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        public void SetPlayerMovementState(bool moving) {
            isMoving = moving;

            if (!moving) {
                Clean(moving);
                animator.SetFloat("isMove", 0);
            }
        }

        //�÷��̾��� û�� ������ ���� ��ȭ üũ
        public void Clean(bool isClean)
        {
            if (!isMoving) return;

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

        public void Equip() {
            if (isEquipping) return;

            isEquipping = true;

            //�÷��̾��� UnEquip �ִϸ��̼� �ʱ�ȭ
            animator.SetBool("isUnEquip", !isEquipping);
            animator.ResetTrigger("UnEquipTrigger");

            //�÷��̾��� Equip �ִϸ��̼� ����
            animator.SetBool("isEquip", isEquipping);
            animator.SetTrigger("EquipTrigger");
        }

        public void UnEquip() {
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
        public void ToolEquip()
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
        public void HandIKControl()
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
                rightHandIK.data.target = null;       //������ IK Ÿ�� ����
                leftHandIK.data.target = null;        //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder�� ���̾� Ȱ��ȭ

                currentTool.toolMainEffect.SetActive(isCleaning); //û�ҵ��� ����Ʈ ��Ȱ��ȭ
                currentTool.toolSubEffext.SetActive(isCleaning); //û�ҵ��� ���� ����Ʈ ��Ȱ��ȭ
            }

            rigBuilder.Build(); //RigBuilder �籸��
        }

        public void Interact()
        {
            if (currentInteractionItems.Count <= 0) return;

            closestInteractable.Interact(this);
            currentInteractionItems.Remove(closestInteractable);

            interactionUI.interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        }

        //��ȣ�ۿ� UI ������Ʈ
        private void UpdateInteractionUI()
        {
            if (closestInteractable != null) interactionUI.ShowUI(closestInteractable);
            else interactionUI.HideUI();
        }
    }
}
