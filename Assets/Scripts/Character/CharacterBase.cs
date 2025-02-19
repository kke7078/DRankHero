using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //CharacterBase Ŭ���� : ��� ĳ������ �⺻ Ŭ����(���� �Ӽ�/����� ���� ex.�̵�, ȸ��, ���)
    //��� ĳ���Ͱ� ���������� ������ �Ӽ��� �޼��� ����
    public class CharacterBase : MonoBehaviour
    {
        public Vector2 Direction { get; set; }  //�̵� ����
        
        protected float targetRotation;
        protected float rotationVelocity;

        private float baseSpeed;    //�⺻ �̵� �ӵ�
        private Vector2 direction;

        private UnityEngine.CharacterController unityCharacterController;

        protected virtual void Awake()
        {
            baseSpeed = 5.0f;   //�⺻ �̵� �ӵ� ����

            //����Ƽ���� ĳ������Ʈ�ѷ� ����
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
        }

        public float GetSpeed()    //�̵� �ӵ� ��ȯ �޼���
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)    //�̵� �ӵ� ���� �޼���
        {
            baseSpeed = newSpeed;
        }

        //ĳ���� �̵� �޼���
        public void Move(Vector2 direction, float speed)
        {
            float magnitude = direction.magnitude;
            if (magnitude <= 0.1f) return;

            Vector3 inputDirection = new Vector3(direction.x, 0, direction.y).normalized;
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, 0.15f);
            transform.rotation = Quaternion.Euler(0, rotation, 0);

            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

            unityCharacterController.Move(targetDirection * speed * Time.deltaTime);
        }
    }
}