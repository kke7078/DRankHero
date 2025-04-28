using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //DialogueUI 클래스 : 대화 UI를 나타내는 클래스
    public class DialogueUI : MonoBehaviour
    {
        public Image speakerImage;
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogueText;
        public CinemachineVirtualCamera virtualCamera;

        private float typingSpeed = 0.05f; //타이핑 속도
        private Animator animator; //애니메이션 컴포넌트
        private DialogueData dialogueData; //대화 데이터
        private Queue<DialogueLine> dialogueQueue; //대화 내용 큐
        private DialogueLine currentLine; //현재 대화 내용
        private Coroutine typingCoroutine; //타이핑 코루틴
        private bool isTyping = false; //타이핑 중인지 여부
        private bool isShow = false; //대화창이 보이는지 여부
        private CinemachineFramingTransposer framingTransposer;

        private void Start()
        {
            animator = GetComponent<Animator>();

            InputSystem.Singleton.onDialogueNextText += ShowNextLine; //다음 대화 텍스트 표시
            InputSystem.Singleton.onDialogueEnd += EndDialogue; //대화 종료

            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        }

        private void Update()
        {
            ControlCamera(isShow);
        }

        //대화 시작
        public void StartDialogue(DialogueData dialogue)
        {
            dialogueData = dialogue;

            if (!isShow)
            {
                isShow = true;
                DialogueSetActive(isShow);
                
                PlayerCharacter.instance.SetPlayerMovementState(false); //플레이어 동작 제어
            }

            dialogueQueue = new Queue<DialogueLine>(dialogue.lines);
            ShowNextLine();
        }

        //대사 넘기기
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
                if (ch.characterName == line.characterName)
                {
                    speakerImage.sprite = ch.characterPortraits[(int)line.faceType];
                    speakerName.text = line.characterName;
                    break;
                }
            }

            typingCoroutine = StartCoroutine(TypeSentence(line.dialogueText));
        }

        //대사 타이핑 효과
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

        //대화 종료
        public void EndDialogue()
        {
            isShow = false;
            isTyping = false;
            DialogueSetActive(false);

            PlayerCharacter.instance.SetPlayerMovementState(true); //플레이어 이동 가능
        }

        //대화창 활성화/비활성화
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

        //대화 시작/종료 시 카메라 제어
        public void ControlCamera(bool show)
        {
            //카메라의 현재 위치
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
