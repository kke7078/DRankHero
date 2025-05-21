using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

namespace KGY
{
    //PlayerCharacter 클래스 : 플레이어 캐릭터의 속성 및 동작을 정의하는 클래스 (CharacterBase 클래스를 상속받아서 확장)
    public class PlayerCharacter : CharacterBase
    {
        public CleanToolManager CurrentTool { get; private set; }   //현재 장착된 청소도구

        [SerializeField] private Transform backToolHolder;         //등에 위치한 청소도구 홀더
        [SerializeField] private Transform handToolHolder;         //손에 위치한 청소도구 홀더
        [SerializeField] private TwoBoneIKConstraint rightHandIK;  //오른손 IK
        [SerializeField] private TwoBoneIKConstraint leftHandIK;   //왼손 IK
        [SerializeField] private InteractionUI interactionUI;      //상호작용 UI

        private bool isCleaning = false;        //플레이어의 청소 유무
        private bool isEquipping = false;       //플레이어의 장비 유무
        private RigBuilder rigBuilder;          //플레이어의 RigBuilder 컴포넌트
                
        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
            InputSystem.Singleton.onInteract += Interact;
        }

        protected override void Start()
        {
            base.Start();

            rigBuilder = GetComponent<RigBuilder>();
            CurrentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //초기 청소도구 설정
        }

        private void Update()
        {
            if (GameManager.Singleton.IsInDialogue) return;

            Direction = InputSystem.Singleton.MoveInput;    //플레이어의 이동 방향 설정
            animator.SetFloat("isMove", Direction.magnitude);

            Rotate();
        }

        private void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        //플레이어의 청소 유무에 따른 변화 체크
        private void Clean(bool isClean)
        {
            if (GameManager.Singleton.IsInDialogue) return;

            isCleaning = isClean;

            if (isClean)
            {
                SetSpeed(3.0f); //플레이어의 이동속도를 3.0f로 설정
                Equip();
            }
            else
            {
                SetSpeed(5.0f); //플레이어의 이동속도를 5.0f로 원복
                UnEquip();

                //Hand IK 초기화
                HandIKControl();
            }
        }

        private void Equip() {
            if (isEquipping) return;

            isEquipping = true;

            //플레이어의 UnEquip 애니메이션 초기화
            animator.SetBool("isUnEquip", !isEquipping);
            animator.ResetTrigger("UnEquipTrigger");

            //플레이어의 Equip 애니메이션 실행
            animator.SetBool("isEquip", isEquipping);
            animator.SetTrigger("EquipTrigger");
        }

        private void UnEquip() {
            isEquipping = false;

            //플레이어의 Equip 애니메이션 초기화
            animator.SetBool("isEquip", isEquipping);
            animator.ResetTrigger("EquipTrigger");

            //플레이어의 UnEquip 애니메이션 실행
            animator.SetBool("isUnEquip", !isEquipping);
            animator.SetTrigger("UnEquipTrigger");
        }

        public void EquipControl(string status)
        {
            if (status == "equip")
            {
                //청소도구 손에 장착
                ToolEquip();
            }
            else if (status == "equipEnd")
            {
                //equip 애니메이션 해제
                animator.SetBool("isEquip", false);

                //clean 애니메이션 설정
                animator.SetFloat("isClean", 1);

                //Hand IK 설정
                Invoke("HandIKControl", 0.03f);
            }
            else if (status == "unEquip")
            {
                //청소도구 등에 장착
                ToolEquip();

                //Idle 애니메이션 설정
                animator.SetFloat("isClean", 0);
            }
            else if (status == "unEquipEnd") {
                //unEquip 애니메이션 해제
                animator.SetBool("isUnEquip", false);
            }
        }

        //청소 도구 장착 및 해제
        private void ToolEquip()
        {
            if (isCleaning)
            {
                //청소도구 손에 장착
                CurrentTool.transform.SetParent(handToolHolder);
                CurrentTool.transform.localRotation = Quaternion.identity;
                CurrentTool.transform.localPosition = Vector3.zero;
            }
            else
            {
                //청소도구를 등에 장착
                CurrentTool.transform.SetParent(backToolHolder);
                CurrentTool.transform.localPosition = CurrentTool.ToolBackPosition;
                CurrentTool.transform.localRotation = Quaternion.Euler(CurrentTool.ToolBackRotation.x, CurrentTool.ToolBackRotation.y, CurrentTool.ToolBackRotation.z);
            }
        }

        //Hand IK 제어
        private void HandIKControl()
        {
            if (isCleaning)
            {
                rightHandIK.data.target = CurrentTool.transform.Find("RightHandGrip");  //오른손 IK 타겟 설정
                leftHandIK.data.target = CurrentTool.transform.Find("LeftHandGrip");    //왼손 IK 타겟 설정
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder의 레이어 활성화

                CurrentTool.ToolMainEffect.SetActive(isCleaning); //청소도구 이펙트 활성화
                CurrentTool.ToolSubEffext.SetActive(isCleaning); //청소도구 서브 이펙트 활성화
            }
            else
            {
                rightHandIK.data.target = null; //오른손 IK 타겟 초기화
                leftHandIK.data.target = null;  //왼손 IK 타겟 초기화
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder의 레이어 비활성화

                CurrentTool.ToolMainEffect.SetActive(isCleaning); //청소도구 이펙트 비활성화
                CurrentTool.ToolSubEffext.SetActive(isCleaning); //청소도구 서브 이펙트 비활성화
            }

            rigBuilder.Build(); //RigBuilder 재구성
        }

        //캐릭터 이동 메서드
        public override void Move(Vector2 direction, float speed)
        {
            if (GameManager.Singleton.IsInDialogue || GameManager.Singleton.IsPause) return;

            base.Move(direction, speed);

            InteractionManager.Singleton.FindClosestinteractable(); //상호작용 가능한 오브젝트 찾기
        }

        //캐릭터 회전 메서드
        private void Rotate()
        {
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

        //플레이어의 움직임 상태 설정
        public void SetPlayerMovementState(bool moving)
        {
            if (!moving)
            {
                Clean(moving);
                animator.SetFloat("isMove", 0);
            }
        }

        //플레이어 상호작용 동작 메서드
        private void Interact()
        {
            var InteractMng = InteractionManager.Singleton;
            if (GameManager.Singleton.IsInDialogue || InteractMng.CurrentInteractable.Count <= 0) return;

            var targetObj = InteractMng.ClosestInteractable;
            if (targetObj == null) return;

            targetObj.Interact();
            InteractMng.InteractComplete(targetObj);
        }
    }
}
