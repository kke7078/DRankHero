using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    ///DialogueTrigger 클래스 : DialogueUI에 DialogueData를 전달하는 클래스
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueData dialogueData; //대화 데이터
        public DialogueUI dialogueUI; //대화 UI

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                dialogueData.SetCharacter(); //대화 데이터에 캐릭터 설정
                dialogueUI.StartDialogue(dialogueData); //대화 시작

                gameObject.SetActive(false); //트리거 비활성화
            }
        }
    }
}
