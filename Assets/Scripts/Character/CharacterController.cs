using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterController Ŭ���� : ĳ������ �������� ���� �� ��ȣ�ۿ��� ����ϴ� Ŭ���� (�������� ���� & ���°���)
    public class CharacterController : MonoBehaviour
    {
        
        private CharacterBase characterBase;
        

        private void Awake()
        {
            //ĳ���� �⺻ Ŭ���� ����
            characterBase = GetComponent<CharacterBase>();
        }

        private void Update()
        {
            characterBase.Move(characterBase.Direction, characterBase.GetSpeed());
        }
    }
}
