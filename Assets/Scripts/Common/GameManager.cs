using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //GameManager : 게임의 전반적인 상태를 관리하는 클래스
    public class GameManager : SingletonBase<GameManager>
    {
        public bool IsGameStarted
        {
            get { return isGameStarted; }
            set {
                isGameStarted = value;
                gameHUD.StartStage();
            }
        }
        private bool isGameStarted;

        public InteractionDoor startPointDoor;
        public GameHUD gameHUD;

        public void Start()
        {
            Invoke("GameStart", 0.1f);
        }

        public void GameStart() {
            startPointDoor.Interact(null);
            startPointDoor.isOpened = true;
        }
    }
}
