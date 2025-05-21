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
    //GameHUD Ŭ���� : ���� ȭ���� HUD�� ��Ÿ���� Ŭ����
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private Animator stageStartUI; //�������� ���� UI
        [SerializeField] private Animator minimapUI; //�̴ϸ� UI
        [SerializeField] private Animator remainingTimeUI; //���� �ð� UI
        [SerializeField] private Animator remainingRoomUI; //���� ��� UI
        [SerializeField] private GameObject moveKey;    //�̵�Ű UI

        #region ���� ���� �� UI ����
        //�������� ���� UI ǥ��
        public void StartLevel()
        {
            stageStartUI.SetTrigger("showTrigger");
            StartCoroutine(ShowNextUISequence());   //stageStartUI�� ����� �� ���� UI ǥ��
        }

        //�̴ϸ�, �����ð�, ������� UI  ǥ��
        IEnumerator ShowNextUISequence()
        {
            yield return new WaitForSeconds(1f);
            stageStartUI.SetTrigger("hideTrigger");

            //���� �ð� UI ǥ��
            remainingTimeUI.SetTrigger("showTrigger");

            //�̴ϸ� UI ǥ��
            yield return new WaitForSeconds(0.3f);
            minimapUI.SetTrigger("showTrigger");

            //���� ��� UI ǥ��
            remainingRoomUI.gameObject.SetActive(true);
        }

        //���� �ð� UI ������Ʈ
        public void UpdateTimerUI(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            remainingTimeUI.GetComponent<TextMeshProUGUI>().text = $"{minutes:0}:{seconds:00}";
        }

        //���� ��� UI ������Ʈ
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
            else textComponent.text = $"������  <b><size=150%>{dirtyRoomCount}</size></b>���� ��Ҹ� ġ�켼��.";
        }
        #endregion

        //�̵�Ű UI ����
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
//        //�̺�Ʈ ���
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

//    //û�� �������� UI ������Ʈ
//    private void UpdateGaugeValue(CleanRoom roomData)
//    {
//        cleanRoomGauge.fillAmount = roomData.DirtyCleanValue / roomData.DirtyTotalValue;
//    }
//    #endregion

//    #region UI Updates




//    //û�� �������� UI ǥ��/����
//    private void ShowCleanRoomGaugeBar(CleanRoom roomData)
//    {
//        bool isShow;
//        if (roomData.ColliderCount != 0 && !roomData.IsComplete) isShow = true;
//        else isShow = false;

//        //���� �� û�� �Ϸ�
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





//    //Ÿ�̸� ���� (���� ����)
//    private void TimerEnd()
//    {
//        Debug.Log("���� ����");
//    }


//    #endregion
//}