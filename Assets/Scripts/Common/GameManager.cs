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
        

        public void Start()
        {
            Invoke("GameStart", 0.1f);

            InitializeDirtyRoomCount();
        }

        #region ������ �������� ���� ��� �޼���
        private void GameStart() {
            startPointDoor.Interact(PlayerCharacter.instance);
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
