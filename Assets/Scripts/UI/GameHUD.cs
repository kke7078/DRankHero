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
        public Animator miniMapUI;
        public Animator timeLimitUI;
        public Animator cleanRoomUI;
        public DialogueUI dialogueUI;

        private string mapObjectName = "Map";
        private float stageTimeLimit = 300f; //�������� �ð� ���� (5��)
        private float completedRooms = 0;   //û���� ���� ����
        private float timeRemaining; //���� �ð�
        private bool isGaugeBarShow;
        private GameObject map;

        public float CleanRoomCount {
            get { return completedRooms; }
            set {
                completedRooms = value;
                UpdateCleanRoomText();
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

        private void Start()
        {
            timeRemaining = stageTimeLimit; //�������� �ð� ���� �ʱ�ȭ

            //�̺�Ʈ ���
            cleanRoomSensor.OnEnterRoom += OnEnterCleanRoom;
            cleanRoomSensor.OnStayRoom += OnStayCleanRoom;
            cleanRoomSensor.OnEixtRoom += OnExitCleanRoom;

            map = GameObject.Find(mapObjectName);
            CountCleanRooms();
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

        
        
        #region Room Events
        public void OnEnterCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            IsGaugeBarShow = true;
            cleanRoomText.text = roomData.dirtyRoomName;
            UpdateGaugeValue(roomData);
        }

        public void OnStayCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            UpdateGaugeValue(roomData);

            if (roomData.dirtyTotalValue >= roomData.dirtyCleanValue)
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

        //û�� �������� UI ������Ʈ
        private void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGauge.fillAmount = roomData.dirtyTotalValue == 0 ? 0 : roomData.dirtyCleanValue / roomData.dirtyTotalValue;
        }
        #endregion

        #region UI Updates
        //û���� �� ���� ������Ʈ
        private void CountCleanRooms()
        {
            int count = 0;
            foreach (var canvas in map.GetComponentsInChildren<Canvas>())
            {
                if (canvas.name == "MinimapCleanRoomIcon") count++;
            }

            completedRooms = count;
        }

        //���� ��� UI ������Ʈ
        private void UpdateCleanRoomText()
        {
            var textComponent = cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>();
            if (completedRooms > 0)
            {
                cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("������  <b><size=150%>{0:0}</size></b>���� ��Ҹ� ġ�켼��.", completedRooms);
            }
            else GameManager.Singleton.IsCleanComplete = true;
        }

        //���� �ð� UI ������Ʈ
        private void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timeLimitUI.GetComponent<TextMeshProUGUI>().text = $"{minutes:0}:{seconds:00}";
        }

        //û�� �������� UI ǥ��/����
        public void ShowCleanRoomGaugeBar(bool isShow)
        {
            Animator animator = cleanRoomGaugeBar.GetComponent<Animator>();
            animator.SetBool("isShow", isShow);
            cleanRoomGaugeBar.GetComponent<Animator>().SetBool("isHide", !isShow);

            if (isShow)
            {
                animator.SetTrigger("showTrigger");
                animator.ResetTrigger("hideTrigger");
            }
            else
            {
                animator.ResetTrigger("showTrigger");
                animator.SetTrigger("hideTrigger");
            }
        }

        //�������� ���� UI ǥ��
        public void StartStage()
        {
            stageStart.SetTrigger("showTrigger");
            StartCoroutine(HideStageStartSequence());
        }

        //�ð�, ��, ��� UI  ǥ��
        IEnumerator HideStageStartSequence()
        {
            yield return new WaitForSeconds(1f);
            stageStart.SetTrigger("hideTrigger");

            //���� �ð� UI ǥ��
            timeLimitUI.SetTrigger("showTrigger");

            //�̴ϸ� UI ǥ��
            yield return new WaitForSeconds(0.3f);
            miniMapUI.SetTrigger("showTrigger");

            //���� ��� UI ǥ��
            cleanRoomUI.gameObject.SetActive(true);
        }

        //Ÿ�̸� ���� (���� ����)
        public void TimerEnd()
        {
            Debug.Log("���� ����");
        }

        //û�� �Ϸ�!
        public void CleanComplete()
        {
            cleanRoomUI.SetTrigger("changeValue");

            var minimap = miniMapUI.GetComponent<MinimapIndicator>();
            minimap.exitIcon.gameObject.SetActive(true);
            minimap.exitTransform.gameObject.SetActive(true);
        }
        #endregion
    }
}
