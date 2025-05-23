using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace KGY
{
    //InteractionUI 클래스 : 상호작용 UI의 속성 및 동작을 정의하는 클래스
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private RectTransform interactionMsg;  //상호작용 메시지 UI의 위치
        [SerializeField] private GameObject interactionMsgPref; //상호작용 메시지 UI 프리팹
        [SerializeField] private CleanRoomGaugeUI cleanRoomGaugeUI;//청소 게이지 이름
        [SerializeField] private Transform player;  //플레이어 캐릭터
        [SerializeField] private Camera mainCamera; //메인 카메라

        private Vector3 worldOffset = new Vector3(0, -1.8f, 0);

        private void Update()
        {
            if (player == null || mainCamera == null) return;

            Vector3 worldPos = player.position + worldOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos); //오브젝트 위치를 화면 좌표로 전환

            interactionMsg.position = screenPos;

            CheckInteractionMsg();  
        }

        //상호작용 메시지 체크
        private void CheckInteractionMsg()
        {
            var currentIds = InteractionManager.Singleton.CurrentInteractionID;

            if (currentIds.Count <= 0)
            {
                foreach (Transform msgObj in interactionMsg)
                {
                    if (msgObj.gameObject.activeSelf) msgObj.gameObject.SetActive(false);
                }
                return;
            }

            //HashSet : 중복을 허용하지 않는 집합 자료구조
            HashSet<InteractionData.MsgId> activeMsgID = new HashSet<InteractionData.MsgId>();

            foreach (IHasInteractionIds interactable in currentIds)
            {
                foreach (InteractionData.MsgId id in interactable.InteractionIdList)
                {
                    if (id != InteractionData.MsgId.none) activeMsgID.Add(id);    //현재 들어와있는 메시지의 ID를 모두 추가
                }
            }

            foreach (var id in activeMsgID)
            {
                bool foundActive = false;
                SetInteractionMsg inactiveMsg = null;

                foreach (Transform msgObj in interactionMsg)
                {
                    SetInteractionMsg msg = msgObj.GetComponent<SetInteractionMsg>();

                    if (msg.MsgId == id && msgObj.gameObject.activeSelf)
                    {
                        foundActive = true;
                        break;  //이미 활성화 되어있으면 다음 ID로
                    }
                    if (!msgObj.gameObject.activeSelf && inactiveMsg == null) inactiveMsg = msg; //비활성화된 메시지 저장
                }

                if (!foundActive)
                {
                    if (inactiveMsg != null)
                    {
                        inactiveMsg.InitMessage(id);//메시지 초기화
                        inactiveMsg.gameObject.SetActive(true); //비활성화된 메시지 활성화
                        LayoutRebuilder.ForceRebuildLayoutImmediate(inactiveMsg.GetComponent<RectTransform>()); //레이아웃 강제 재구성
                        break;
                    }
                    else
                    {
                        var newMsg = Instantiate(interactionMsgPref, interactionMsg).GetComponent<SetInteractionMsg>(); //새로운 메시지 프리팹 생성
                        newMsg.InitMessage(id);    
                        newMsg.gameObject.SetActive(true);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(newMsg.GetComponent<RectTransform>());
                    }
                }
            }

            foreach (Transform msgObj in interactionMsg)
            {
                SetInteractionMsg msg = msgObj.GetComponent<SetInteractionMsg>();

                if (!activeMsgID.Contains(msg.MsgId) && msgObj.gameObject.activeSelf) msgObj.gameObject.SetActive(false); //활성화된 메시지 중 현재 ID에 포함되지 않는 메시지 비활성화
            }
        }

        #region CleanGauge Show/Hide
        public void ShowCleanRoomGaugeUI(CleanRoom roomData)
        {
            if (roomData.IsComplete)
            {
                cleanRoomGaugeUI.ChangeNameAnimation();
                StartCoroutine(DelayHideGaugeAnimation());
            }
            else
            {
                if (roomData.ColliderCount > 0) ApplyGaugeAnimation(true);
                else ApplyGaugeAnimation(false);
            }
        }

        private IEnumerator DelayHideGaugeAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            ApplyGaugeAnimation(false);
        }

        private void ApplyGaugeAnimation(bool isShow)
        {
            Animator animator = cleanRoomGaugeUI.GetComponent<Animator>();
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
        #endregion

        //CleanGauge 이름 설정
        public void SetGaugeBarName(string roomName)
        {
            cleanRoomGaugeUI.SetGaugeBarName(roomName);
        }

        //CleanGauge 값 업데이트
        public void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGaugeUI.UpdateGaugeValue(roomData);
        }
    }
}
