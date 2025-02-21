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

        private Animator animator;              //�÷��̾��� �ִϸ����� ������Ʈ
        private RigBuilder rigBuilder;          //�÷��̾��� RigBuilder ������Ʈ
        private CleanToolManager currentTool;   //���� ������ û�ҵ���
        

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
        }

        private void Start()
        {
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
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    targetRot.eulerAngles = new Vector3(0, targetRot.eulerAngles.y, 0);
                    transform.rotation = targetRot;
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

                animator.SetFloat("isClean", 1);    //û�ҵ��� ���� �ִϸ��̼� ����
                ToolEquip();                        //û�ҵ��� ����
            }
            else
            {
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����

                //û�ҵ��� ���� ���� �ִϸ��̼� ����
                animator.SetBool("isUnEquip", true);
                animator.SetTrigger("EquipTrigger");
                animator.SetFloat("isClean", 0);
            }

            HandIKControl(isClean);    //�� IK ����
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

        public void HandIKControl(bool isActive)
        {
            if (isActive)
            {
                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");  //������ IK Ÿ�� ����
                leftHandIK.data.target = currentTool.transform.Find("LeftHandGrip");    //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = true;  //RigBuilder�� ���̾� Ȱ��ȭ
            }
            else
            {
                rightHandIK.data.target = null;       //������ IK Ÿ�� ����
                leftHandIK.data.target = null;        //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = false;  //RigBuilder�� ���̾� Ȱ��ȭ
            }

            rigBuilder.Build(); //RigBuilder �籸��
        }
    }
}
