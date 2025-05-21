using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace KGY
{
    //InteractionManager 클래스 : 상호작용을 관리하는 클래스
    public class InteractionManager : SingletonBase<InteractionManager>
    {
        private InteractionUI itneractionUI;  //상호작용 UI
        [SerializeField] private InteractionSensor interactionSensor;   //플레이어 상호작용 센서

        //상호작용 메시지 데이터
        public InteractionData InteractionData => interactionData;
        [SerializeField] private InteractionData interactionData;

        //상호작용 메시지(IHasInteractionIds) 를 가진 오브젝트 리스트 → InteractionSensor에서 감지
        public List<IHasInteractionIds> CurrentInteractionID => currentInteractionID;
        private List<IHasInteractionIds> currentInteractionID = new List<IHasInteractionIds>();

        //상호작용 가능(IInteractable) 오브젝트 리스트 → InteractionSensor에서 감지
        public List<IInteractable> CurrentInteractable => currentInteractable;
        private List<IInteractable> currentInteractable = new List<IInteractable>();

        public IInteractable ClosestInteractable { get; private set; }  //현재 가장 가까운 상호작용 아이템

        private void Start()
        {
            itneractionUI = GetComponent<InteractionUI>();
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        #region Detected / LostSignal
        //상호작용 센서에 의해 오브젝트가 감지되었을 때 호출되는 메서드
        private void OnDetectedInteraction(IHasInteractionIds interactable)
        {
            //상호작용 메시지 호출
            if (interactable.InteractionIdList.Count > 0) CurrentInteractionID.Add(interactable);

            //interactable이 IInteractable 인터페이스를 구현하고 있는지 확인 -> Interact()가 되는 오브젝트인지 확인
            if (interactable is IInteractable interactableObj)
            {
                if (interactable.InteractionIdList.Count == 0) interactableObj.Interact();  //사용자 상호작용 없이 바로 동작
                else CurrentInteractable.Add(interactableObj);  //사용자가 상호작용 할 수 있는 리스트에 추가
            }
        }

        //상호작용 센서에 의해 상호작용 오브젝트의 신호가 사라졌을 때 호출되는 메서드
        private void OnLostSignalInteraction(IHasInteractionIds interactable)
        {
            if (interactable.InteractionIdList.Count > 0) CurrentInteractionID.Remove(interactable);

            //interactable이 IInteractable 인터페이스를 구현하고 있는지 확인
            if (interactable is IInteractable interactableItem) CurrentInteractable.Remove(interactableItem);
        }
        #endregion


        //플레이어와 가장 가까운 상호작용 오브젝트 찾기
        public void FindClosestinteractable()
        {
            if (CurrentInteractable.Count > 0)
            {
                IInteractable closest = null;
                float closestDistance = float.MaxValue;

                Vector3 playerPos = interactionSensor.GetComponentInParent<PlayerCharacter>().transform.position;
                foreach (IInteractable interactable in CurrentInteractable)
                {
                    float distance = Vector3.Distance(playerPos, interactable.GetTransform().position);
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

        //상호작용 완료 후 호출되는 메서드
        public void InteractComplete(IInteractable interactable)
        {
            CurrentInteractable.Remove(interactable);
            CurrentInteractionID.Remove(interactable as IHasInteractionIds);
        }
    }
}
