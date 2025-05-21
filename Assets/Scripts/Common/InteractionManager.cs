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
        private InteractionUI itneractionUI;  //��ȣ�ۿ� UI
        [SerializeField] private InteractionSensor interactionSensor;   //�÷��̾� ��ȣ�ۿ� ����

        //��ȣ�ۿ� �޽��� ������
        public InteractionData InteractionData => interactionData;
        [SerializeField] private InteractionData interactionData;

        //��ȣ�ۿ� �޽���(IHasInteractionIds) �� ���� ������Ʈ ����Ʈ �� InteractionSensor���� ����
        public List<IHasInteractionIds> CurrentInteractionID => currentInteractionID;
        private List<IHasInteractionIds> currentInteractionID = new List<IHasInteractionIds>();

        //��ȣ�ۿ� ����(IInteractable) ������Ʈ ����Ʈ �� InteractionSensor���� ����
        public List<IInteractable> CurrentInteractable => currentInteractable;
        private List<IInteractable> currentInteractable = new List<IInteractable>();

        public IInteractable ClosestInteractable { get; private set; }  //���� ���� ����� ��ȣ�ۿ� ������

        private void Start()
        {
            itneractionUI = GetComponent<InteractionUI>();
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLostSignal += OnLostSignalInteraction;
        }

        #region Detected / LostSignal
        //��ȣ�ۿ� ������ ���� ������Ʈ�� �����Ǿ��� �� ȣ��Ǵ� �޼���
        private void OnDetectedInteraction(IHasInteractionIds interactable)
        {
            //��ȣ�ۿ� �޽��� ȣ��
            if (interactable.InteractionIdList.Count > 0) CurrentInteractionID.Add(interactable);

            //interactable�� IInteractable �������̽��� �����ϰ� �ִ��� Ȯ�� -> Interact()�� �Ǵ� ������Ʈ���� Ȯ��
            if (interactable is IInteractable interactableObj)
            {
                if (interactable.InteractionIdList.Count == 0) interactableObj.Interact();  //����� ��ȣ�ۿ� ���� �ٷ� ����
                else CurrentInteractable.Add(interactableObj);  //����ڰ� ��ȣ�ۿ� �� �� �ִ� ����Ʈ�� �߰�
            }
        }

        //��ȣ�ۿ� ������ ���� ��ȣ�ۿ� ������Ʈ�� ��ȣ�� ������� �� ȣ��Ǵ� �޼���
        private void OnLostSignalInteraction(IHasInteractionIds interactable)
        {
            if (interactable.InteractionIdList.Count > 0) CurrentInteractionID.Remove(interactable);

            //interactable�� IInteractable �������̽��� �����ϰ� �ִ��� Ȯ��
            if (interactable is IInteractable interactableItem) CurrentInteractable.Remove(interactableItem);
        }
        #endregion


        //�÷��̾�� ���� ����� ��ȣ�ۿ� ������Ʈ ã��
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

        //��ȣ�ۿ� �Ϸ� �� ȣ��Ǵ� �޼���
        public void InteractComplete(IInteractable interactable)
        {
            CurrentInteractable.Remove(interactable);
            CurrentInteractionID.Remove(interactable as IHasInteractionIds);
        }
    }
}
