using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //GameHUD Ŭ���� : ���� ȭ���� HUD�� ��Ÿ���� Ŭ����
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
        private float cleanRoomCount = 0;   //û���� ���� ����
        private float timeRemaining = 300f; //���� �ð�
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

            cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("������ <b><size=150%>{0:0}</size></b>���� ��Ҹ� ġ�켼��.", cleanRoomCount); //���� ��� UI ǥ��
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
            //�������� ���� UI ǥ��
            stageStart.SetTrigger("showTrigger");
            StartCoroutine("StartStageHide");
        }

        IEnumerator StartStageHide()
        {
            yield return new WaitForSeconds(1f);
            stageStart.SetTrigger("hideTrigger");

            //�̴ϸ� UI ǥ��
            miniMap.SetTrigger("showTrigger");

            //���� �ð� UI ǥ��
            timeLimit.SetTrigger("showTrigger");

            //���� ��� UI ǥ��
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
            //Ÿ�̸� ����
            Debug.Log("���� ����");
        }
    }
}
