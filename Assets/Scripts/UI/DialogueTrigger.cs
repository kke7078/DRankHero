using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    ///DialogueTrigger Ŭ���� : DialogueUI�� DialogueData�� �����ϴ� Ŭ����
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueData dialogueData; //��ȭ ������
        public DialogueUI dialogueUI; //��ȭ UI
        public Animator speechBubble; //��ȭ ��ǳ��    

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                dialogueData.SetCharacter(); //��ȭ �����Ϳ� ĳ���� ����
                dialogueUI.StartDialogue(dialogueData); //��ȭ ����

                gameObject.SetActive(false); //Ʈ���� ��Ȱ��ȭ

                if (speechBubble != null) 
                {
                    dialogueUI.SpeechBubble = speechBubble; //��ȭ UI�� ��ǳ�� �ִϸ��̼� ����

                    speechBubble.SetBool("isShow", true);
                    speechBubble.SetTrigger("showTrigger");
                }
            }
        }
    }
}