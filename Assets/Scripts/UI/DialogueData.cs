using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //DialogueData Ŭ���� : ��ȭ �����͸� �����ϴ� Ŭ����
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<DIalogueLine> lines; //��ȭ ����
    }
}
