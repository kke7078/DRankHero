using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //PlayerCharacter Ŭ���� : �÷��̾� ĳ������ �Ӽ� �� ������ �����ϴ� Ŭ���� (CharacterBase Ŭ������ ��ӹ޾Ƽ� Ȯ��)
    public class PlayerCharacter : CharacterBase
    {

        protected bool isCleaning = false;    //�÷��̾��� û�� ����

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
        }

        private void Start()
        {
            SetSpeed(5.0f); //�÷��̾��� �⺻�̵� �ӵ� ����
        }

        private void Update()
        {
            Direction = InputSystem.Singleton.MoveInput;    //�÷��̾��� �̵� ���� ����

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
            }
            else
            {
                isCleaning = false;
                SetSpeed(5.0f); //�÷��̾��� �̵��ӵ��� 5.0f�� ����
            }
        }
    }
}
