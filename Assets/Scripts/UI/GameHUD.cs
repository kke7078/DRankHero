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

        private string mapObjectName = "Map";
        private float stageTimeLimit = 300f; //스테이지 시간 제한 (5분)
        private float completedRooms = 0;   //청소한 방의 개수
        private float timeRemaining; //남은 시간
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
            timeRemaining = stageTimeLimit; //스테이지 시간 제한 초기화

            //이벤트 등록
            cleanRoomSensor.OnEnterRoom += OnEnterCleanRoom;
            cleanRoomSensor.OnStayRoom += OnStayCleanRoom;
            cleanRoomSensor.OnEixtRoom += OnExitCleanRoom;

            map = GameObject.Find(mapObjectName);
            CountCleanRooms();
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

        //청소 게이지바 UI 업데이트
        private void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGauge.fillAmount = roomData.dirtyTotalValue == 0 ? 0 : roomData.dirtyCleanValue / roomData.dirtyTotalValue;
        }
        #endregion

        #region UI Updates
        //청소한 방 개수 업데이트
        private void CountCleanRooms()
        {
            int count = 0;
            foreach (var canvas in map.GetComponentsInChildren<Canvas>())
            {
                if (canvas.name == "MinimapCleanRoomIcon") count++;
            }

            completedRooms = count;
        }

        //남은 장소 UI 업데이트
        private void UpdateCleanRoomText()
        {
            var textComponent = cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>();
            if (completedRooms > 0)
            {
                cleanRoomUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("더러운  <b><size=150%>{0:0}</size></b>개의 장소를 치우세요.", completedRooms);
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
        public void TimerEnd()
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
