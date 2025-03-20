using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace KGY
{
    //InteractionUI 클래스 : 상호작용 UI의 속성 및 동작을 정의하는 클래스
    public class InteractionUI : MonoBehaviour
    {
        public Transform interactionObj;    //상호작용 Obj의 위치
        public TextMeshProUGUI interactionText;    //상호작용 텍스트
        public CharacterBase player;        //플레이어 캐릭터
        public Camera mainCamera;           //메인 카메라

        private void Update()
        {
            if (player == null) return;

            //오브젝트 위치를 화면 좌표로 전환
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position + new Vector3(0, -1f, 0));

            //화면 좌표를 다시 월드 좌표로 변환해서 UI 배치
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, GetComponent<Canvas>().planeDistance));
            interactionObj.position = worldPos;
        }

        public void ShowUI(IInteractable interactable)
        {
            if (!interactable.IsAutoInteract)
            {
                //형변환 : 오브젝트가 문일 때
                if (interactable is InteractionDoor door) {
                    if (door.isOpened || door.isKeepOut) return;
                }

                    interactionText.text = interactable.InteractionMsg;

                //부모 레이아웃을 강제로 갱신
                RectTransform parentRect = interactionText.GetComponent<RectTransform>().parent as RectTransform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

                //alpha값을 1로 올려 보이게 만들기
                interactionObj.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        public void HideUI()
        {
            //alpha값을 0으로 내려 안 보이게 만들기
            interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
