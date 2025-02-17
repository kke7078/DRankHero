using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //CharacterBase Ŭ���� : ��� ĳ������ �⺻ Ŭ����(���� �Ӽ�/����� ���� ex.�̵�, ȸ��, ���)
    //��� ĳ���Ͱ� ���������� ������ �Ӽ��� �޼��� ����
    public class CharacterBase : MonoBehaviour
    {
        private float baseSpeed;    //�⺻ �̵� �ӵ�
        public Vector2 Direction { get; set; }  //�̵� ����

        private Vector2 direction;

        protected virtual void Awake()
        {
            baseSpeed = 5.0f;   //�⺻ �̵� �ӵ� ����
        }

        public float GetSpeed()    //�̵� �ӵ� ��ȯ �޼���
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)    //�̵� �ӵ� ���� �޼���
        {
            baseSpeed = newSpeed;
        }
    }
}