using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //InteractionSensor 클래스 : 상호작용 센서의 속성 및 동작을 정의하는 클래스
    public class InteractionSensor : MonoBehaviour
    {
        public System.Action<IInteractable> OnDetected; //상호작용 센서에서 상호작용 가능한 오브젝트를 감지했을 때 호출되는 이벤트
        public System.Action<IInteractable> OnLostSignal; //상호작용 센서에서 상호작용 가능한 오브젝트가 빠졌을 때 호출되는 이벤트

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
