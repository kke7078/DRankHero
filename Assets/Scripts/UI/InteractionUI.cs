using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace KGY
{
    //InteractionUI 클래스 : 상호작용 UI의 속성 및 동작을 정의하는 클래스
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private Transform interactionObjs;     //상호작용 Obj의 위치
        [SerializeField] private GameObject interactionObjPref;
        [SerializeField] private PlayerCharacter player;        //플레이어 캐릭터
        [SerializeField] private Camera mainCamera;             //메인 카메라

        private void Update()
        {
            if (player == null) return;

            //오브젝트 위치를 화면 좌표로 전환
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position + new Vector3(0, -1.8f, 0));

            //화면 좌표를 다시 월드 좌표로 변환해서 UI 배치
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, GetComponent<Canvas>().planeDistance));
            interactionObjs.position = worldPos;
        }

        public void ShowInteractionMsg(IHasInteractionIds interactable) 
        {
            /*
             [오브젝트 마주침]
                    ↓
                [인터랙션 메시지 ID 확인]
                    ↓
                [현재 띄워진 메시지 중 동일 ID 있는가?]
                    ├─▶ 있음
                    │    ├─ 활성화 상태? → return
                    │    └─ 비활성화 상태 → 초기화 후 활성화
                    │
                    └─▶ 없음
                         ├─ 비활성 메시지 있음 → 내용 세팅 후 활성화
                         └─ 없음
                              ├─ 최대 개수 초과 아님 → 프리팹 복제 후 세팅+활성화
                              └─ 초과 → 가장 오래된 메시지 제거 또는 무시
             */

            foreach (InteractionData.MsgId id in interactable.InteractionIdList)
            {
                SetInteractionMsg sameIdMsg = null;
                SetInteractionMsg inactiveMsg = null;

                //같은 ID의 메시지가 있는지 확인
                foreach (Transform child in interactionObjs)
                {
                    SetInteractionMsg msg = child.GetComponent<SetInteractionMsg>();

                    if (msg.MsgId == id)
                    {
                        sameIdMsg = msg;
                        msg.InitMessage(id);
                        break;
                    }
                    else
                    {
                        if (!child.gameObject.activeSelf && inactiveMsg == null) inactiveMsg = msg;
                    }
                }

                //같은 ID의 메시지가 있을 때
                if (sameIdMsg != null)
                {
                    if (sameIdMsg.gameObject.activeSelf) return;    //이미 활성화 상태라면 리턴
                    else
                    {
                        sameIdMsg.gameObject.SetActive(true);          //비활성화 상태라면 활성화 후 종료
                        return;
                    }
                }

                //같은 ID의 메시지가 없을 때, 비활성화 메시지가 있을 때
                if (inactiveMsg != null)
                {
                    inactiveMsg.InitMessage(id);    //메시지 초기화
                    inactiveMsg.gameObject.SetActive(true); //활성화
                    Debug.Log(inactiveMsg);
                    return;
                }
                //같은 ID의 메시지가 없고, 비활성화 메시지가 없을 때
                else
                { 
                    var newMsg = Instantiate(interactionObjPref, interactionObjs).GetComponent<SetInteractionMsg>();
                    newMsg.InitMessage(id);    //메시지 초기화
                    newMsg.gameObject.SetActive(true); //활성화
                    return;
                }
            }
        }

        public void HideInteractionUI(IHasInteractionIds interactable)
        {

        }




















        private void UpdateInteractionUI()
        {
            //플레이어에게 상호작용 가능한 오브젝트가 있을 때
            //if (PlayerCharacter.instance.closestInteractable != null) ShowInteractionUI(PlayerCharacter.instance.closestInteractable);
        }

        private void CheckInteractObj(Transform parent)
        {
            int count = 0;
            foreach (Transform child in parent.transform)
            {
                CanvasGroup cg = child.GetComponent<CanvasGroup>();
                if (cg != null && cg.alpha == 0f) count++;
            }
        }

        //public void ShowInteractionUI(IInteractable interactable)
        //{
        //    //if (!interactable.IsAutoInteract)
        //    //{
        //    //    //형변환 : 오브젝트가 문일 때
        //    //    if (interactable is InteractionDoor door)
        //    //    {
        //    //        if (door.isOpened) return;
        //    //    }
        //    //}
        //}

        //public void ShowInteractionUI(IInteractable interactable)
        //{
        //    if (!interactable.IsAutoInteract)
        //    {
        //        
                
        //        interactionText.text = interactable.InteractionMsg;

        //        //부모 레이아웃을 강제로 갱신
        //        RectTransform parentRect = interactionText.GetComponent<RectTransform>().parent as RectTransform;
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

        //        //alpha값을 1로 올려 보이게 만들기
        //        interactionText.GetComponentInParent<CanvasGroup>().alpha = 1;
        //        //interactionObj.GetComponent<CanvasGroup>().alpha = 1;
        //    }
        //}

        //public void HideInteractionUI()
        //{
        //    //alpha값을 0으로 내려 안 보이게 만들기
        //    interactionText.GetComponentInParent<CanvasGroup>().alpha = 0;
        //    //interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        //}
    }
}
