using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace KGY
{
    //CleanToolManager Ŭ���� : û�ҵ����� �Ӽ��� �����ϴ� Ŭ����
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
        [SerializeField] private ToolType currentToolType; //���� ������ û�ҵ��� Ÿ��


        public Vector3 ToolBackPosition { 
            get => toolBackPosition;
            set => toolBackPosition = value;
        }
        [SerializeField] private Vector3 toolBackPosition;    //� ��ġ�� û�ҵ����� ��ġ


        public Vector3 ToolBackRotation
        {
            get => toolBackRotation;
            set => toolBackRotation = value;
        }
        [SerializeField] private Vector3 toolBackRotation; //� ��ġ�� û�ҵ����� ȸ��
        
        public GameObject ToolMainEffect
        {
            get => toolMainEffect;
            set => toolMainEffect = value;
        }
        [SerializeField] private GameObject toolMainEffect; //û�ҵ����� ���� ����Ʈ

        public GameObject ToolSubEffext {
            get => toolSubEffext;
            set => toolSubEffext = value;
        }
        [SerializeField] private GameObject toolSubEffext; //û�ҵ����� ���� ����Ʈ

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
