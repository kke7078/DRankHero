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
                if (isGameStarted == value) return;
                isGameStarted = value;
                GameHUD.StartLevel();
            }
        }
        private bool isGameStarted;

        //û�� �Ϸ� ����
        public bool IsCleanComplete
        {
            get => isCleanComplete;
            set {
                isCleanComplete = value;
                if (isCleanComplete) GameHUD.UpdatecompletedRoomsText(true); //û�� �Ϸ� UI ������Ʈ
            }
        }
        private bool isCleanComplete;

        //���� ���� ����
        public bool IsPause
        { 
            get => isPause;
            set {
                if (isPause == value) return;
                isPause = value;
                TogglePause(isPause);
            }
        }
        private bool isPause;

        //��ȭ �� ����
        public bool IsInDialogue { get; set; }

        //���� �� û���ؾ��ϴ� ���� ����
        public int DirtyRoomCount {
            get => dirtyRoomCount;
            set {
                dirtyRoomCount = value;
                if (dirtyRoomCount == 0) IsCleanComplete = true;
                else GameHUD.UpdatecompletedRoomsText(false); //û�� �̿Ϸ� UI ������Ʈ
            }
        }
        private int dirtyRoomCount;

        //���� HUD
        public GameHUD GameHUD => gameHUD;
        [SerializeField] private GameHUD gameHUD;

        [SerializeField] private InteractionDoor startPointDoor;

        private const float MaxTime = 300f; //�������� ���ѽð�
        private float remainingTime;    //���� �ð�

        public void Start()
        {
            remainingTime = MaxTime; //�������� ���� �ð� �ʱ�ȭ
            InitializeDirtyRoomCount(); //�������� ���� ��� �ʱ�ȭ
            Invoke("StartGame", 0.1f);  //���� ����
        }

        private void Update()
        {
            //Ÿ�̸� UI ������Ʈ
            if (!IsGameStarted) return;

            if (remainingTime > 0)
            {
                remainingTime = Mathf.Max(remainingTime - Time.deltaTime, 0f);
                GameHUD.UpdateTimerUI(remainingTime);
            }
            else EndGame(false);
        }

        #region ������ �������� ���� ��� �޼���
        private void StartGame() {
            startPointDoor.Interact();
        }

        public void EndGame(bool isClear)
        {
            if (isClear)
            {
                Debug.Log("���� Ŭ����!");
            }
            else
            { }
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
