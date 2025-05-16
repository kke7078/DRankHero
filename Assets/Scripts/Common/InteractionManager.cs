using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //InteractionManager 클래스 : 상호작용을 관리하는 클래스
    public class InteractionManager : SingletonBase<InteractionManager>
    {
        [SerializeField] private InteractionSensor interactionSensor;   //상호작용 센서
        private InteractionUI interactionUI; //상호작용 UI

        //현재 상호작용 가능한 오브젝트 리스트
        public List<IInteractable> CurrentInteractionItems => currentInteractionItems;
        private List<IInteractable> currentInteractionItems = new List<IInteractable>();

        public IInteractable ClosestInteractable { get; private set; }  //현재 가장 가까운 상호작용 아이템


        private void Start()
        {
            interactionUI = GetComponent<InteractionUI>();

            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        #region Detected / LostSignal
        //상호작용 센서에 의해 상호작용 오브젝트가 감지되었을 때 호출되는 메서드
        private void OnDetectedInteraction(InteractionMsg interactable)
        {
            //interactable이 IInteractable 인터페이스를 구현하고 있는지 확인
            if (interactable is IInteractable interactableObj)
            {
                if (interactable.InteractionDatas.Count == 0) interactableObj.Interact();
                else CurrentInteractionItems.Add(interactableObj);
            }

            //interactable이 IInteractable 인터페이스를 구현하지 않는 경우 (ex.projector)
            else
            { 
                
            }
        }

        //상호작용 센서에 의해 상호작용 오브젝트의 신호가 사라졌을 때 호출되는 메서드
        private void OnLostSignalInteraction(InteractionMsg interactable)
        {
            //interactable이 IInteractable 인터페이스를 구현하고 있는지 확인
            if (interactable is IInteractable interactableObj)
            {
                CurrentInteractionItems.Remove(interactableObj);
            }
        }
        #endregion


        //플레이어와 가장 가까운 상호작용 오브젝트 찾기
        public void FindClosestinteractable()
        {
            if (CurrentInteractionItems.Count > 0)
            {
                IInteractable closest = null;
                float closestDistance = float.MaxValue;

                Vector3 playerPos = interactionSensor.GetComponentInParent<PlayerCharacter>().transform.position;
                foreach (IInteractable interactable in CurrentInteractionItems)
                {
                    float distance = Vector3.Distance(playerPos, interactable.GetTransform().position);
                    if (distance < closestDistance)
                    {
                        closest = interactable;
                        closestDistance = distance;
                    }
                }

                ClosestInteractable = closest;
                interactionUI.ShowInteractionUI(ClosestInteractable);

            }
            else
            {
                ClosestInteractable = null;
                interactionUI.HideInteractUI();
            }
        }

        //상호작용 완료 후 호출되는 메서드
        public void InteractComplete(IInteractable interactable)
        {
            currentInteractionItems.Remove(interactable);
        }























    }
}
