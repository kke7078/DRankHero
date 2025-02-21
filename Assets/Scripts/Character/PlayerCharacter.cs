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
            ToolEquip(isClean); //û�� ���� ����

            if (isClean)
            {
                SetSpeed(3.0f); //�÷��̾��� �̵��ӵ��� 3.0f�� ����
                animator.SetFloat("isClean", 1); //�ִϸ��̼� ���̾� 1�� ����
            }
            else
            {
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����
                animator.SetFloat("isClean", 0); //�ִϸ��̼� ���̾� 0���� ����
            }
        }

        //û�� ���� ���� �� ����
        private void ToolEquip(bool isClean) 
        {
            if (isClean)
            {
                currentTool.transform.SetParent(handToolHolder);             //û�ҵ����� �տ� ����
                currentTool.transform.localRotation = Quaternion.identity;  //û�ҵ����� ȸ���� �ʱ�ȭ
                currentTool.transform.localPosition = Vector3.zero;         //û�ҵ����� ��ġ�� �ʱ�ȭ

                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");  //������ IK Ÿ�� ����
                leftHandIK.data.target = currentTool.transform.Find("LeftHandGrip");    //�޼� IK Ÿ�� ����
                rigBuilder.layers[0].active = true;  //RigBuilder�� ���̾� Ȱ��ȭ
            }                                                                   
            else                                                                
            {
                currentTool.transform.SetParent(backToolHolder);    //û�ҵ����� � ����
                currentTool.transform.localPosition = currentTool.toolBackPosition;    //û�ҵ����� ��ġ�� � ��ġ�� ��ġ�� ����
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, 0);  //û�ҵ����� ��ġ�� � ��ġ�� ȸ������ ����

                rightHandIK.data.target = null;         //������ IK Ÿ�� �ʱ�ȭ
                leftHandIK.data.target = null;          //�޼� IK Ÿ�� �ʱ�ȭ
                rigBuilder.layers[0].active = false;    //RigBuilder�� ���̾� ��Ȱ��ȭ
            }

            rigBuilder.Build(); //RigBuilder �籸��
        }
    }
}
