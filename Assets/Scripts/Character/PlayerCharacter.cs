using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace KGY
{
    //PlayerCharacter 클래스 : 플레이어 캐릭터의 속성 및 동작을 정의하는 클래스 (CharacterBase 클래스를 상속받아서 확장)
    public class PlayerCharacter : CharacterBase
    {

        public Transform backToolHolder;        //플레이어의 등에 위치한 청소도구 홀더
        public Transform handToolHolder;        //플레이어의 손에 위치한 청소도구 홀더
        public TwoBoneIKConstraint rightHandIK; //플레이어의 오른손 IK
        public TwoBoneIKConstraint leftHandIK;  //플레이어의 왼손 IK

        protected bool isCleaning = false;      //플레이어의 청소 유무

        private Animator animator;              //플레이어의 애니메이터 컴포넌트
        private RigBuilder rigBuilder;          //플레이어의 RigBuilder 컴포넌트
        private CleanToolManager currentTool;   //현재 장착된 청소도구
        

        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rigBuilder = GetComponent<RigBuilder>();
            currentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //초기 청소도구 설정

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
            ToolEquip(isClean); //청소 도구 장착

            if (isClean)
            {
                SetSpeed(3.0f); //플레이어의 이동속도를 3.0f로 설정
                animator.SetFloat("isClean", 1); //애니메이션 레이어 1로 설정
            }
            else
            {
                SetSpeed(5.0f); //플레이어의 이동속도를 5.0f로 원복
                animator.SetFloat("isClean", 0); //애니메이션 레이어 0으로 설정
            }
        }

        //청소 도구 장착 및 해제
        private void ToolEquip(bool isClean) 
        {
            if (isClean)
            {
                currentTool.transform.SetParent(handToolHolder);             //청소도구를 손에 장착
                currentTool.transform.localRotation = Quaternion.identity;  //청소도구의 회전을 초기화
                currentTool.transform.localPosition = Vector3.zero;         //청소도구의 위치를 초기화

                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");  //오른손 IK 타겟 설정
                leftHandIK.data.target = currentTool.transform.Find("LeftHandGrip");    //왼손 IK 타겟 설정
                rigBuilder.layers[0].active = true;  //RigBuilder의 레이어 활성화
            }                                                                   
            else                                                                
            {
                currentTool.transform.SetParent(backToolHolder);    //청소도구를 등에 장착
                currentTool.transform.localPosition = currentTool.toolBackPosition;    //청소도구의 위치를 등에 위치한 위치로 설정
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, 0);  //청소도구의 위치를 등에 위치한 회전으로 설정

                rightHandIK.data.target = null;         //오른손 IK 타겟 초기화
                leftHandIK.data.target = null;          //왼손 IK 타겟 초기화
                rigBuilder.layers[0].active = false;    //RigBuilder의 레이어 비활성화
            }

            rigBuilder.Build(); //RigBuilder 재구성
        }
    }
}
