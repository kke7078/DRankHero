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

    //IInteractable : 상호작용 가능한 오브젝트를 관리하는 인터페이스
    public interface IInteractable
    {
        List<InteractionInfo> InteractionInfos { get; }

        public void Interact(CharacterBase character);

        //상호작용 오브젝트의 위치 반환
        Transform GetTransform();
    }
}
