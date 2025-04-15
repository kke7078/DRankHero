using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //DialogueUI Ŭ���� : ��ȭ UI�� ��Ÿ���� Ŭ����
    public class DialogueUI : MonoBehaviour
    {
        public DialogueData dialogueData;
        public Image speakerImage;
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogueText;
        public float typingSpeed = 0.05f; //Ÿ���� �ӵ�

        private Animator animator; //�ִϸ��̼� ������Ʈ
        private Queue<DialogueLine> dialogueQueue; //��ȭ ���� ť
        private Coroutine typingCoroutine; //Ÿ���� �ڷ�ƾ
        private bool isTyping = false; //Ÿ���� ������ ����
        private bool isShow = false; //��ȭâ�� ���̴��� ����

        private void Start()
        {
            animator = GetComponent<Animator>();
            InputSystem.Singleton.onDialogueNextText += ShowNextLine; //���� ��ȭ �ؽ�Ʈ ǥ��
        }


        public void StartDialogue(DialogueData dialogue) {
            if (!isShow) {
                isShow = true;
                DialogueSetActive(isShow);
            }

            dialogueQueue = new Queue<DialogueLine>(dialogue.lines);
            ShowNextLine();
        }

        public void ShowNextLine()
        {
            if (!isShow) return;

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueQueue.Peek().dialogueText;
                isTyping = false;
                return;
            }

            if (dialogueQueue.Count == 0) {
                EndDialogue();
                return;
            }

            DialogueLine line = dialogueQueue.Dequeue();
            speakerName.text = line.speakerName;
            typingCoroutine = StartCoroutine(TypeSentence(line.dialogueText));
        }

        IEnumerator TypeSentence(string sentence) {
            isTyping = true;
            dialogueText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }

        public void EndDialogue() {
            isShow = false;
            DialogueSetActive(isShow);
            Debug.Log("��ȭ ����!");
        }

        public void DialogueSetActive(bool show) {
            animator.SetBool("isShow", show);
            animator.SetBool("isHide", !show);

            if (show)
            {
                animator.SetTrigger("showTrigger");
                animator.ResetTrigger("hideTrigger");
            }
            else {
                animator.SetTrigger("hideTrigger");
                animator.ResetTrigger("showTrigger");
            }
        }
    }
}
