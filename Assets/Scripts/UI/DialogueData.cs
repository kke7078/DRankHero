using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

namespace KGY
{
    public enum FaceType
    {
        Neutral,    //기본
        Happy,      //즐거움
        Angry,      //분노
        Sad,        //슬픔
        Surprised,  //놀람
        Doubt       //의심
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string characterName; //대화하는 캐릭터의 이름
        [TextArea] public string dialogueText; //말하는 내용
        public FaceType faceType; // 캐릭터 표정
    }

    //DialogueData 클래스 : 대화 데이터를 저장하는 클래스
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<CharacterBase> character = new List<CharacterBase>();
        public List<DialogueLine> lines; //대화 내용

        public void SetCharacter() {
            //씬에 등장하는 캐릭터를 가져옴
            CharacterBase[] characterInScene = FindObjectsOfType<CharacterBase>();

            //각 대화 내용에 등장하는 캐릭터를 초기화
            character.Clear();

            //대화에 등장하는 캐릭터들을 리스트에 추가
            foreach (var ch in characterInScene)
            {
                foreach (var line in lines)
                {
                    if (line.characterName == ch.characterName)
                    {
                        if(!character.Contains(ch)) character.Add(ch);
                    }
                }
            }
        }
    }
}