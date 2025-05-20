using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace KGY
{
    //ProjectorCollider : ���������� �浹�� �����ϴ� Ŭ����
    public class ProjectorCollider : MonoBehaviour, IHasInteractionIds
    {
        [SerializeField] private CleanToolManager.ToolType toolType;

        public List<InteractionData.MsgId> InteractionIdList
        {
            get => interactionIdList;
            set => interactionIdList = value;
        }
        [SerializeField] private List<InteractionData.MsgId> interactionIdList = new List<InteractionData.MsgId>();

        private CleanRoom currentRoom;
        private Projector projector;
        [SerializeField] Transform projectors;
        [SerializeField] InteractionSensor interactionSensor;

        private void Start()
        {
            currentRoom = GetComponentInParent<CleanRoom>();
            projector = GetComponentInParent<Projector>();
        }


        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("WaterRipple"))
            {
                float currentFOV = 0;

                // ���ٱ� �浹 �� ���������� Field of View�� �ٿ���
                projector.fieldOfView = Mathf.Max(projector.fieldOfView - 7f * Time.deltaTime, 0.001f);
                if (projector.fieldOfView <= 0.001f)
                {
                    interactionSensor.CheckColliderExit(GetComponent<SphereCollider>());
                    projector.gameObject.SetActive(false);
                }

                var projectors = currentRoom.GetComponentsInChildren<Projector>();
                // ���������� Field of View�� ��� ���Ͽ� û�ҵ� ������ ���
                for (int i = 0; i < projectors.Length; i++)
                {
                    currentFOV += projectors[i].fieldOfView;
                }

                currentRoom.DirtyCleanValue = currentRoom.DirtyTotalValue - currentFOV;
            }
            
            if (other.TryGetComponent(out PlayerCharacter player)) {

                var currentTool = player.CurrentTool.CurrentToolType;

                switch (toolType)
                {
                    case CleanToolManager.ToolType.WaterTank:
                        if (currentTool == toolType)
                        {
                            foreach (Transform child in projectors)
                            {
                                ProjectorCollider pc = child.GetComponentInChildren<ProjectorCollider>();
                                if (pc.InteractionIdList[0] != InteractionData.MsgId.projectorWater) pc.InteractionIdList[0] = InteractionData.MsgId.projectorWater;
                            }
                        }
                        else
                        {
                            foreach (Transform child in projectors)
                            {
                                ProjectorCollider pc = child.GetComponentInChildren<ProjectorCollider>();
                                if (pc.InteractionIdList[0] != InteractionData.MsgId.projectorWaterError) pc.InteractionIdList[0] = InteractionData.MsgId.projectorWaterError;
                            }
                        }
                        break;
                }
            }
        }
    }
}