using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    public enum InteractionType
    { 
        KeyCode,
        Icon,
    }

    [System.Serializable]
    public struct InteractionData
    {
        public InteractionType interactionType; //상호작용 타입
        public string key;
        public Sprite icon;
        public string msg;
    }

    public class InteractionMsg : MonoBehaviour
    {
        public IReadOnlyList<InteractionData> InteractionDatas => interactionDatas; //상호작용 데이터 리스트 읽기 전용 속성
        [SerializeField] private List<InteractionData> interactionDatas = new List<InteractionData>(); //상호작용 데이터 리스트
    }
}
