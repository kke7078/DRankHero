using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

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
                    //게임HUD의 청소한 방 개수 감소
                    GameManager.Singleton.DirtyRoomCount--;

                    //미니맵 아이콘 비활성화
                    minimapIcon.gameObject.SetActive(false);
                }
            }
        }
        private bool isComplete;

        public float ColliderCount { get; set; }

        public string DirtyRoomName => dirtyRoomName;
        [SerializeField] private string dirtyRoomName;

        public float DirtyTotalValue { get; private set; } = 0f;
        public float DirtyCleanValue { get; set; } = 0f;

        [SerializeField] private Canvas minimapIcon;

        private void Start()
        {
            var projectors = GetComponentsInChildren<Projector>();
            for (int i = 0; i < projectors.Length; i++)
            {
                DirtyTotalValue += projectors[i].fieldOfView;
            }
        }
    }
}
