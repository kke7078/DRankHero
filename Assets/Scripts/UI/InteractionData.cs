using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KGY
{
    //InteractionEntry 클래스 : 상호작용 UI의 메시지 항목을 정의하는 클래스
    [CreateAssetMenu(menuName = "Data/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        public enum MsgId
        {
            none,
            stageStartDoor,
            projectorWater,
            projectorWaterError,
        }

        [System.Serializable]
        public class InteractionEntry
        {
            public MsgId msgId;
            public string key;
            public Sprite icon;
            public string msg;
        }

        [SerializeField] private List<InteractionEntry> interactionEntries;

        private Dictionary<MsgId, InteractionEntry> interactionDict;

        private void Init()
        {
            if (interactionDict == null) interactionDict = interactionEntries.ToDictionary(entry => entry.msgId, entry => entry);
        }

        public string GetKey(MsgId msgId)
        {
            Init();
            return interactionDict.TryGetValue(msgId, out var entry) ? entry.key : null;
        }

        public Sprite GetIcon(MsgId msgId)
        {
            Init();
            return interactionDict.TryGetValue(msgId, out var entry) ? entry.icon : null;
        }

        public string GetMessage(MsgId msgId)
        {
            Init();
            return interactionDict.TryGetValue(msgId, out var entry) ? entry.msg : null;
        }

    }
}
