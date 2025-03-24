using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //CharacterController 클래스 : 캐릭터의 물리적인 동작 및 상호작용을 담당하는 클래스 (전반적인 제어 & 상태관리)
    public class CharacterController : MonoBehaviour
    {
        
        private CharacterBase characterBase;
        

        private void Awake()
        {
            //캐릭터 기본 클래스 선언
            characterBase = GetComponent<CharacterBase>();
        }

        private void Update()
        {
            characterBase.Move(characterBase.Direction, characterBase.GetSpeed());
        }
    }
}
