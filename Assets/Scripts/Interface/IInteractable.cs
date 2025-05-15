using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    [System.Serializable]
    public class InteractionInfo
    {
        public string interactKey;
        public string interactMsg;

        public InteractionInfo(string key, string msg)
        {
            interactKey = key;
            interactMsg = msg;
        }
    }

    //IInteractable : ��ȣ�ۿ� ������ ������Ʈ�� �����ϴ� �������̽�
    public interface IInteractable
    {
        List<InteractionInfo> InteractionInfos { get; }

        public void Interact(CharacterBase character);

        //��ȣ�ۿ� ������Ʈ�� ��ġ ��ȯ
        Transform GetTransform();
    }
}
