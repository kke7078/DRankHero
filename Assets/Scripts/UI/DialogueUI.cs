using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KGY
{
    //DialogueUI 클래스 : 대화 UI를 나타내는 클래스
    public class DialogueUI : MonoBehaviour
    {
        public TextMeshPro speakerName;
        public TextMeshPro dialogueText;
        public float typeSpeed = 0.05f; //타이핑 속도

        private Animator animator; //애니메이션 컴포넌트
        private Queue<DIalogueLine> disalobueQueue; //대화 내용 큐
        private Coroutine typingCoroutine; //타이핑 코루틴
        private bool isTyping = false; //타이핑 중인지 여부
        private bool isShow = false; //대화창이 보이는지 여부

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
