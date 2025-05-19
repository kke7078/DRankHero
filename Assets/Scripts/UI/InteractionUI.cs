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
    //InteractionUI Ŭ���� : ��ȣ�ۿ� UI�� �Ӽ� �� ������ �����ϴ� Ŭ����
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private Transform interactionObjs;     //��ȣ�ۿ� Obj�� ��ġ
        [SerializeField] private GameObject interactionObjPref;
        [SerializeField] private PlayerCharacter player;        //�÷��̾� ĳ����
        [SerializeField] private Camera mainCamera;             //���� ī�޶�

        private void Update()
        {
            if (player == null) return;

            //������Ʈ ��ġ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position + new Vector3(0, -1.8f, 0));

            //ȭ�� ��ǥ�� �ٽ� ���� ��ǥ�� ��ȯ�ؼ� UI ��ġ
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, GetComponent<Canvas>().planeDistance));
            interactionObjs.position = worldPos;
        }

        public void ShowInteractionMsg(IHasInteractionIds interactable) 
        {
            /*
             [������Ʈ ����ħ]
                    ��
                [���ͷ��� �޽��� ID Ȯ��]
                    ��
                [���� ����� �޽��� �� ���� ID �ִ°�?]
                    ������ ����
                    ��    ���� Ȱ��ȭ ����? �� return
                    ��    ���� ��Ȱ��ȭ ���� �� �ʱ�ȭ �� Ȱ��ȭ
                    ��
                    ������ ����
                         ���� ��Ȱ�� �޽��� ���� �� ���� ���� �� Ȱ��ȭ
                         ���� ����
                              ���� �ִ� ���� �ʰ� �ƴ� �� ������ ���� �� ����+Ȱ��ȭ
                              ���� �ʰ� �� ���� ������ �޽��� ���� �Ǵ� ����
             */

            foreach (InteractionData.MsgId id in interactable.InteractionIdList)
            {
                SetInteractionMsg sameIdMsg = null;
                SetInteractionMsg inactiveMsg = null;

                //���� ID�� �޽����� �ִ��� Ȯ��
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

                //���� ID�� �޽����� ���� ��
                if (sameIdMsg != null)
                {
                    if (sameIdMsg.gameObject.activeSelf) return;    //�̹� Ȱ��ȭ ���¶�� ����
                    else
                    {
                        sameIdMsg.gameObject.SetActive(true);          //��Ȱ��ȭ ���¶�� Ȱ��ȭ �� ����
                        return;
                    }
                }

                //���� ID�� �޽����� ���� ��, ��Ȱ��ȭ �޽����� ���� ��
                if (inactiveMsg != null)
                {
                    inactiveMsg.InitMessage(id);    //�޽��� �ʱ�ȭ
                    inactiveMsg.gameObject.SetActive(true); //Ȱ��ȭ
                    Debug.Log(inactiveMsg);
                    return;
                }
                //���� ID�� �޽����� ����, ��Ȱ��ȭ �޽����� ���� ��
                else
                { 
                    var newMsg = Instantiate(interactionObjPref, interactionObjs).GetComponent<SetInteractionMsg>();
                    newMsg.InitMessage(id);    //�޽��� �ʱ�ȭ
                    newMsg.gameObject.SetActive(true); //Ȱ��ȭ
                    return;
                }
            }
        }

        public void HideInteractionUI(IHasInteractionIds interactable)
        {

        }




















        private void UpdateInteractionUI()
        {
            //�÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� ���� ��
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
        //    //    //����ȯ : ������Ʈ�� ���� ��
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

        //        //�θ� ���̾ƿ��� ������ ����
        //        RectTransform parentRect = interactionText.GetComponent<RectTransform>().parent as RectTransform;
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

        //        //alpha���� 1�� �÷� ���̰� �����
        //        interactionText.GetComponentInParent<CanvasGroup>().alpha = 1;
        //        //interactionObj.GetComponent<CanvasGroup>().alpha = 1;
        //    }
        //}

        //public void HideInteractionUI()
        //{
        //    //alpha���� 0���� ���� �� ���̰� �����
        //    interactionText.GetComponentInParent<CanvasGroup>().alpha = 0;
        //    //interactionObj.GetComponent<CanvasGroup>().alpha = 0;
        //}
    }
}
