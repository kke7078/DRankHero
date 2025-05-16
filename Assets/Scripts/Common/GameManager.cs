using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //GameManager : 게임의 전반적인 상태를 관리하는 클래스
    public class GameManager : SingletonBase<GameManager>
    {
        //게임 시작 유무
        public bool IsGameStarted
        {
            get { return isGameStarted; }
            set {
                isGameStarted = value;
                gameHUD.StartStage();
            }
        }
        private bool isGameStarted;

        //청소 완료 유무
        public bool IsCleanComplete
        {
            get { return isCleanComplete; }
            set {
                isCleanComplete = value;
                gameHUD.CleanComplete();
            }
        }
        private bool isCleanComplete;

        //게임 멈춤 유무
        public bool IsPause
        { 
            get => isPause;
            set {
                if (isPause == value) return;
                IsPause = value;
                TogglePause(IsPause);
            }
        }
        private bool isPause;
        public bool IsInDialogue { get; set; }

        //레벨 별 청소해야하는 방의 개수
        public int DirtyRoomCount {
            get => dirtyRoomCount;
            set {
                dirtyRoomCount = value;
                gameHUD.UpdatecompletedRoomsText();
            }
        }
        private int dirtyRoomCount;

        [SerializeField] private InteractionDoor startPointDoor;
        [SerializeField] private GameHUD gameHUD;
        [SerializeField] private float stageTimeLimit; //스테이지 시간 제한

        private float timeRemaining;                   //남은 시간


        public void Start()
        {
            Invoke("GameStart", 0.1f);

            InitializeDirtyRoomCount();
        }

        private void Update()
        {
            //타이머 UI 업데이트
            if (!isGameStarted) return;

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                gameHUD.UpdateTimerUI(timeRemaining);
            }
            else GameOver();
        }

        #region 게임의 전반적인 상태 담당 메서드
        private void GameStart() {
            startPointDoor.Interact();
        }

        public void GameClear()
        {
            Debug.Log("GameClear");
        }

        private void GameOver()
        { 
            
        }

        private void TogglePause(bool isPause)
        {
            if (isPause)
            { 
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        #endregion

        #region 게임의 레벨 담당 메서드
        private void InitializeDirtyRoomCount()
        {
            var dirtyRooms = GameObject.FindGameObjectsWithTag("MinimapDirtyRoomIcon");
            DirtyRoomCount = dirtyRooms.Length;
        }
        #endregion
    }
}
