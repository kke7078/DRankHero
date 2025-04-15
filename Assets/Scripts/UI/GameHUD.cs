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
        public DialogueUI dialogueUI;

        public float CleanRoomCount {
            get { return cleanRoomCount; }
            set {
                cleanRoomCount = value;

                if (cleanRoomCount != 0)
                {
                    cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("������  <b><size=150%>{0:0}</size></b>���� ��Ҹ� ġ�켼��.", cleanRoomCount); //���� ��� UI ǥ��
                }
                else
                {
                    cleanRoomUI.SetTrigger("changeValue");
                }
            }
        }
        public bool IsGaugeBarShow
        {
            get { return isGaugeBarShow; }
            set
            {
                isGaugeBarShow = value;
                ShowCleanRoomGaugeBar(isGaugeBarShow);
            }
        }

        private GameObject map;
        private float cleanRoomCount = 0;   //û���� ���� ����
        private float timeRemaining = 300f; //���� �ð�
        private bool isGaugeBarShow;

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
                    CleanRoomCount++;
                }
            }
        }

        private void Update()
        {
            //Ÿ�̸� UI ������Ʈ
            if (GameManager.Singleton.IsGameStarted)
            {
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
            if (roomData.IsComplete) return;

            IsGaugeBarShow = true;
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
                IsGaugeBarShow = false;
            }
        }

        public void OnExitCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            IsGaugeBarShow = false;
        }

        //û�� �������� UI ǥ��/����
        public void ShowCleanRoomGaugeBar(bool isShow)
        {
            cleanRoomGaugeBar.GetComponent<Animator>().SetBool("isShow", isShow);
            cleanRoomGaugeBar.GetComponent<Animator>().SetBool("isHide", !isShow);

            //�������� UI ǥ��
            if (isShow)
            {
                cleanRoomGaugeBar.GetComponent<Animator>().SetTrigger("showTrigger");
                cleanRoomGaugeBar.GetComponent<Animator>().ResetTrigger("hideTrigger");
            }

            //�������� UI ����
            else 
            {
                cleanRoomGaugeBar.GetComponent<Animator>().ResetTrigger("showTrigger");
                cleanRoomGaugeBar.GetComponent<Animator>().SetTrigger("hideTrigger");
            }
        }

        //�������� ���� UI ǥ��
        public void StartStage() {
            //�������� ���� UI ǥ��
            stageStart.SetTrigger("showTrigger");
            StartCoroutine("StartStageHide");
        }

        //�������� ���� UI ����
        IEnumerator StartStageHide()
        {
            yield return new WaitForSeconds(1f);
            stageStart.SetTrigger("hideTrigger");

            //���� �ð� UI ǥ��
            timeLimit.SetTrigger("showTrigger");

            yield return new WaitForSeconds(0.3f);

            //�̴ϸ� UI ǥ��
            miniMap.SetTrigger("showTrigger");

            //���� ��� UI ǥ��
            cleanRoomUI.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            stageStart.gameObject.SetActive(false);
        }

        //���� �ð� UI ������Ʈ
        public void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            string timerText = string.Format("{0:0}:{1:00}", minutes, seconds);
            timeLimit.GetComponent<TextMeshProUGUI>().text = timerText;
        }

        //Ÿ�̾� ����
        public void TimerEnd() {
            //Ÿ�̸� ����
            Debug.Log("���� ����");
        }

        //Dialog UI ǥ��

    }
}
