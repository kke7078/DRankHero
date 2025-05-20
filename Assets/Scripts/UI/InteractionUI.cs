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

            //인터렉션 메시지 실시간 동기화
            CheckInteractionMsg();
        }

        private void CheckInteractionMsg()
        {
            var currentIds = InteractionManager.Singleton.CurrentInteractionIds;           

            if (currentIds.Count <= 0)
            {
                foreach (Transform msgObj in interactionObjs)
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
                    activeMsgID.Add(id);    //현재 들어와있는 메시지의 ID를 모두 추가
                }
            }

            foreach (var id in activeMsgID)
            {
                bool foundActive = false;
                SetInteractionMsg inactiveMsg = null;

                foreach (Transform msgObj in interactionObjs)
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
                        inactiveMsg.InitMessage(id);
                        inactiveMsg.gameObject.SetActive(true); //비활성화된 메시지 활성화
                        break;
                    }
                    else
                    {
                        var newMsg = Instantiate(interactionObjPref, interactionObjs).GetComponent<SetInteractionMsg>();
                        newMsg.InitMessage(id);    //메시지 초기화
                        newMsg.gameObject.SetActive(true); //활성화
                    }
                }
            }

            foreach (Transform msgObj in interactionObjs)
            {
                SetInteractionMsg msg = msgObj.GetComponent<SetInteractionMsg>();

                if (!activeMsgID.Contains(msg.MsgId) && msgObj.gameObject.activeSelf) msgObj.gameObject.SetActive(false); //활성화된 메시지 중 현재 ID에 포함되지 않는 메시지 비활성화
            }
        }
    }
}
