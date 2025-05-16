using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //GameManager : ������ �������� ���¸� �����ϴ� Ŭ����
    public class GameManager : SingletonBase<GameManager>
    {
        //���� ���� ����
        public bool IsGameStarted
        {
            get { return isGameStarted; }
            set {
                isGameStarted = value;
                gameHUD.StartStage();
            }
        }
        private bool isGameStarted;

        //û�� �Ϸ� ����
        public bool IsCleanComplete
        {
            get { return isCleanComplete; }
            set {
                isCleanComplete = value;
                gameHUD.CleanComplete();
            }
        }
        private bool isCleanComplete;

        //���� ���� ����
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

        //���� �� û���ؾ��ϴ� ���� ����
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
        [SerializeField] private float stageTimeLimit; //�������� �ð� ����

        private float timeRemaining;                   //���� �ð�


        public void Start()
        {
            Invoke("GameStart", 0.1f);

            InitializeDirtyRoomCount();
        }

        private void Update()
        {
            //Ÿ�̸� UI ������Ʈ
            if (!isGameStarted) return;

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                gameHUD.UpdateTimerUI(timeRemaining);
            }
            else GameOver();
        }

        #region ������ �������� ���� ��� �޼���
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

        #region ������ ���� ��� �޼���
        private void InitializeDirtyRoomCount()
        {
            var dirtyRooms = GameObject.FindGameObjectsWithTag("MinimapDirtyRoomIcon");
            DirtyRoomCount = dirtyRooms.Length;
        }
        #endregion
    }
}
