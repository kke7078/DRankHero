using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KGY
{
    //DialogueUI Ŭ���� : ��ȭ UI�� ��Ÿ���� Ŭ����
    public class DialogueUI : MonoBehaviour
    {
        public TextMeshPro speakerName;
        public TextMeshPro dialogueText;
        public float typeSpeed = 0.05f; //Ÿ���� �ӵ�

        private Animator animator; //�ִϸ��̼� ������Ʈ
        private Queue<DIalogueLine> disalobueQueue; //��ȭ ���� ť
        private Coroutine typingCoroutine; //Ÿ���� �ڷ�ƾ
        private bool isTyping = false; //Ÿ���� ������ ����
        private bool isShow = false; //��ȭâ�� ���̴��� ����

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void StartDialogue(DialogueUI dialogue) {
            if (!isShow) {
                animator.setTrigger("isShowTrigger");

            }
        }
    }
}
