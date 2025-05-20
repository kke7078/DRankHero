using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace KGY
{
    //ProjectorCollider : 프로젝터의 충돌을 관리하는 클래스
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

                // 물줄기 충돌 시 프로젝터의 Field of View를 줄여줌
                projector.fieldOfView = Mathf.Max(projector.fieldOfView - 7f * Time.deltaTime, 0.001f);
                if (projector.fieldOfView <= 0.001f)
                {
                    interactionSensor.CheckColliderExit(GetComponent<SphereCollider>());
                    projector.gameObject.SetActive(false);
                }

                var projectors = currentRoom.GetComponentsInChildren<Projector>();
                // 프로젝터의 Field of View를 모두 더하여 청소된 정도를 계산
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