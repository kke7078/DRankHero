using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace KGY
{
    //CleanToolManager Ŭ���� : û�ҵ����� �Ӽ��� �����ϴ� Ŭ����
    public class CleanToolManager : MonoBehaviour
    {
        public enum ToolType { 
            WaterTank,

        }
        public ToolType currentTool;
        public Vector3 toolBackPosition;    //� ��ġ�� û�ҵ����� ��ġ
        public Vector3 toolBackRotation; //� ��ġ�� û�ҵ����� ȸ��
        
        public GameObject toolMainEffect;       //û�ҵ����� ����Ʈ
        public GameObject toolSubEffext;        //û�ҵ����� ���� ����Ʈ

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
