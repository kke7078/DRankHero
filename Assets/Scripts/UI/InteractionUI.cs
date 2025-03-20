using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace KGY
{
    //InteractionUI Ŭ���� : ��ȣ�ۿ� UI�� �Ӽ� �� ������ �����ϴ� Ŭ����
    public class InteractionUI : MonoBehaviour
    {
        public Transform interactionObj;    //��ȣ�ۿ� Obj�� ��ġ
        public TextMeshProUGUI interactionText;    //��ȣ�ۿ� �ؽ�Ʈ
        public CharacterBase player;        //�÷��̾� ĳ����
        public Camera mainCamera;           //���� ī�޶�

        private void Update()
        {
            if (player == null) return;

            //������Ʈ ��ġ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position + new Vector3(0, -1f, 0));

            //ȭ�� ��ǥ�� �ٽ� ���� ��ǥ�� ��ȯ�ؼ� UI ��ġ
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, GetComponent<Canvas>().planeDistance));
            interactionObj.position = worldPos;
        }

        public void ShowUI(IInteractable interactable)
        {
            if (!interactable.IsAutoInteract)
            {
                //����ȯ : ������Ʈ�� ���� ��
                if (interactable is InteractionDoor door) {
                    if (door.isOpened || door.isKeepOut) return;
                }

                    interactionText.text = interactable.InteractionMsg;

                //�θ� ���̾ƿ��� ������ ����
                RectTransform parentRect = interactionText.GetComponent<RectTransform>().parent as RectTransform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

                //alpha���� 1�� �÷� ���̰� �����
                interactionObj.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        public void HideUI()
        {
            //alpha���� 0���� ���� �� ���̰� �����
            interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
