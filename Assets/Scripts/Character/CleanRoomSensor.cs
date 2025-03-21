using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KGY
{
    //CleanRoomSensor Ŭ���� : û�ұ����� �����ϴ� ���� Ŭ����
    public class CleanRoomSensor : MonoBehaviour
    {
        public CleanRoom CurrentRoom { get; private set; }
        public System.Action<CleanRoom> OnEnterRoom;
        public System.Action<CleanRoom> OnStayRoom;
        public System.Action<CleanRoom> OnEixtRoom;

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out CleanRoom roomData)) {
                CurrentRoom = roomData;
                OnEnterRoom?.Invoke(CurrentRoom);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out CleanRoom roomData))
            {
                CurrentRoom = roomData;
                OnStayRoom?.Invoke(CurrentRoom);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CleanRoom roomData))
            {
                OnEixtRoom?.Invoke(CurrentRoom);
                CurrentRoom = null;
            }
        }
    }
}
