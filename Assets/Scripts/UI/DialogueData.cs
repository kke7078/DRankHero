using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KGY
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;  //말하는 사람
        [TextArea] public string dialogueText; //말하는 내용
        public int portraitIndex; //캐릭터의 이미지 인덱스
    }

    //DialogueData 클래스 : 대화 데이터를 저장하는 클래스
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueLine> lines; //대화 내용
    }
}
