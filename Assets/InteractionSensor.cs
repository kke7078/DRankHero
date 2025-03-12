using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //InteractionSensor Ŭ���� : ��ȣ�ۿ� ������ �Ӽ� �� ������ �����ϴ� Ŭ����
    public class InteractionSensor : MonoBehaviour
    {
        public System.Action<IInteractable> OnDetected; //��ȣ�ۿ� �������� ��ȣ�ۿ� ������ ������Ʈ�� �������� �� ȣ��Ǵ� �̺�Ʈ
        public System.Action<IInteractable> OnLostSignal; //��ȣ�ۿ� �������� ��ȣ�ۿ� ������ ������Ʈ�� ������ �� ȣ��Ǵ� �̺�Ʈ

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out IInteractable interactable))
            {
                OnDetected?.Invoke(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out IInteractable interactable))
            {
                OnLostSignal?.Invoke(interactable);
            }
        }
    }
}
