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
    //InteractionUI Ŭ���� : ��ȣ�ۿ� UI�� �Ӽ� �� ������ �����ϴ� Ŭ����
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private RectTransform interactionMsg;  //��ȣ�ۿ� �޽��� UI�� ��ġ
        [SerializeField] private GameObject interactionMsgPref; //��ȣ�ۿ� �޽��� UI ������
        [SerializeField] private CleanRoomGaugeUI cleanRoomGaugeUI;//û�� ������ �̸�
        [SerializeField] private Transform player;  //�÷��̾� ĳ����
        [SerializeField] private Camera mainCamera; //���� ī�޶�

        private Vector3 worldOffset = new Vector3(0, -1.8f, 0);

        private void Update()
        {
            if (player == null || mainCamera == null) return;

            Vector3 worldPos = player.position + worldOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos); //������Ʈ ��ġ�� ȭ�� ��ǥ�� ��ȯ

            interactionMsg.position = screenPos;

            CheckInteractionMsg();  
        }

        //��ȣ�ۿ� �޽��� üũ
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

            //HashSet : �ߺ��� ������� �ʴ� ���� �ڷᱸ��
            HashSet<InteractionData.MsgId> activeMsgID = new HashSet<InteractionData.MsgId>();

            foreach (IHasInteractionIds interactable in currentIds)
            {
                foreach (InteractionData.MsgId id in interactable.InteractionIdList)
                {
                    if (id != InteractionData.MsgId.none) activeMsgID.Add(id);    //���� �����ִ� �޽����� ID�� ��� �߰�
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
                        break;  //�̹� Ȱ��ȭ �Ǿ������� ���� ID��
                    }
                    if (!msgObj.gameObject.activeSelf && inactiveMsg == null) inactiveMsg = msg; //��Ȱ��ȭ�� �޽��� ����
                }

                if (!foundActive)
                {
                    if (inactiveMsg != null)
                    {
                        inactiveMsg.InitMessage(id);//�޽��� �ʱ�ȭ
                        inactiveMsg.gameObject.SetActive(true); //��Ȱ��ȭ�� �޽��� Ȱ��ȭ
                        LayoutRebuilder.ForceRebuildLayoutImmediate(inactiveMsg.GetComponent<RectTransform>()); //���̾ƿ� ���� �籸��
                        break;
                    }
                    else
                    {
                        var newMsg = Instantiate(interactionMsgPref, interactionMsg).GetComponent<SetInteractionMsg>(); //���ο� �޽��� ������ ����
                        newMsg.InitMessage(id);    
                        newMsg.gameObject.SetActive(true);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(newMsg.GetComponent<RectTransform>());
                    }
                }
            }

            foreach (Transform msgObj in interactionMsg)
            {
                SetInteractionMsg msg = msgObj.GetComponent<SetInteractionMsg>();

                if (!activeMsgID.Contains(msg.MsgId) && msgObj.gameObject.activeSelf) msgObj.gameObject.SetActive(false); //Ȱ��ȭ�� �޽��� �� ���� ID�� ���Ե��� �ʴ� �޽��� ��Ȱ��ȭ
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
            else ApplyGaugeAnimation(true);
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

        //CleanGauge �̸� ����
        public void SetGaugeBarName(string roomName)
        {
            cleanRoomGaugeUI.SetGaugeBarName(roomName);
        }

        //CleanGauge �� ������Ʈ
        public void UpdateGaugeValue(CleanRoom roomData)
        {
            cleanRoomGaugeUI.UpdateGaugeValue(roomData);
        }
    }
}
