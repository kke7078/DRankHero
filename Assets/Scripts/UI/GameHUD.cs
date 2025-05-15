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

        private float stageTimeLimit = 300f; //�������� �ð� ���� (5��)
        private float timeRemaining;        //���� �ð�

        private void Start()
        {
            timeRemaining = stageTimeLimit; //�������� �ð� ���� �ʱ�ȭ

            //�̺�Ʈ ���
            cleanRoomSensor.OnEnterRoom += OnEnterCleanRoom;
            cleanRoomSensor.OnStayRoom += OnStayCleanRoom;
            cleanRoomSensor.OnEixtRoom += OnExitCleanRoom;
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

            roomData.ColliderCount++;
            ShowCleanRoomGaugeBar(roomData);

            cleanRoomText.text = roomData.DirtyRoomName;
            UpdateGaugeValue(roomData);
        }

        public void OnStayCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            UpdateGaugeValue(roomData);

            if (roomData.DirtyCleanValue >= roomData.DirtyTotalValue)
            {
                roomData.IsComplete = true;

                roomData.ColliderCount = 0;
                ShowCleanRoomGaugeBar(roomData);
            }
        }

        public void OnExitCleanRoom(CleanRoom roomData)
        {
            if (roomData.IsComplete) return;

            roomData.ColliderCount--;
            ShowCleanRoomGaugeBar(roomData);
        }

        //û�� �������� UI ������Ʈ
        private void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGauge.fillAmount = roomData.DirtyCleanValue / roomData.DirtyTotalValue;
        }
        #endregion

        #region UI Updates
        //���� ��� UI ������Ʈ
        public void UpdatecompletedRoomsText()
        {
            var textComponent = cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>();
            int dirtyRoomCount = GameManager.Singleton.DirtyRoomCount;

            if (dirtyRoomCount > 0)
            {
                textComponent.text = $"������  <b><size=150%>{dirtyRoomCount}</size></b>���� ��Ҹ� ġ�켼��.";
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
        private void ShowCleanRoomGaugeBar(CleanRoom roomData)
        {
            bool isShow;
            if (roomData.ColliderCount != 0 && !roomData.IsComplete) isShow = true;
            else isShow = false;

            //���� �� û�� �Ϸ�
            if (roomData.IsComplete)
            {
                cleanRoomText.GetComponent<Animator>().SetTrigger("changeValue");
                StartCoroutine(DelayGaugeAnimation());
            }
            else ApplyGaugeAnimation(isShow);
        }

        private IEnumerator DelayGaugeAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            ApplyGaugeAnimation(false);
        }

        private void ApplyGaugeAnimation(bool isShow)
        {
            Animator animator = cleanRoomGaugeBar.GetComponent<Animator>();
            animator.SetBool("isShow", isShow);
            animator.SetBool("isHide", !isShow);

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
        private void TimerEnd()
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
