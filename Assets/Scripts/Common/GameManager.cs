using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //GameManager : 게임의 전반적인 상태를 관리하는 클래스
    public class GameManager : SingletonBase<GameManager>
    {
        //스테이지 시작 유무 -> 스테이지 시작 문이 열림
        public bool IsStageStarted
        {
            get { return isStageStarted; }
            set {
                if (isStageStarted == value) return;
                isStageStarted = value;
                GameHUD.StartStage();
            }
        }
        private bool isStageStarted;

        //청소 완료 유무
        public bool IsCleanComplete
        {
            get => isCleanComplete;
            set {
                isCleanComplete = value;
                if (isCleanComplete)
                {
                    GameHUD.UpdatecompletedRoomsText(true); //청소 완료 UI 업데이트
                    StartGame();    //startPoint 문 열기
                }
            }
        }
        private bool isCleanComplete;

        //게임 멈춤 유무
        public bool IsGamePaused
        { 
            get => isGamePaused;
            set {
                if (isGamePaused == value) return;
                isGamePaused = value;
            }
        }
        private bool isGamePaused;

        //대화 중 유무
        public bool IsCharacterMovementLocked { get; set; }

        //레벨 별 청소해야하는 방의 개수
        public int DirtyRoomCount {
            get => dirtyRoomCount;
            set {
                dirtyRoomCount = value;
                if (dirtyRoomCount == 0) IsCleanComplete = true;
                else GameHUD.UpdatecompletedRoomsText(false); //청소 미완료 UI 업데이트
            }
        }
        private int dirtyRoomCount;

        //게임 HUD
        public GameHUD GameHUD => gameHUD;
        [SerializeField] private GameHUD gameHUD;

        [SerializeField] private InteractionDoor startPointDoor;

        private const float MaxTime = 300f; //스테이지 제한시간
        private float remainingTime;    //남은 시간

        public void Start()
        {
            InputSystem.Singleton.onGamePause += PauseGame; //게임 일시정지

            remainingTime = MaxTime; //스테이지 남은 시간 초기화
            InitializeDirtyRoomCount(); //스테이지 남은 장소 초기화
            Invoke("StartGame", 0.1f);  //게임 시작
        }

        private void Update()
        {
            //타이머 UI 업데이트
            if (!IsStageStarted) return;

            if (remainingTime > 0)
            {
                remainingTime = Mathf.Max(remainingTime - Time.deltaTime, 0f);
                GameHUD.UpdateTimerUI(remainingTime);
            }
            else EndGame(false);
        }

        #region 게임의 전반적인 상태 담당 메서드
        public void StartGame() {
            startPointDoor.Interact();
        }

        public void PauseGame()
        {
            IsGamePaused = !IsGamePaused;   //게임 일시정지 토글

            if (IsGamePaused) Time.timeScale = 0f;
            else Time.timeScale = 1f;
        }

        public void EndGame(bool isClear)
        {
            if (isClear)
            {
                GameHUD.ClearStage();
            }
            else
            { }
        }
        #endregion

        #region 게임의 레벨 담당 메서드
        private void InitializeDirtyRoomCount()
        {
            var dirtyRooms = GameObject.FindGameObjectsWithTag("MinimapDirtyRoomIcon");
            DirtyRoomCount = dirtyRooms.Length;
        }
        #endregion

        public void ClosedDoor()
        {
            if (!startPointDoor.IsOpened) return;

            startPointDoor.IsOpened = false;

            var animator = startPointDoor.GetComponent<Animator>();
            animator.SetBool("isClosed", true);
            animator.SetTrigger("slidingCloseTrigger");

            animator.SetBool("isOpen", false);
            animator.ResetTrigger("slidingOpenTrigger");
        }
    }
}
