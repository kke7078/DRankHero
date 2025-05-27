using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace KGY
{
    [System.Serializable]
    public struct ToolTransform
    {
        public Vector3 backPosition; //등에 위치한 청소도구의 위치
        public Vector3 backRotation; //등에 위치한 청소도구의 회전
        public Vector3 handPosition; //손에 위치한 청소도구의 위치
        public Vector3 handRotation; //손에 위치한 청소도구의 회전
    }

    //CleanToolManager 클래스 : 청소도구의 속성을 정의하는 클래스
    public class CleanToolManager : MonoBehaviour
    {
        public enum ToolType
        {
            WaterTank,
            None,
        }
        public ToolType CurrentToolType
        {
            get => currentToolType;
            set => currentToolType = value;
        }
        [SerializeField] private ToolType currentToolType; //현재 장착된 청소도구 타입

        public GameObject ToolMainEffect
        {
            get => toolMainEffect;
            set => toolMainEffect = value;
        }
        [SerializeField] private GameObject toolMainEffect; //청소도구의 메인 이펙트

        public GameObject ToolSubEffext {
            get => toolSubEffext;
            set => toolSubEffext = value;
        }
        [SerializeField] private GameObject toolSubEffext; //청소도구의 서브 이펙트

        public ToolTransform ToolTransform => toolTransform;
        [SerializeField] private ToolTransform toolTransform;   //청소도구의 위치와 회전 정보

        [SerializeField] private GameObject waterTankCap;

        private void OnEnable()
        {
            InputSystem.Singleton.onClean += OnClean;
        }

        private void OnClean(bool cleanState)
        {
            if (GameManager.Singleton.IsCharacterMovementLocked == true) return;

            switch (CurrentToolType) {
                case ToolType.WaterTank:
                    if (waterTankCap == null) waterTankCap = GameObject.Find("BottleCap");

                    if (cleanState) waterTankCap.SetActive(false);
                    else waterTankCap.SetActive(true);

                    break;
            }
        }
    }
}
