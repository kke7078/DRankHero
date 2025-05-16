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
        public InteractionType interactionType; //��ȣ�ۿ� Ÿ��
        public string key;
        public Sprite icon;
        public string msg;
    }

    public class InteractionMsg : MonoBehaviour
    {
        public IReadOnlyList<InteractionData> InteractionDatas => interactionDatas; //��ȣ�ۿ� ������ ����Ʈ �б� ���� �Ӽ�
        [SerializeField] private List<InteractionData> interactionDatas = new List<InteractionData>(); //��ȣ�ۿ� ������ ����Ʈ
    }
}
