﻿using System;
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
        [SerializeField] private PlayerCharacter player; //플레이어의 위치
        [SerializeField] private Animator stageStartUI; //스테이지 시작 UI
        [SerializeField] private Animator stageClearUI; //스테이지 클리어 UI
        [SerializeField] private Animator minimapUI; //미니맵 UI
        [SerializeField] private Animator remainingTimeUI; //남은 시간 UI
        [SerializeField] private Animator remainingRoomUI; //남은 장소 UI
        [SerializeField] private GameObject moveKeyUI;    //이동키 UI

        #region 레벨 시작 시 UI 세팅
        //레벨 시작 UI 표시
        public void StartStage()
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
            StartCoroutine(PlayUIAnimation(remainingTimeUI, true));

            //미니맵 UI 표시
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(PlayUIAnimation(minimapUI, true));

            //남은 장소 UI 표시
            StartCoroutine(PlayUIAnimation(remainingRoomUI, true));
            LayoutRebuilder.ForceRebuildLayoutImmediate(remainingRoomUI.GetComponent<RectTransform>());
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
        public void HideMoveKeyUI()
        {
            if(moveKeyUI.activeSelf) moveKeyUI.SetActive(false);
        }

        #region 레벨 클리어 시 UI 세팅
        public void ClearStage()
        {
            Vector3 targetPosition = new Vector3(0, 0.08f, 0);
            Vector3 targetRotation = new Vector3(0, -135, 0);
            StartCoroutine(player.SetPlayerTransform(targetPosition, targetRotation));
            StartCoroutine(HideNextUISequence());
        }

        IEnumerator HideNextUISequence()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(PlayUIAnimation(minimapUI, false));
            StartCoroutine(PlayUIAnimation(remainingTimeUI, false));
            StartCoroutine(PlayUIAnimation(remainingRoomUI, false));

            yield return new WaitForSeconds(0.4f);
            GameManager.Singleton.ClosedDoor(); //출입문 닫기
            yield return new WaitForSeconds(0.5f);
            stageClearUI.gameObject.SetActive(true); //스테이지 클리어 UI 활성화
        }

        private IEnumerator PlayUIAnimation(Animator targetUI, bool isPlay)
        {
            if (isPlay)
            {
                if (!targetUI.gameObject.activeSelf) targetUI.gameObject.SetActive(true);

                targetUI.SetBool("isShow", isPlay);
                targetUI.SetTrigger("showTrigger");
                targetUI.SetBool("isHide", !isPlay);
                targetUI.ResetTrigger("hideTrigger");
            }
            else
            {
                if (targetUI.gameObject.activeSelf)
                {
                    targetUI.SetBool("isShow", isPlay);
                    targetUI.ResetTrigger("showTrigger");
                    targetUI.SetBool("isHide", !isPlay);
                    targetUI.SetTrigger("hideTrigger");

                    yield return new WaitForSeconds(0.5f);
                    targetUI.gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}