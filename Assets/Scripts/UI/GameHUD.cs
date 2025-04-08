using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //GameHUD 클래스 : 게임 화면의 HUD를 나타내는 클래스
    public class GameHUD : MonoBehaviour
    {
        public CleanRoomSensor cleanRoomSensor;
        public RectTransform cleanRoomGaugeBar;
        public TextMeshProUGUI cleanRoomText;
        public Image cleanRoomGauge;
        public Animator stageStart;
        public Animator miniMap;
        public Animator timeLimit;
        public Animator cleanRoomUI;


        private GameObject map;
        private float cleanRoomCount = 0;   //청소한 방의 개수
        private float timeRemaining = 300f; //남은 시간
        private bool isShow;

        private void Start()
        {
            cleanRoomSensor.OnEnterRoom += (CleanRoom roomData) => OnEnterCleanRoom(roomData);
            cleanRoomSensor.OnStayRoom += (CleanRoom roomData) => OnStayCleanRoom(roomData);
            cleanRoomSensor.OnEixtRoom += (CleanRoom roomData) => OnExitCleanRoom(roomData);

            map = GameObject.Find("Map");
            for (int i = 0; i < map.GetComponentsInChildren<Canvas>().Length; i++)
            {
                if(map.GetComponentsInChildren<Canvas>()[i].name == "MinimapCleanRoomIcon") 
                {
                    cleanRoomCount++;
                }
            }

            cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("더러운 <b><size=150%>{0:0}</size></b>개의 장소를 치우세요.", cleanRoomCount); //남은 장소 UI 표시
        }

        private void Update()
        {
            Vector2 showPosition = isShow ? new Vector2(cleanRoomGaugeBar.anchoredPosition.x, 20f) : new Vector2(cleanRoomGaugeBar.anchoredPosition.x, -80f);
            cleanRoomGaugeBar.anchoredPosition = Vector2.Lerp(cleanRoomGaugeBar.anchoredPosition, showPosition, Time.deltaTime * 10f);

            if (GameManager.Singleton.IsGameStarted) {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    UpdateTimerUI();
                }
                else TimerEnd();
            }
        }

        public void OnEnterCleanRoom(CleanRoom roomData)
        {
            if (roomData.isComplete) return;

            isShow = true;
            cleanRoomText.text = roomData.dirtyRoomName;
            cleanRoomGauge.fillAmount = roomData.dirtyCleanValue / roomData.dirtyTotalValue;
        }

        public void OnStayCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            cleanRoomGauge.fillAmount = roomData.dirtyCleanValue / roomData.dirtyTotalValue;

            if (roomData.dirtyTotalValue == roomData.dirtyCleanValue)
            {
                roomData.IsComplete = true;
                isShow = false;
            }
        }

        public void OnExitCleanRoom(CleanRoom roomData)
        {
            if (roomData.isComplete) return;

            isShow = false;
            cleanRoomText.text = "";
            cleanRoomGauge.fillAmount = 0;
        }

        public void StartStage() {
            //스테이지 시작 UI 표시
            stageStart.SetTrigger("showTrigger");
            StartCoroutine("StartStageHide");
        }

        IEnumerator StartStageHide()
        {
            yield return new WaitForSeconds(1f);
            stageStart.SetTrigger("hideTrigger");

            //미니맵 UI 표시
            miniMap.SetTrigger("showTrigger");

            //남은 시간 UI 표시
            timeLimit.SetTrigger("showTrigger");

            //남은 장소 UI 표시
            cleanRoomUI.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            stageStart.gameObject.SetActive(false);
        }

        public void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            string timerText = string.Format("{0:0}:{1:00}", minutes, seconds);
            timeLimit.GetComponent<TextMeshProUGUI>().text = timerText;
        }

        public void TimerEnd() {
            //타이머 종료
            Debug.Log("게임 오버");
        }
    }
}
