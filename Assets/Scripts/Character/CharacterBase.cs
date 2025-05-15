using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterBase Ŭ���� : ��� ĳ������ �⺻ Ŭ����(���� �Ӽ�/����� ���� ex.�̵�, ȸ��, ���)
    //��� ĳ���Ͱ� ���������� ������ �Ӽ��� �޼��� ����
    public class CharacterBase : MonoBehaviour
    {
        public Vector2 Direction { get; set; }  //�̵� ����

        public bool IsMoving { get; set; } //ĳ���� �̵� ���� ����

        //Dialogue UI�� ����  ĳ���� �̸�
        public string CharacterName => characterName;
        [SerializeField] private string characterName;

        //Dialogue UI�� ���� ĳ���� �ʻ�ȭ
        public IReadOnlyList<Sprite> CharacterPortraits => characterPortraits;
        [SerializeField] private Sprite[] characterPortraits; 

        protected Animator animator;

        private float targetRotation;
        private float rotationVelocity;
        private float baseSpeed;
        private UnityEngine.CharacterController unityCharacterController;

        protected virtual void Start()
        {
            SetSpeed(5.0f);   //�⺻ �̵� �ӵ� ����
            animator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>(); //����Ƽ���� ĳ������Ʈ�ѷ� ����
        }

        public float GetSpeed()
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)
        {
            baseSpeed = newSpeed;
        }

        //ĳ���� �̵� �޼���
        public virtual void Move(Vector2 direction, float speed)
        {
            if (IsMoving) return;

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