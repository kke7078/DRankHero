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
                if (isGameStarted == value) return;
                isGameStarted = value;
                GameHUD.StartLevel();
            }
        }
        private bool isGameStarted;

        //청소 완료 유무
        public bool IsCleanComplete
        {
            get => isCleanComplete;
            set {
                isCleanComplete = value;
                if (isCleanComplete) GameHUD.UpdatecompletedRoomsText(true); //청소 완료 UI 업데이트
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
                TogglePause(isGamePaused);
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
            remainingTime = MaxTime; //스테이지 남은 시간 초기화
            InitializeDirtyRoomCount(); //스테이지 남은 장소 초기화
            Invoke("StartGame", 0.1f);  //게임 시작
        }

        private void Update()
        {
            //타이머 UI 업데이트
            if (!IsGameStarted) return;

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

        public void EndGame(bool isClear)
        {
            if (isClear)
            {
                GameHUD.ClearLevel();
            }
            else
            { }
        }

        private void TogglePause(bool isPause)
        {
            if (IsGamePaused)
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

        public void ClosedDoor()
        {
            if (!startPointDoor.IsOpened) return;

            var animator = startPointDoor.GetComponent<Animator>();
            animator.SetBool("isClosed", true);
            animator.SetTrigger("slidingCloseTrigger");

            animator.SetBool("isOpen", false);
        }
    }
}
