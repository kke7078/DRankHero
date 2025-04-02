using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

        private bool isShow;

        private void Start()
        {
            cleanRoomSensor.OnEnterRoom += (CleanRoom roomData) => OnEnterCleanRoom(roomData);
            cleanRoomSensor.OnStayRoom += (CleanRoom roomData) => OnStayCleanRoom(roomData);
            cleanRoomSensor.OnEixtRoom += (CleanRoom roomData) => OnExitCleanRoom(roomData);
        }

        private void Update()
        {
            Vector2 showPosition = isShow ? new Vector2(cleanRoomGaugeBar.anchoredPosition.x, 20f) : new Vector2(cleanRoomGaugeBar.anchoredPosition.x, -80f);
            cleanRoomGaugeBar.anchoredPosition = Vector2.Lerp(cleanRoomGaugeBar.anchoredPosition, showPosition, Time.deltaTime * 10f);
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
            if (roomData.isComplete) return;

            cleanRoomGauge.fillAmount = roomData.dirtyCleanValue / roomData.dirtyTotalValue;

            if (roomData.dirtyTotalValue == roomData.dirtyCleanValue) isShow = false;
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

            //미니맵 UI 표시
            miniMap.SetTrigger("showTrigger");
        }

        IEnumerator StartStageHide()
        {
            yield return new WaitForSeconds(1.5f);
            stageStart.SetTrigger("hideTrigger");
            yield return new WaitForSeconds(1.5f);
            stageStart.gameObject.SetActive(false);
        }
    }
}
