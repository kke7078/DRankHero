using System;
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
        public enum ToolType { 
            WaterTank,

        }
        public ToolType currentTool;
        public Vector3 toolBackPosition;    //등에 위치한 청소도구의 위치
        public Vector3 toolBackRotation; //등에 위치한 청소도구의 회전
        
        public GameObject toolMainEffect;       //청소도구의 이펙트
        public GameObject toolSubEffext;        //청소도구의 서브 이펙트

        private GameObject waterTankCap;

        private void OnEnable()
        {
            InputSystem.Singleton.onClean += OnClean;
        }

        private void OnClean(bool cleanState)
        {
            switch (currentTool) {
                case ToolType.WaterTank:
                    if (waterTankCap == null) waterTankCap = GameObject.Find("BottleCap");

                    if (cleanState) waterTankCap.SetActive(false);
                    else waterTankCap.SetActive(true);

                    break;
            }
        }
    }
}
