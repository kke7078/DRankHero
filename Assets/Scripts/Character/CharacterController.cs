using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterController Ŭ���� : ĳ������ �������� ���� �� ��ȣ�ۿ��� ����ϴ� Ŭ���� (�������� ���� & ���°���)
    public class CharacterController : MonoBehaviour
    {
        public static CharacterController instance; 

        protected float targetRotation;
        protected float rotationVelocity;

        private CharacterBase characterBase;
        private UnityEngine.CharacterController unityCharacterController;

        private void Awake()
        {
            instance = this;

            //ĳ���� �⺻ Ŭ���� ����
            characterBase = GetComponent<CharacterBase>();

            //����Ƽ���� ĳ������Ʈ�ѷ� ����
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
        }

        private void Update()
        {
            Move(characterBase.Direction, characterBase.GetSpeed());
        }

        private void Move(Vector2 direction, float speed)
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
