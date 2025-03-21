using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KGY
{
    //CleanRoomSensor 클래스 : 청소구역을 감지하는 센서 클래스
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
