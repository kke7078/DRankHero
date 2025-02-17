using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterController 클래스 : 캐릭터의 물리적인 동작 및 상호작용을 담당하는 클래스 (전반적인 제어 & 상태관리)
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

            //캐릭터 기본 클래스 선언
            characterBase = GetComponent<CharacterBase>();

            //유니티엔진 캐릭터컨트롤러 선언
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
