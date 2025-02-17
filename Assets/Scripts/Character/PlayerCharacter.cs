using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //PlayerCharacter Ŭ���� : �÷��̾� ĳ������ �Ӽ� �� ������ �����ϴ� Ŭ���� (CharacterBase Ŭ������ ��ӹ޾Ƽ� Ȯ��)
    public class PlayerCharacter : CharacterBase
    {

        public Transform backPipeHolder;    //�÷��̾��� � ��ġ�� ������ Ȧ��
        public Transform handPipeHolder;    //�÷��̾��� �տ� ��ġ�� ������ Ȧ��

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
            if (isClean)
            {
                isCleaning = true;
                SetSpeed(3.0f); //�÷��̾��� �̵��ӵ��� 3.0f�� ����
                PipeEquip(isClean); //û�� ���� ����

                animator.SetFloat("isClean", 1); //�ִϸ��̼� ���̾� 1�� ����
            }
            else
            {
                isCleaning = false;
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����
                PipeEquip(isClean); //û�� ���� ����

                animator.SetFloat("isClean", 0); //�ִϸ��̼� ���̾� 0���� ����
            }
        }

        private void PipeEquip(bool isClean) 
        {
            if (isClean)
            {
                Debug.Log("�տ� �����ؾ���");
            }
            else
            {
                Debug.Log("� �����ؾ���!");
            }
        }
    }
}
