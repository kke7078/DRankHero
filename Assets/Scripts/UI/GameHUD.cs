using System;
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
        [SerializeField] private Animator stageStartUI; //스테이지 시작 UI
        [SerializeField] private Animator minimapUI; //미니맵 UI
        [SerializeField] private Animator remainingTimeUI; //남은 시간 UI
        [SerializeField] private Animator remainingRoomUI; //남은 장소 UI
        [SerializeField] private GameObject moveKey;    //이동키 UI

        #region 레벨 시작 시 UI 세팅
        //스테이지 시작 UI 표시
        public void StartLevel()
        {
            stageStartUI.SetTrigger("showTrigger");
            StartCoroutine(ShowNextUISequence());   //stageStartUI가 사라진 후 다음 UI 표시
        }

        //미니맵, 남은시간, 남은장소 UI  표시
        IEnumerator ShowNextUISequence()
        {
            yield return new WaitForSeconds(1f);
            stageStartUI.SetTrigger("hideTrigger");

            //남은 시간 UI 표시
            remainingTimeUI.SetTrigger("showTrigger");

            //미니맵 UI 표시
            yield return new WaitForSeconds(0.3f);
            minimapUI.SetTrigger("showTrigger");

            //남은 장소 UI 표시
            remainingRoomUI.gameObject.SetActive(true);
        }

        //남은 시간 UI 업데이트
        public void UpdateTimerUI(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            remainingTimeUI.GetComponent<TextMeshProUGUI>().text = $"{minutes:0}:{seconds:00}";
        }

        //남은 장소 UI 업데이트
        public void UpdatecompletedRoomsText(bool isComplete)
        {
            var textComponent = remainingRoomUI.GetComponentInChildren<TextMeshProUGUI>();
            int dirtyRoomCount = GameManager.Singleton.DirtyRoomCount;

            if (isComplete)
            {
                remainingRoomUI.SetTrigger("changeValue");

                var minimap = minimapUI.GetComponent<MinimapIndicator>();
                minimap.exitIcon.gameObject.SetActive(true);
                minimap.exitTransform.gameObject.SetActive(true);
            }
            else textComponent.text = $"더러운  <b><size=150%>{dirtyRoomCount}</size></b>개의 장소를 치우세요.";
        }
        #endregion

        //이동키 UI 숨김
        public void HideMoveKey()
        {
            if(moveKey.activeSelf) moveKey.SetActive(false);
        }
    }
}






















//    public CleanRoomSensor cleanRoomSensor;
//    public RectTransform cleanRoomGaugeBar;
//    public TextMeshProUGUI cleanRoomText;
//    public Image cleanRoomGauge;

//    public DialogueUI dialogueUI;

//    private void Start()
//    {
//        //이벤트 등록
//        cleanRoomSensor.OnEnterRoom += OnEnterCleanRoom;
//        cleanRoomSensor.OnStayRoom += OnStayCleanRoom;
//        cleanRoomSensor.OnEixtRoom += OnExitCleanRoom;
//    }

//    #region Room Events
//    public void OnEnterCleanRoom(CleanRoom roomData)
//    {
//        if (roomData.IsComplete) return;

//        roomData.ColliderCount++;
//        ShowCleanRoomGaugeBar(roomData);

//        cleanRoomText.text = roomData.DirtyRoomName;
//        UpdateGaugeValue(roomData);
//    }

//    public void OnStayCleanRoom(CleanRoom roomData)
//    {
//        if (roomData.IsComplete) return;

//        UpdateGaugeValue(roomData);

//        if (roomData.DirtyCleanValue >= roomData.DirtyTotalValue)
//        {
//            roomData.IsComplete = true;

//            roomData.ColliderCount = 0;
//            ShowCleanRoomGaugeBar(roomData);
//        }
//    }

//    public void OnExitCleanRoom(CleanRoom roomData)
//    {
//        if (roomData.IsComplete) return;

//        roomData.ColliderCount--;
//        ShowCleanRoomGaugeBar(roomData);
//    }

//    //청소 게이지바 UI 업데이트
//    private void UpdateGaugeValue(CleanRoom roomData)
//    {
//        cleanRoomGauge.fillAmount = roomData.DirtyCleanValue / roomData.DirtyTotalValue;
//    }
//    #endregion

//    #region UI Updates




//    //청소 게이지바 UI 표시/숨김
//    private void ShowCleanRoomGaugeBar(CleanRoom roomData)
//    {
//        bool isShow;
//        if (roomData.ColliderCount != 0 && !roomData.IsComplete) isShow = true;
//        else isShow = false;

//        //현재 룸 청소 완료
//        if (roomData.IsComplete)
//        {
//            cleanRoomText.GetComponent<Animator>().SetTrigger("changeValue");
//            StartCoroutine(DelayGaugeAnimation());
//        }
//        else ApplyGaugeAnimation(isShow);
//    }

//    private IEnumerator DelayGaugeAnimation()
//    {
//        yield return new WaitForSeconds(0.5f);
//        ApplyGaugeAnimation(false);
//    }

//    private void ApplyGaugeAnimation(bool isShow)
//    {
//        Animator animator = cleanRoomGaugeBar.GetComponent<Animator>();
//        animator.SetBool("isShow", isShow);
//        animator.SetBool("isHide", !isShow);

//        if (isShow)
//        {
//            animator.SetTrigger("showTrigger");
//            animator.ResetTrigger("hideTrigger");
//        }
//        else
//        {
//            animator.ResetTrigger("showTrigger");
//            animator.SetTrigger("hideTrigger");
//        }
//    }





//    //타이머 종료 (게임 오버)
//    private void TimerEnd()
//    {
//        Debug.Log("게임 오버");
//    }


//    #endregion
//}