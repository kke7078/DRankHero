using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace KGY
{
    //CleanToolManager 클래스 : 청소도구의 속성을 정의하는 클래스
    public class CleanToolManager : MonoBehaviour
    {
        public string toolName;             //청소도구의 이름
        public Vector3 toolBackPosition;    //등에 위치한 청소도구의 위치
        public Quaternion toolBackRotation; //등에 위치한 청소도구의 회전
        
        public GameObject toolMainEffect;       //청소도구의 이펙트
        public GameObject toolSubEffect;      //청소도구의 이펙트2
    }
}
