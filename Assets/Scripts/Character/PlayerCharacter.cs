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
        public CleanToolManager CurrentTool { get; private set; }   //���� ������ û�ҵ���

        [SerializeField] private Transform backToolHolder;         //� ��ġ�� û�ҵ��� Ȧ��
        [SerializeField] private Transform handToolHolder;         //�տ� ��ġ�� û�ҵ��� Ȧ��
        [SerializeField] private TwoBoneIKConstraint rightHandIK;  //������ IK
        [SerializeField] private TwoBoneIKConstraint leftHandIK;   //�޼� IK
        [SerializeField] private InteractionUI interactionUI;      //��ȣ�ۿ� UI

        private bool isCleaning = false;        //�÷��̾��� û�� ����
        private bool isEquipping = false;       //�÷��̾��� ��� ����
        private RigBuilder rigBuilder;          //�÷��̾��� RigBuilder ������Ʈ
                
        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
            InputSystem.Singleton.onInteract += Interact;
        }

        protected override void Start()
        {
            base.Start();

            rigBuilder = GetComponent<RigBuilder>();
            CurrentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //�ʱ� û�ҵ��� ����
        }

        private void Update()
        {
            if (GameManager.Singleton.IsInDialogue) return;

            Direction = InputSystem.Singleton.MoveInput;    //�÷��̾��� �̵� ���� ����
            animator.SetFloat("isMove", Direction.magnitude);

            Rotate();
        }

        private void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        //�÷��̾��� û�� ������ ���� ��ȭ üũ
        private void Clean(bool isClean)
        {
            if (GameManager.Singleton.IsInDialogue) return;

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
                CurrentTool.transform.SetParent(handToolHolder);
                CurrentTool.transform.localRotation = Quaternion.identity;
                CurrentTool.transform.localPosition = Vector3.zero;
            }
            else
            {
                //û�ҵ����� � ����
                CurrentTool.transform.SetParent(backToolHolder);
                CurrentTool.transform.localPosition = CurrentTool.ToolBackPosition;
                CurrentTool.transform.localRotation = Quaternion.Euler(CurrentTool.ToolBackRotation.x, CurrentTool.ToolBackRotation.y, CurrentTool.ToolBackRotation.z);
            }
        }

        //Hand IK ����
        private void HandIKControl()
        {
            if (isCleaning)
            {
                rightHandIK.data.target = CurrentTool.transform.Find("RightHandGrip");  //������ IK Ÿ�� ����
                leftHandIK.data.target = CurrentTool.transform.Find("LeftHandGrip");    //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder�� ���̾� Ȱ��ȭ

                CurrentTool.ToolMainEffect.SetActive(isCleaning); //û�ҵ��� ����Ʈ Ȱ��ȭ
                CurrentTool.ToolSubEffext.SetActive(isCleaning); //û�ҵ��� ���� ����Ʈ Ȱ��ȭ
            }
            else
            {
                rightHandIK.data.target = null; //������ IK Ÿ�� �ʱ�ȭ
                leftHandIK.data.target = null;  //�޼� IK Ÿ�� �ʱ�ȭ
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder�� ���̾� ��Ȱ��ȭ

                CurrentTool.ToolMainEffect.SetActive(isCleaning); //û�ҵ��� ����Ʈ ��Ȱ��ȭ
                CurrentTool.ToolSubEffext.SetActive(isCleaning); //û�ҵ��� ���� ����Ʈ ��Ȱ��ȭ
            }

            rigBuilder.Build(); //RigBuilder �籸��
        }

        //ĳ���� �̵� �޼���
        public override void Move(Vector2 direction, float speed)
        {
            if (GameManager.Singleton.IsInDialogue || GameManager.Singleton.IsPause) return;

            base.Move(direction, speed);

            InteractionManager.Singleton.FindClosestinteractable(); //��ȣ�ۿ� ������ ������Ʈ ã��
        }

        //ĳ���� ȸ�� �޼���
        private void Rotate()
        {
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
            var InteractMng = InteractionManager.Singleton;
            if (GameManager.Singleton.IsInDialogue || InteractMng.CurrentInteractable.Count <= 0) return;

            var targetObj = InteractMng.ClosestInteractable;
            if (targetObj == null) return;

            targetObj.Interact();
            InteractMng.InteractComplete(targetObj);
        }
    }
}
