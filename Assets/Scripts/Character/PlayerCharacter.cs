using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace KGY
{
    //PlayerCharacter Ŭ���� : �÷��̾� ĳ������ �Ӽ� �� ������ �����ϴ� Ŭ���� (CharacterBase Ŭ������ ��ӹ޾Ƽ� Ȯ��)
    public class PlayerCharacter : CharacterBase
    {

        public Transform backToolHolder;        //�÷��̾��� � ��ġ�� û�ҵ��� Ȧ��
        public Transform handToolHolder;        //�÷��̾��� �տ� ��ġ�� û�ҵ��� Ȧ��
        public TwoBoneIKConstraint rightHandIK; //�÷��̾��� ������ IK
        public TwoBoneIKConstraint leftHandIK;  //�÷��̾��� �޼� IK

        protected bool isCleaning = false;      //�÷��̾��� û�� ����
        protected bool isEquipping = false;     //�÷��̾��� ���� ���� 

        private Animator animator;              //�÷��̾��� �ִϸ����� ������Ʈ
        private RigBuilder rigBuilder;          //�÷��̾��� RigBuilder ������Ʈ
        private CleanToolManager currentTool;   //���� ������ û�ҵ���

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
            InputSystem.Singleton.onInteract += Interact;
        }

        protected override void Start()
        {
            base.Start();

            animator = GetComponent<Animator>();
            rigBuilder = GetComponent<RigBuilder>();
            currentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //�ʱ� û�ҵ��� ����

            SetSpeed(5.0f); //�÷��̾��� �⺻�̵� �ӵ� ����
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

                    //ĳ���Ϳ� ���콺 Ŭ�� ��ġ�� �Ÿ��� 1f �̻��� �� ȸ��
                    if (direction.magnitude > 1f) {
                        Quaternion targetRot = Quaternion.LookRotation(direction);
                        targetRot.eulerAngles = new Vector3(0, targetRot.eulerAngles.y, 0);
                        transform.rotation = targetRot;
                    }
                }
            }
        }

        public void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        //�÷��̾��� û�� ������ ���� ��ȭ üũ
        private void Clean(bool isClean)
        {
            isCleaning = isClean;

            if (isClean)
            {
                SetSpeed(3.0f); //�÷��̾��� �̵��ӵ��� 3.0f�� ����

                //�÷��̾��� Equip �ִϸ��̼� ����
                animator.SetBool("isEquip", isClean);
                animator.SetTrigger("EquipTrigger");

            }
            else
            {
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����

                //�÷��̾��� Equip �ִϸ��̼� �ʱ�ȭ
                animator.SetBool("isEquip", isClean);

                //�÷��̾��� unEquip �ִϸ��̼� ����
                animator.SetBool("isUnEquip", !isClean);
                animator.SetTrigger("EquipTrigger");

                //Hand IK �ʱ�ȭ
                HandIKControl();
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
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, 0);
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

        //public override void Interact()
        //{
            
        //    Debug.Log("Player Character Interact");
        //}
    }
}
