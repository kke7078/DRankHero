using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KGY
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;  //���ϴ� ���
        [TextArea] public string dialogueText; //���ϴ� ����
        public int portraitIndex; //ĳ������ �̹��� �ε���
    }

    //DialogueData Ŭ���� : ��ȭ �����͸� �����ϴ� Ŭ����
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueLine> lines; //��ȭ ����
    }
}
