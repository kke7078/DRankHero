using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace KGY
{
    //InteractionSensor Ŭ���� : ��ȣ�ۿ� ������ �Ӽ� �� ������ �����ϴ� Ŭ����
    public class InteractionSensor : MonoBehaviour
    {
        public System.Action<IHasInteractionIds> OnDetected; //��ȣ�ۿ� �������� ��ȣ�ۿ� �޽��� �����͸� �������� �� ȣ��Ǵ� �̺�Ʈ
        public System.Action<IHasInteractionIds> OnLostSignal; //��ȣ�ۿ� �������� ��ȣ�ۿ� �޽��� �����Ͱ� ������ �� ȣ��Ǵ� �̺�Ʈ

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out IHasInteractionIds interactable))
            {
                OnDetected?.Invoke(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out IHasInteractionIds interactable))
            {
                OnLostSignal?.Invoke(interactable);
            }
        }

        public void CheckColliderExit(Collider other)
        {
            OnTriggerExit(other);
        }
    }
}
