using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //DialogueLine 클래스 : 대화를 저장할 수 있는 클래스
    [System.Serializable]
    public class DIalogueLine
    {
        public string speakerName;  //말하는 사람
        public string dialogueText; //말하는 내용
    }
}
