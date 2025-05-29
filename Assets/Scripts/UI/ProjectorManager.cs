using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    public class ProjectorManager : MonoBehaviour, IHasInteractionIds
    {
        [SerializeField] private CleanToolManager.ToolType cleanToolType;

        public List<InteractionData.MsgId> InteractionIdList
        {
            get => interactionIdList;
            private set => interactionIdList = value;
        }
        [SerializeField] private List<InteractionData.MsgId> interactionIdList = new List<InteractionData.MsgId>();

        private readonly Dictionary<CleanToolManager.ToolType, (InteractionData.MsgId success, InteractionData.MsgId fail)> toolMessages =
            new Dictionary<CleanToolManager.ToolType, (InteractionData.MsgId, InteractionData.MsgId)>
        {
            { CleanToolManager.ToolType.WaterTank, (InteractionData.MsgId.projectorWater, InteractionData.MsgId.projectorWaterError) },
        };

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCharacter player))
            { 
                CompareTool(player.CurrentTool.CurrentToolType);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out PlayerCharacter player))
            {
                CompareTool(player.CurrentTool.CurrentToolType);
            }
        }

        private void CompareTool(CleanToolManager.ToolType currentTool)
        {
            if (InteractionIdList.Count == 0) InteractionIdList.Add(InteractionData.MsgId.none);
            if (!toolMessages.TryGetValue(cleanToolType, out var messages)) return;

            var newMsg = (currentTool == cleanToolType) ? messages.success : messages.fail;
            if (InteractionIdList[0] != newMsg) InteractionIdList[0] = newMsg;
        }
    }
}
