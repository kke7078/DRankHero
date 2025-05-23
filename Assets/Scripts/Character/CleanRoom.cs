using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace KGY
{
    //CleanRoom 클래스 : 청소구역을 나타내는 클래스
    public class CleanRoom : MonoBehaviour
    {
        //방의 청소 완료 상태
        public bool IsComplete
        {
            get { return isComplete; }
            set {
                isComplete = value;
                if (isComplete) {
                    //게임HUD의 청소한 방 개수 감소
                    GameManager.Singleton.DirtyRoomCount--;

                    //미니맵 아이콘 비활성화
                    minimapIcon.gameObject.SetActive(false);
                }
            }
        }
        private bool isComplete;

        //방의 이름
        public string DirtyRoomName => dirtyRoomName;
        [SerializeField] private string dirtyRoomName;

        public float DirtyTotalValue { get; private set; } = 0f;    //방의 청소해야할 값
        public float DirtyCleanValue { get; set; } = 0f;    //방의 청소된 값

        public float ColliderCount { get; private set; } = 0f;  //청소할 방의 콜라이더 개수

        [SerializeField] private InteractionUI interactionUI;
        [SerializeField] private Canvas minimapIcon;


        private void Start()
        {
            var projectors = GetComponentsInChildren<Projector>();
            for (int i = 0; i < projectors.Length; i++)
            {
                DirtyTotalValue += projectors[i].fieldOfView;
            }
        }

        #region OnTrigger
        //플레이어가 청소구역에 들어왔을 때
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (IsComplete) return;

                ColliderCount++;
                interactionUI.SetGaugeBarName(DirtyRoomName);
                interactionUI.ShowCleanRoomGaugeUI(this);
                interactionUI.UpdateGaugeValue(this);
            }
        }

        //플레이어가 청소구역에서 나갔을 때
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            { 
                if(IsComplete) return;

                ColliderCount--;
                interactionUI.ShowCleanRoomGaugeUI(this);
            }
        }
        #endregion
    }
}
