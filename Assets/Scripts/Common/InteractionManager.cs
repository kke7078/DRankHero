using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace KGY
{
    //InteractionManager Ŭ���� : ��ȣ�ۿ��� �����ϴ� Ŭ����
    public class InteractionManager : SingletonBase<InteractionManager>
    {
        [SerializeField] private InteractionSensor interactionSensor;   //��ȣ�ۿ� ����
        private InteractionUI interactionUI; //��ȣ�ۿ� UI

        //���� ��ȣ�ۿ� �޽����� ��� �� �ִ� ������Ʈ ����Ʈ
        public List<IHasInteractionIds> CurrentInteractionIds => currentInteractionIds;
        private List<IHasInteractionIds> currentInteractionIds = new List<IHasInteractionIds>();

        //���� ��ȣ�ۿ� ������ ������Ʈ ����Ʈ
        public List<IInteractable> CurrentInteractionItems => currentInteractionItems;
        private List<IInteractable> currentInteractionItems = new List<IInteractable>();

        public IInteractable ClosestInteractable { get; private set; }  //���� ���� ����� ��ȣ�ۿ� ������

        //��ȣ�ۿ� �޽��� ������
        public InteractionData InteractionData => interactionData;
        [SerializeField] private InteractionData interactionData;


        private void Start()
        {
            interactionUI = GetComponent<InteractionUI>();

            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        #region Detected / LostSignal
        //��ȣ�ۿ� ������ ���� ������Ʈ�� �����Ǿ��� �� ȣ��Ǵ� �޼���
        private void OnDetectedInteraction(IHasInteractionIds interactable)
        {
            //��ȣ�ۿ� �޽��� ȣ��
            if (interactable.InteractionIdList.Count > 0) CurrentInteractionIds.Add(interactable);

            //interactable�� IInteractable �������̽��� �����ϰ� �ִ��� Ȯ�� -> Interact()�� �Ǵ� ������Ʈ���� Ȯ��
            if (interactable is IInteractable interactableObj)
            {
                if (interactable.InteractionIdList.Count == 0) interactableObj.Interact();  //����� ��ȣ�ۿ� ���� �ٷ� ����
                else CurrentInteractionItems.Add(interactableObj);  //����ڰ� ��ȣ�ۿ� �� �� �ִ� ����Ʈ�� �߰�
            }
        }

        ////��ȣ�ۿ� ������ ���� ��ȣ�ۿ� ������Ʈ�� ��ȣ�� ������� �� ȣ��Ǵ� �޼���
        private void OnLostSignalInteraction(IHasInteractionIds interactable)
        {
            CurrentInteractionIds.Remove(interactable);

            //interactable�� IInteractable �������̽��� �����ϰ� �ִ��� Ȯ��
            if (interactable is IInteractable interactableItem) CurrentInteractionItems.Remove(interactableItem);

            
        }
        #endregion


        //�÷��̾�� ���� ����� ��ȣ�ۿ� ������Ʈ ã��
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
            }
            else
            {
                ClosestInteractable = null;
            }
        }

        //��ȣ�ۿ� �Ϸ� �� ȣ��Ǵ� �޼���
        public void InteractComplete(IInteractable interactable)
        {
            CurrentInteractionItems.Remove(interactable);
            CurrentInteractionIds.Remove(interactable as IHasInteractionIds);
        }
    }
}
