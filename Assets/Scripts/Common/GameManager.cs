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

        public InteractionDoor startPointDoor;
        public GameHUD gameHUD;

        public void Start()
        {
            Invoke("GameStart", 0.1f);
        }

        public void GameStart() {
            startPointDoor.Interact(PlayerCharacter.instance);
            startPointDoor.isOpened = true;
        }
    }
}
