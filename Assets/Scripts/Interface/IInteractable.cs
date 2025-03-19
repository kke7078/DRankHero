using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //IInteractable : ��ȣ�ۿ� ������ ������Ʈ�� �����ϴ� �������̽�
    public interface IInteractable
    {
        public bool IsAutoInteract { get; }

        public bool IsOffLimit { get; }

        public string InteractionMsg { get; }

        public void Interact(CharacterBase character);

        //��ȣ�ۿ� ������Ʈ�� ��ġ ��ȯ
        Transform GetTransform();
    }
}
