using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //CleanRoom 클래스 : 청소구역을 나타내는 클래스
    public class CleanRoom : MonoBehaviour
    {
        public bool IsComplete
        {
            get { return isComplete; }
            set {
                isComplete = value;
                if (isComplete) {
                    //Complete 파티클 효과 재생
                    Debug.Log("띠로롱~~");
                }
            }
        }
        public bool isComplete;

        public string dirtyRoomName;
        public float dirtyTotalValue;
        public float dirtyCleanValue;

        private void Start()
        {
            for (int i = 0; i < GetComponentsInChildren<Projector>().Length; i++)
            {
                dirtyTotalValue += GetComponentsInChildren<Projector>()[i].fieldOfView;
            }
        }
    }
}
