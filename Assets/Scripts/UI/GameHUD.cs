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
        public Animator miniMapUI;
        public Animator timeLimitUI;
        public Animator cleanRoomUI;
        public DialogueUI dialogueUI;

        private float stageTimeLimit = 300f; //스테이지 시간 제한 (5분)
        private float timeRemaining;        //남은 시간

        private void Start()
        {
            timeRemaining = stageTimeLimit; //스테이지 시간 제한 초기화

            //이벤트 등록
            cleanRoomSensor.OnEnterRoom += OnEnterCleanRoom;
            cleanRoomSensor.OnStayRoom += OnStayCleanRoom;
            cleanRoomSensor.OnEixtRoom += OnExitCleanRoom;
        }

        private void Update()
        {
            //타이머 UI 업데이트
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

        //청소 게이지바 UI 업데이트
        private void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGauge.fillAmount = roomData.DirtyCleanValue / roomData.DirtyTotalValue;
        }
        #endregion

        #region UI Updates
        //남은 장소 UI 업데이트
        public void UpdatecompletedRoomsText()
        {
            var textComponent = cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>();
            int dirtyRoomCount = GameManager.Singleton.DirtyRoomCount;

            if (dirtyRoomCount > 0)
            {
                textComponent.text = $"더러운  <b><size=150%>{dirtyRoomCount}</size></b>개의 장소를 치우세요.";
            }
            else GameManager.Singleton.IsCleanComplete = true;
        }

        //남은 시간 UI 업데이트
        private void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timeLimitUI.GetComponent<TextMeshProUGUI>().text = $"{minutes:0}:{seconds:00}";
        }

        //청소 게이지바 UI 표시/숨김
        private void ShowCleanRoomGaugeBar(CleanRoom roomData)
        {
            bool isShow;
            if (roomData.ColliderCount != 0 && !roomData.IsComplete) isShow = true;
            else isShow = false;

            //현재 룸 청소 완료
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

        //스테이지 시작 UI 표시
        public void StartStage()
        {
            stageStart.SetTrigger("showTrigger");
            StartCoroutine(HideStageStartSequence());
        }

        //시간, 맵, 장소 UI  표시
        IEnumerator HideStageStartSequence()
        {
            yield return new WaitForSeconds(1f);
            stageStart.SetTrigger("hideTrigger");

            //남은 시간 UI 표시
            timeLimitUI.SetTrigger("showTrigger");

            //미니맵 UI 표시
            yield return new WaitForSeconds(0.3f);
            miniMapUI.SetTrigger("showTrigger");

            //남은 장소 UI 표시
            cleanRoomUI.gameObject.SetActive(true);
        }

        //타이머 종료 (게임 오버)
        private void TimerEnd()
        {
            Debug.Log("게임 오버");
        }

        //청소 완료!
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
