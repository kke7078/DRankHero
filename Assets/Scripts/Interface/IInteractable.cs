using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //IInteractable : ��ȣ�ۿ� ������ ������Ʈ�� �����ϴ� �������̽�
    public interface IInteractable
    {
        public void Interact();

        //��ȣ�ۿ� ������Ʈ�� ��ġ ��ȯ
        Transform GetTransform();
    }
}
