using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //CharacterBase 클래스 : 모든 캐릭터의 기본 클래스(공통 속성/기능을 정의 ex.이동, 회전, 등등)
    //모든 캐릭터가 공통적으로 가지는 속성과 메서드 정의
    public class CharacterBase : MonoBehaviour
    {
        private float baseSpeed;    //기본 이동 속도
        public Vector2 Direction { get; set; }  //이동 방향

        private Vector2 direction;

        protected virtual void Awake()
        {
            baseSpeed = 5.0f;   //기본 이동 속도 설정
        }

        public float GetSpeed()    //이동 속도 반환 메서드
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)    //이동 속도 설정 메서드
        {
            baseSpeed = newSpeed;
        }
    }
}