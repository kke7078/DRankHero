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

            //���ͷ��� �޽��� �ǽð� ����ȭ
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

            //HashSet : �ߺ��� ������� �ʴ� ���� �ڷᱸ��
            HashSet<InteractionData.MsgId> activeMsgID = new HashSet<InteractionData.MsgId>();

            foreach (IHasInteractionIds interactable in currentIds)
            {
                foreach (InteractionData.MsgId id in interactable.InteractionIdList)
                {
                    activeMsgID.Add(id);    //���� �����ִ� �޽����� ID�� ��� �߰�
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
                        break;  //�̹� Ȱ��ȭ �Ǿ������� ���� ID��
                    }
                    if (!msgObj.gameObject.activeSelf && inactiveMsg == null) inactiveMsg = msg; //��Ȱ��ȭ�� �޽��� ����
                }

                if (!foundActive)
                {
                    if (inactiveMsg != null)
                    {
                        inactiveMsg.InitMessage(id);
                        inactiveMsg.gameObject.SetActive(true); //��Ȱ��ȭ�� �޽��� Ȱ��ȭ
                        break;
                    }
                    else
                    {
                        var newMsg = Instantiate(interactionObjPref, interactionObjs).GetComponent<SetInteractionMsg>();
                        newMsg.InitMessage(id);    //�޽��� �ʱ�ȭ
                        newMsg.gameObject.SetActive(true); //Ȱ��ȭ
                    }
                }
            }

            foreach (Transform msgObj in interactionObjs)
            {
                SetInteractionMsg msg = msgObj.GetComponent<SetInteractionMsg>();

                if (!activeMsgID.Contains(msg.MsgId) && msgObj.gameObject.activeSelf) msgObj.gameObject.SetActive(false); //Ȱ��ȭ�� �޽��� �� ���� ID�� ���Ե��� �ʴ� �޽��� ��Ȱ��ȭ
            }
        }
    }
}
