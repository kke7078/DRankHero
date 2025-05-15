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
        public System.Action<IInteractable> OnDetected; //��ȣ�ۿ� �������� ��ȣ�ۿ� ������ ������Ʈ�� �������� �� ȣ��Ǵ� �̺�Ʈ
        public System.Action<IInteractable> OnLostSignal; //��ȣ�ۿ� �������� ��ȣ�ۿ� ������ ������Ʈ�� ������ �� ȣ��Ǵ� �̺�Ʈ

        public bool IsShowCleanInteractMsg { get; set; } = false;   //û�� ������ ������Ʈ�� �����Ǿ����� ����

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out IInteractable interactable))
            {
                OnDetected?.Invoke(interactable);
            }

            if (other.CompareTag("DirtyObj")) IsShowCleanInteractMsg = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("DirtyObj")) IsShowCleanInteractMsg = true;
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out IInteractable interactable))
            {
                OnLostSignal?.Invoke(interactable);
            }

            if (other.CompareTag("DirtyObj")) IsShowCleanInteractMsg = false;
        }
    }
}
