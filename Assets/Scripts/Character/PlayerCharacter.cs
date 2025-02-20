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

        public Transform backToolHolder;    //�÷��̾��� � ��ġ�� û�ҵ��� Ȧ��
        public Transform handToolHolder;    //�÷��̾��� �տ� ��ġ�� û�ҵ��� Ȧ��
        public CleanToolManager currentTool;      //���� ������ û�ҵ���

        public TwoBoneIKConstraint rightHandIK;    //�÷��̾��� ������ IK

        protected bool isCleaning = false;    //�÷��̾��� û�� ����

        private Animator animator; //�÷��̾��� �ִϸ����� ������Ʈ

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;

            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            SetSpeed(5.0f); //�÷��̾��� �⺻�̵� �ӵ� ����
            currentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //û�ҵ��� ����
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

                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");
                GetComponent<RigBuilder>().enabled = true;    //RigBuilder ������Ʈ Ȱ��ȭ
            }                                                                   
            else                                                                
            {
                GetComponent<RigBuilder>().enabled = false;    //RigBuilder ������Ʈ Ȱ��ȭ

                currentTool.transform.SetParent(backToolHolder);    //û�ҵ����� � ����
                currentTool.transform.localPosition = currentTool.toolBackPosition;    //û�ҵ����� ��ġ�� � ��ġ�� ��ġ�� ����
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, 0);  //û�ҵ����� ��ġ�� � ��ġ�� ȸ������ ����
            }
        }
    }
}
