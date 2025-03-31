using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KGY
{
    //CharacterBase Ŭ���� : ��� ĳ������ �⺻ Ŭ����(���� �Ӽ�/����� ���� ex.�̵�, ȸ��, ���)
    //��� ĳ���Ͱ� ���������� ������ �Ӽ��� �޼��� ����
    public class CharacterBase : MonoBehaviour
    {
        public Vector2 Direction { get; set; }  //�̵� ����
        private Vector2 direction;
        public InteractionSensor interactionSensor;
        public InteractionUI InteractionUI;
        public bool isMoving;

        public List<IInteractable> currentInteractionItems = new List<IInteractable>();
        public IInteractable closestInteractable;

        protected float targetRotation;
        protected float rotationVelocity;
        protected Animator animator;

        private float baseSpeed;    //�⺻ �̵� �ӵ�
        private UnityEngine.CharacterController unityCharacterController;


        protected virtual void Start()
        {
            baseSpeed = 5.0f;   //�⺻ �̵� �ӵ� ����

            animator = GetComponent<Animator>();

            //����Ƽ���� ĳ������Ʈ�ѷ� ����
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();

            //��ȣ�ۿ� ���� ������Ʈ ����
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        public float GetSpeed()    //�̵� �ӵ� ��ȯ �޼���
        {
            return baseSpeed;
        }

        public void SetSpeed(float newSpeed)    //�̵� �ӵ� ���� �޼���
        {
            baseSpeed = newSpeed;
        }

        //ĳ���� �̵� �޼���
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

        //ĳ���Ϳ� ���� ����� ��ȣ�ۿ� ������Ʈ ã��
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