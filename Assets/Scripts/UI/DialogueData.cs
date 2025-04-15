using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //DialogueData 클래스 : 대화 데이터를 저장하는 클래스
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<DIalogueLine> lines; //대화 내용
    }
}
