using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterBase 클래스 : 모든 캐릭터의 기본 클래스(공통 속성/기능을 정의 ex.이동, 회전, 등등)
    //모든 캐릭터가 공통적으로 가지는 속성과 메서드 정의
    public class CharacterBase : MonoBehaviour
    {
        public Vector2 Direction { get; set; }  //이동 방향

        //Dialogue UI에 사용될  캐릭터 이름
        public string CharacterName => characterName;
        [SerializeField] private string characterName;

        //Dialogue UI에 사용될 캐릭터 초상화
        public IReadOnlyList<Sprite> CharacterPortraits => characterPortraits;
        [SerializeField] private Sprite[] characterPortraits; 

        protected Animator animator;

        private float targetRotation;
        private float rotationVelocity;
        private float baseSpeed;
        private UnityEngine.CharacterController unityCharacterController;

        protected virtual void Start()
        {
            SetSpeed(5.0f);   //기본 이동 속도 설정
            animator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>(); //유니티엔진 캐릭터컨트롤러 선언
        }

        public float GetSpeed()
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)
        {
            baseSpeed = newSpeed;
        }

        //캐릭터 이동 메서드
        public virtual void Move(Vector2 direction, float speed)
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