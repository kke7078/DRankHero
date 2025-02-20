using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //PlayerCharacter Ŭ���� : �÷��̾� ĳ������ �Ӽ� �� ������ �����ϴ� Ŭ���� (CharacterBase Ŭ������ ��ӹ޾Ƽ� Ȯ��)
    public class PlayerCharacter : CharacterBase
    {

        public Transform backToolHolder;    //�÷��̾��� � ��ġ�� û�ҵ��� Ȧ��
        public Transform handToolHolder;    //�÷��̾��� �տ� ��ġ�� û�ҵ��� Ȧ��
        public GameObject currentTool;      //�÷��̾ ����ϴ� û�ҵ���

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
            //PipeEquip(isClean); //û�� ���� ����

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
        private void PipeEquip(bool isClean) 
        {
            if (isClean)
            {
                currentTool.transform.SetParent(handToolHolder);              //û�ҵ����� �տ� ����
                currentTool.transform.localRotation = Quaternion.identity;    //û�ҵ����� ȸ���� �ʱ�ȭ
                currentTool.transform.localPosition = Vector3.zero;           //û�ҵ����� ��ġ�� �ʱ�ȭ
                currentTool.transform.Rotate(-90, 0, 30);                     //û�ҵ����� ȸ���� ����
            }                                                                   
            else                                                                
            {                                                                   
                currentTool.transform.SetParent(backToolHolder);              //û�ҵ����� � ����
                currentTool.transform.localRotation = Quaternion.identity;    //û�ҵ����� ȸ���� �ʱ�ȭ
                currentTool.transform.localPosition = Vector3.zero;           //û�ҵ����� ��ġ�� �ʱ�ȭ
                currentTool.transform.Rotate(0, -45, 90);                     //û�ҵ����� ȸ���� ����
            }
        }
    }
}
