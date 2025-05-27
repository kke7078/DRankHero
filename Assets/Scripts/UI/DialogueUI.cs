using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    //DialogueUI 클래스 : 대화 UI를 나타내는 클래스
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter player;
        [SerializeField] private Image speakerImage;
        [SerializeField] private TextMeshProUGUI speakerName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraTarget;    //대화창 열렸을 때 카메라의 타겟

        private float typingSpeed = 0.05f; //타이핑 속도
        private Animator animator; //애니메이션 컴포넌트
        private DialogueData dialogueData; //대화 데이터
        private Queue<DialogueLine> dialogueQueue; //대화 내용 큐
        private DialogueLine currentLine; //현재 대화 내용
        private Coroutine typingCoroutine; //타이핑 코루틴
        private bool isShow = false;    //대화창이 보이는지 여부
        private bool isTyping = false; //타이핑 중인지 여부
        private CinemachineFramingTransposer framingTransposer; // 카메라의 프레이밍 트랜스포저

        public bool IsSet { get; set; } //DialogueTrigger에서 대화 데이터가 설정되었는지 여부
        public Animator SpeechBubble { get; set; } //대화 말풍선 애니메이션
        public GameObject DialogueTrigger { get; set; } // 대화 트리거 오브젝트

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
            GameManager.Singleton.ClosedDoor(); //시작지점 문 닫기

            dialogueData = dialogue;
            isShow = true;

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
                if (ch.CharacterName == line.characterName)
                {
                    speakerImage.sprite = ch.CharacterPortraits[(int)line.faceType];
                    speakerName.text = line.characterName;
                    break;
                }
            }

            typingCoroutine = StartCoroutine(TypeSentence(line.dialogueText));
        }
        
        //현재 대사 전체 표시
        public IEnumerator ShowCurrentLine()
        {
            yield return new WaitForSeconds(0.5f); //대화창이 비활성화된 후 잠시 대기

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLine.dialogueText;
                isTyping = false;
            }
        }

        //대사 타이핑 효과
        IEnumerator TypeSentence(string sentence)
        {
            isTyping = true;
            dialogueText.text = "";

            string currentText = "";
            bool insideTag = false;

            for (int i = 0; i < sentence.Length; i++)
            { 
                char c = sentence[i];
                currentText += c;

                if (c == '<') insideTag = true;
                else if(c == '>') insideTag = false;

                if (!insideTag)
                {
                    dialogueText.text = currentText;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            dialogueText.text = sentence;
            isTyping = false;
        }

        //대화 종료
        public void EndDialogue()
        {
            isShow = false;
            isTyping = false;
            DialogueSetActive(isShow);

            IsSet = false; //대화 데이터 설정 초기화
            DialogueTrigger.SetActive(false); //대화 트리거 비활성화
            DialogueTrigger = null; //대화 트리거 초기화
        }

        //대화창 활성화/비활성화
        public void DialogueSetActive(bool show)
        {
            isShow = show;

            animator.SetBool("isShow", isShow);
            animator.SetBool("isHide", !isShow);

            if (isShow)
            {
                animator.SetTrigger("showTrigger");
                animator.ResetTrigger("hideTrigger");

                if (SpeechBubble != null)
                {
                    SpeechBubble.SetBool("isShow", true);
                    SpeechBubble.SetTrigger("showTrigger");

                    SpeechBubble.SetBool("isHide", false);
                    SpeechBubble.ResetTrigger("hideTrigger");
                }
            }
            else
            {
                animator.SetTrigger("hideTrigger");
                animator.ResetTrigger("showTrigger");

                //말풍선 삭제
                if (SpeechBubble != null)
                {
                    SpeechBubble.SetBool("isShow", false);
                    SpeechBubble.ResetTrigger("showTrigger");

                    SpeechBubble.SetBool("isHide", true);
                    SpeechBubble.SetTrigger("hideTrigger");
                }
            }
        }

        //대화 시작/종료 시 카메라 제어
        public void ControlCamera(bool isShow)
        {
            //카메라의 현재 위치
            Vector3 currentOffset = framingTransposer.m_TrackedObjectOffset;
            Vector3 desiredOffset = new Vector3(0, 1f, 0);

            if (isShow)
            {
                virtualCamera.Follow = cameraTarget;
                virtualCamera.LookAt = cameraTarget;
            }
            else
            {
                virtualCamera.Follow = player.transform;
                virtualCamera.LookAt = player.transform;
            }

            if (Vector3.Distance(currentOffset, desiredOffset) > 0.01f)
            {
                framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, desiredOffset, Time.deltaTime * 1f);
                //대화 트리거 사라지는 거 해야함
            }
        }
    }
}