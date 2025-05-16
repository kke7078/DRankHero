using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //IInteractable : 상호작용 가능한 오브젝트를 관리하는 인터페이스
    public interface IInteractable
    {
        public void Interact();

        //상호작용 오브젝트의 위치 반환
        Transform GetTransform();
    }
}
