using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KGY
{
    //CharacterBase 클래스 : 모든 캐릭터의 기본 클래스(공통 속성/기능을 정의 ex.이동, 회전, 등등)
    //모든 캐릭터가 공통적으로 가지는 속성과 메서드 정의
    public class CharacterBase : MonoBehaviour
    {
        public Vector2 Direction { get; set; }  //이동 방향
        private Vector2 direction;
        public InteractionSensor interactionSensor;
        public InteractionUI InteractionUI;
        public bool isMoving;

        public List<IInteractable> currentInteractionItems = new List<IInteractable>();
        public IInteractable closestInteractable;

        protected float targetRotation;
        protected float rotationVelocity;
        protected Animator animator;

        private float baseSpeed;    //기본 이동 속도
        private UnityEngine.CharacterController unityCharacterController;


        protected virtual void Start()
        {
            baseSpeed = 5.0f;   //기본 이동 속도 설정

            animator = GetComponent<Animator>();

            //유니티엔진 캐릭터컨트롤러 선언
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();

            //상호작용 센서 컴포넌트 선언
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        public float GetSpeed()    //이동 속도 반환 메서드
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)    //이동 속도 설정 메서드
        {
            baseSpeed = newSpeed;
        }

        //캐릭터 이동 메서드
        public void Move(Vector2 direction, float speed)
        {
            if (!isMoving) return;

            float magnitude = direction.magnitude;
            if (magnitude <= 0.1f) return;

            Vector3 inputDirection = new Vector3(direction.x, 0, direction.y).normalized;
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, 0.15f);
            transform.rotation = Quaternion.Euler(0, rotation, 0);

            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

            unityCharacterController.Move(targetDirection * speed * Time.deltaTime);

            FindClosestinteractable();
        }

        public void OnDetectedInteraction(IInteractable interactable) {
            if (interactable.IsAutoInteract) interactable.Interact(this);
            else currentInteractionItems.Add(interactable);
        }

        public void OnLostSignalInteraction(IInteractable interactable)
        {
            currentInteractionItems.Remove(interactable);
        }

        //캐릭터와 가장 가까운 상호작용 오브젝트 찾기
        public void FindClosestinteractable() {
            if (currentInteractionItems.Count == 0)
            {
                InteractionUI.HideUI();
                return;
            }

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

            closestInteractable = closest;
            InteractionUI.ShowUI(closestInteractable);
        }

        protected void OnAnimatorIK(int layerIndex)
        {
            if (animator == null) return;

            AdjustFootPosition(AvatarIKGoal.LeftFoot);
            AdjustFootPosition(AvatarIKGoal.RightFoot);
        }

        private void AdjustFootPosition(AvatarIKGoal foot)
        {
            RaycastHit hit;
            Vector3 footPosition = animator.GetIKPosition(foot);
            Vector3 rayOrigin = footPosition + Vector3.up * 0.5f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1f)) 
            {
                footPosition.y = hit.point.y + 0.5f;
                animator.SetIKPosition(foot, footPosition);
            }
        }
    }
}