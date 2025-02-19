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

        public Transform backPipeHolder;    //�÷��̾��� � ��ġ�� ������ Ȧ��
        public Transform handPipeHolder;    //�÷��̾��� �տ� ��ġ�� ������ Ȧ��
        public GameObject pipe; //�÷��̾ ����ϴ� ������
        public GameObject water;

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
            water.SetActive(isClean); //�� ������Ʈ Ȱ��ȭ

            if (isClean)
            {
                SetSpeed(3.0f); //�÷��̾��� �̵��ӵ��� 3.0f�� ����
                //PipeEquip(isClean); //û�� ���� ����

                animator.SetFloat("isClean", 1); //�ִϸ��̼� ���̾� 1�� ����
            }
            else
            {
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����
                //PipeEquip(isClean); //û�� ���� ����

                animator.SetFloat("isClean", 0); //�ִϸ��̼� ���̾� 0���� ����
            }
        }

        //û�� ���� ���� �� ����
        private void PipeEquip(bool isClean) 
        {
            if (isClean)
            {
                pipe.transform.SetParent(handPipeHolder);              //�������� �տ� ����
                pipe.transform.localRotation = Quaternion.identity;    //�������� ȸ���� �ʱ�ȭ
                pipe.transform.localPosition = Vector3.zero;           //�������� ��ġ�� �ʱ�ȭ
                pipe.transform.Rotate(-90, 0, 20);                      //�������� ȸ���� ����
            }
            else
            {
                pipe.transform.SetParent(backPipeHolder);              //�������� � ����
                pipe.transform.localRotation = Quaternion.identity;    //�������� ȸ���� �ʱ�ȭ
                pipe.transform.localPosition = Vector3.zero;           //�������� ��ġ�� �ʱ�ȭ
                pipe.transform.Rotate(0, -45, 90);                     //�������� ȸ���� ����
            }
        }
    }
}
