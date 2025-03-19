using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //IInteractable : 상호작용 가능한 오브젝트를 관리하는 인터페이스
    public interface IInteractable
    {
        public bool IsAutoInteract { get; }

        public bool IsOffLimit { get; }

        public string InteractionMsg { get; }

        public void Interact(CharacterBase character);

        //상호작용 오브젝트의 위치 반환
        Transform GetTransform();
    }
}
