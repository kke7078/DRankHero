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
        Neutral,    //�⺻
        Happy,      //��ſ�
        Angry,      //�г�
        Sad,        //����
        Surprised,  //���
        Doubt       //�ǽ�
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string characterName; //��ȭ�ϴ� ĳ������ �̸�
        [TextArea] public string dialogueText; //���ϴ� ����
        public FaceType faceType; // ĳ���� ǥ��
    }

    //DialogueData Ŭ���� : ��ȭ �����͸� �����ϴ� Ŭ����
    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public List<CharacterBase> character = new List<CharacterBase>();
        public List<DialogueLine> lines; //��ȭ ����

        public void SetCharacter() {
            //���� �����ϴ� ĳ���͸� ������
            CharacterBase[] characterInScene = FindObjectsOfType<CharacterBase>();

            //�� ��ȭ ���뿡 �����ϴ� ĳ���͸� �ʱ�ȭ
            character.Clear();

            //��ȭ�� �����ϴ� ĳ���͵��� ����Ʈ�� �߰�
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