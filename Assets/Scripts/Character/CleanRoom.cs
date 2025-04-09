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
                    //게임HUD의 청소한 방 개수 감소
                    GameManager.Singleton.gameHUD.CleanRoomCount--;

                    //미니맵 아이콘 비활성화
                    minimapIcon.gameObject.SetActive(false);

                    //청소 완료된 방의 프로젝터 삭제
                    Destroy(projectors);
                }
            }
        }
        private bool isComplete;

        public string dirtyRoomName;
        public float dirtyTotalValue;
        public float dirtyCleanValue;
        public Canvas minimapIcon;
        public GameObject projectors;

        private void Start()
        {
            for (int i = 0; i < GetComponentsInChildren<Projector>().Length; i++)
            {
                dirtyTotalValue += GetComponentsInChildren<Projector>()[i].fieldOfView;
            }
        }
    }
}
