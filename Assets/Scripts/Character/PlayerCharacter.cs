using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //PlayerCharacter 클래스 : 플레이어 캐릭터의 속성 및 동작을 정의하는 클래스 (CharacterBase 클래스를 상속받아서 확장)
    public class PlayerCharacter : CharacterBase
    {

        public Transform backPipeHolder;    //플레이어의 등에 위치한 파이프 홀더
        public Transform handPipeHolder;    //플레이어의 손에 위치한 파이프 홀더
        public GameObject pipe; //플레이어가 사용하는 파이프
        public GameObject water;

        protected bool isCleaning = false;    //플레이어의 청소 유무

        private Animator animator; //플레이어의 애니메이터 컴포넌트

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;

            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            SetSpeed(5.0f); //플레이어의 기본이동 속도 설정
        }

        private void Update()
        {
            Direction = InputSystem.Singleton.MoveInput;    //플레이어의 이동 방향 설정
            animator.SetFloat("isMove", Direction.magnitude);

            //플레이어의 회전 방향 설정
            if (isCleaning)
            {
                //클릭하는 방향으로 플레이어 회전
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

        //플레이어의 청소 유무에 따른 변화 체크
        private void Clean(bool isClean)
        {
            isCleaning = isClean;
            water.SetActive(isClean); //물 오브젝트 활성화

            if (isClean)
            {
                SetSpeed(3.0f); //플레이어의 이동속도를 3.0f로 설정
                //PipeEquip(isClean); //청소 도구 장착

                animator.SetFloat("isClean", 1); //애니메이션 레이어 1로 설정
            }
            else
            {
                SetSpeed(5.0f); //플레이어의 이동속도를 5.0f로 원복
                //PipeEquip(isClean); //청소 도구 해제

                animator.SetFloat("isClean", 0); //애니메이션 레이어 0으로 설정
            }
        }

        //청소 도구 장착 및 해제
        private void PipeEquip(bool isClean) 
        {
            if (isClean)
            {
                pipe.transform.SetParent(handPipeHolder);              //파이프를 손에 장착
                pipe.transform.localRotation = Quaternion.identity;    //파이프의 회전을 초기화
                pipe.transform.localPosition = Vector3.zero;           //파이프의 위치를 초기화
                pipe.transform.Rotate(-90, 0, 20);                      //파이프의 회전을 설정
            }
            else
            {
                pipe.transform.SetParent(backPipeHolder);              //파이프를 등에 장착
                pipe.transform.localRotation = Quaternion.identity;    //파이프의 회전을 초기화
                pipe.transform.localPosition = Vector3.zero;           //파이프의 위치를 초기화
                pipe.transform.Rotate(0, -45, 90);                     //파이프의 회전을 설정
            }
        }
    }
}
