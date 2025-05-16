using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //DialogueUI Ŭ���� : ��ȭ UI�� ��Ÿ���� Ŭ����
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter player;
        [SerializeField] private Image speakerImage;
        [SerializeField] private TextMeshProUGUI speakerName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private float typingSpeed = 0.05f; //Ÿ���� �ӵ�
        private Animator animator; //�ִϸ��̼� ������Ʈ
        private DialogueData dialogueData; //��ȭ ������
        private Queue<DialogueLine> dialogueQueue; //��ȭ ���� ť
        private DialogueLine currentLine; //���� ��ȭ ����
        private Coroutine typingCoroutine; //Ÿ���� �ڷ�ƾ
        private bool isTyping = false; //Ÿ���� ������ ����
        private bool isShow = false; //��ȭâ�� ���̴��� ����
        private CinemachineFramingTransposer framingTransposer;

        public Animator SpeechBubble { get; set; } //��ȭ ��ǳ�� �ִϸ��̼�

        private void Start()
        {
            animator = GetComponent<Animator>();

            InputSystem.Singleton.onDialogueNextText += ShowNextLine; //���� ��ȭ �ؽ�Ʈ ǥ��
            InputSystem.Singleton.onDialogueEnd += EndDialogue; //��ȭ ����

            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        }

        private void Update()
        {
            ControlCamera(isShow);
        }

        //��ȭ ����
        public void StartDialogue(DialogueData dialogue)
        {
            dialogueData = dialogue;

            if (!isShow)
            {
                isShow = true;
                DialogueSetActive(isShow);

                player.SetPlayerMovementState(false); //�÷��̾� ���� ����

                GameManager.Singleton.IsInDialogue = true;
            }

            dialogueQueue = new Queue<DialogueLine>(dialogue.lines);
            ShowNextLine();
        }

        //��� �ѱ��
        public void ShowNextLine()
        {
            if (!isShow) return;

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLine.dialogueText;
                isTyping = false;
                return;
            }

            if (dialogueQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            DialogueLine line = dialogueQueue.Dequeue();
            currentLine = line;
            foreach (var ch in dialogueData.character)
            {
                if (ch.CharacterName == line.characterName)
                {
                    speakerImage.sprite = ch.CharacterPortraits[(int)line.faceType];
                    speakerName.text = line.characterName;
                    break;
                }
            }

            typingCoroutine = StartCoroutine(TypeSentence(line.dialogueText));
        }

        //��� Ÿ���� ȿ��
        IEnumerator TypeSentence(string sentence)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }

        //��ȭ ����
        public void EndDialogue()
        {
            GameManager.Singleton.IsInDialogue = false;
            isShow = false;
            isTyping = false;
            DialogueSetActive(false);

            //��ǳ�� ����
            if (SpeechBubble != null)
            {
                SpeechBubble.SetBool("isShow", false);
                SpeechBubble.ResetTrigger("showTrigger");

                SpeechBubble.SetBool("isHide", true);
                SpeechBubble.SetTrigger("hideTrigger");
            }

            player.SetPlayerMovementState(true); //�÷��̾� �̵� ����
        }

        //��ȭâ Ȱ��ȭ/��Ȱ��ȭ
        public void DialogueSetActive(bool show)
        {
            animator.SetBool("isShow", show);
            animator.SetBool("isHide", !show);

            if (show)
            {
                animator.SetTrigger("showTrigger");
                animator.ResetTrigger("hideTrigger");
            }
            else
            {
                animator.SetTrigger("hideTrigger");
                animator.ResetTrigger("showTrigger");
            }
        }

        //��ȭ ����/���� �� ī�޶� ����
        public void ControlCamera(bool show)
        {
            //ī�޶��� ���� ��ġ
            Vector3 currentOffset = framingTransposer.m_TrackedObjectOffset;
            Vector3 desiredOffset = Vector3.zero;

            if (show)
            {
                desiredOffset = new Vector3(currentOffset.x, 3.5f, currentOffset.z);
            }
            else
            {
                desiredOffset = new Vector3(currentOffset.x, 1f, currentOffset.z);
            }

            if (Vector3.Distance(currentOffset, desiredOffset) > 0.01f)
            {
                framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, desiredOffset, Time.deltaTime * 10f);
            }
        }
    }
}