using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    ///DialogueTrigger 클래스 : DialogueUI에 DialogueData를 전달하는 클래스
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueData dialogueData; //대화 데이터
        [SerializeField] private DialogueUI dialogueUI; //대화 UI
        [SerializeField] private Animator speechBubble; //대화 말풍선

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (GameManager.Singleton.IsStageStarted)
                { 
                    gameObject.SetActive(false); //스테이지 시작 후 대화 트리거 비활성화
                    return;
                }

                if (!dialogueUI.IsSet)
                {
                    dialogueData.SetCharacter(); //대화 데이터에 캐릭터 설정

                    dialogueUI.IsSet = true;
                    dialogueUI.DialogueTrigger = gameObject; //DialogueTrigger 설정
                    dialogueUI.StartDialogue(dialogueData); //대화 시작
                }
                else dialogueUI.DialogueSetActive(true); //대화 UI 비활성화

                if (speechBubble != null)
                {
                    dialogueUI.SpeechBubble = speechBubble; //대화 UI에 말풍선 애니메이션 설정

                    speechBubble.SetBool("isShow", true);
                    speechBubble.SetTrigger("showTrigger");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                dialogueUI.DialogueSetActive(false); //대화 UI 비활성화
                StartCoroutine(dialogueUI.ShowCurrentLine()); //현재 대사 표시
            }
        }
    }
}