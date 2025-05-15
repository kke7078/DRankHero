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
        public static PlayerCharacter instance;

        //상호작용 센서
        public InteractionSensor InteractionSensor => interactionSensor;
        [SerializeField] private InteractionSensor interactionSensor;

        //현재 상호작용 가능한 오브젝트 리스트
        public List<IInteractable> CurrentInteractionItems => currentInteractionItems;
        private List<IInteractable> currentInteractionItems = new List<IInteractable>();

        //현재 가장 가까운 상호작용 아이템
        public IInteractable ClosestInteractable { get; private set; }

        [SerializeField] private Transform backToolHolder;         //등에 위치한 청소도구 홀더
        [SerializeField] private Transform handToolHolder;         //손에 위치한 청소도구 홀더
        [SerializeField] private TwoBoneIKConstraint rightHandIK;  //오른손 IK
        [SerializeField] private TwoBoneIKConstraint leftHandIK;   //왼손 IK
        [SerializeField] private InteractionUI interactionUI;      //상호작용 UI

        private bool isCleaning = false;        //플레이어의 청소 유무
        private bool isEquipping = false;       //플레이어의 장비 유무
        private RigBuilder rigBuilder;          //플레이어의 RigBuilder 컴포넌트
        private CleanToolManager currentTool;   //현재 장착된 청소도구
        
        public void OnEnable()
        {
            InputSystem.Singleton.onClean += Clean;
            InputSystem.Singleton.onInteract += Interact;
        }

        private void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            rigBuilder = GetComponent<RigBuilder>();
            currentTool = backToolHolder.GetComponentInChildren<CleanToolManager>(); //초기 청소도구 설정

            //상호작용 센서 컴포넌트 선언
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;

            //interactionUI.HideUI(); //플레이어의 상호작용 UI 비활성화
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

        private void OnDisabled()
        {
            InputSystem.Singleton.onClean -= Clean;
        }

        //상호작용 센서에 의해 상호작용 오브젝트가 감지되었을 때 호출되는 메서드
        private void OnDetectedInteraction(IInteractable interactable)
        {
            if(interactable.InteractionInfos.Count == 0) interactable.Interact(this); //상호작용 정보가 없을 경우 바로 상호작용 실행
            else currentInteractionItems.Add(interactable);
        }

        //상호작용 센서에 의해 상호작용 오브젝트의 신호가 사라졌을 때 호출되는 메서드
        private void OnLostSignalInteraction(IInteractable interactable)
        {
            currentInteractionItems.Remove(interactable);
        }

        //캐릭터와 가장 가까운 상호작용 오브젝트 찾기
        private void FindClosestinteractable()
        {
            //가장 가까운 상호작용 오브젝트 찾기
            if (currentInteractionItems.Count > 0)
            {
                IInteractable closest = null;
                float closestDistance = float.MaxValue;

                foreach (IInteractable interactable in currentInteractionItems)
                {
                    float distance = Vector3.Distance(transform.position, interactable.GetTransform().position);
                    if (distance < closestDistance)
                    {
                        closest = interactable;
                        closestDistance = distance;
                    }
                }

                ClosestInteractable = closest;
            }
            else ClosestInteractable = null;
        }

        //플레이어의 청소 유무에 따른 변화 체크
        private void Clean(bool isClean)
        {
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
                currentTool.transform.SetParent(handToolHolder);
                currentTool.transform.localRotation = Quaternion.identity;
                currentTool.transform.localPosition = Vector3.zero;
            }
            else
            {
                //청소도구를 등에 장착
                currentTool.transform.SetParent(backToolHolder);
                currentTool.transform.localPosition = currentTool.toolBackPosition;
                currentTool.transform.localRotation = Quaternion.Euler(currentTool.toolBackRotation.x, currentTool.toolBackRotation.y, currentTool.toolBackRotation.z);
            }
        }

        //Hand IK 제어
        private void HandIKControl()
        {
            if (isCleaning)
            {
                rightHandIK.data.target = currentTool.transform.Find("RightHandGrip");  //오른손 IK 타겟 설정
                leftHandIK.data.target = currentTool.transform.Find("LeftHandGrip");    //왼손 IK 타겟 설정
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder의 레이어 활성화

                currentTool.toolMainEffect.SetActive(isCleaning); //청소도구 이펙트 활성화
                currentTool.toolSubEffext.SetActive(isCleaning); //청소도구 서브 이펙트 활성화
            }
            else
            {
                rightHandIK.data.target = null; //오른손 IK 타겟 초기화
                leftHandIK.data.target = null;  //왼손 IK 타겟 초기화
                rigBuilder.layers[0].active = isCleaning;  //RigBuilder의 레이어 비활성화

                currentTool.toolMainEffect.SetActive(isCleaning); //청소도구 이펙트 비활성화
                currentTool.toolSubEffext.SetActive(isCleaning); //청소도구 서브 이펙트 비활성화
            }

            rigBuilder.Build(); //RigBuilder 재구성
        }

        //캐릭터 이동 메서드
        public override void Move(Vector2 direction, float speed)
        {
            base.Move(direction, speed);

            FindClosestinteractable();
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
            if (CurrentInteractionItems.Count <= 0) return;

            ClosestInteractable.Interact(this);
            CurrentInteractionItems.Remove(ClosestInteractable);

            //interactionUI.interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
