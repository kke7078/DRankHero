using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //PlayerCharacter 클래스 : 플레이어 캐릭터의 속성 및 동작을 정의하는 클래스 (CharacterBase 클래스를 상속받아서 확장)
    public class PlayerCharacter : CharacterBase
    {

        protected bool isCleaning = false;    //플레이어의 청소 유무

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
        }

        private void Start()
        {
            SetSpeed(5.0f); //플레이어의 기본이동 속도 설정
        }

        private void Update()
        {
            Direction = InputSystem.Singleton.MoveInput;    //플레이어의 이동 방향 설정

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

            if (isClean)
            {
                isCleaning = true;
                SetSpeed(3.0f); //플레이어의 이동속도를 3.0f로 설정
            }
            else
            {
                isCleaning = false;
                SetSpeed(5.0f); //플레이어의 이동속도를 5.0f로 원복
            }
        }
    }
}
