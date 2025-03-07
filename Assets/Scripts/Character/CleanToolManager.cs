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
        public string toolName;             //û�ҵ����� �̸�
        public Vector3 toolBackPosition;    //� ��ġ�� û�ҵ����� ��ġ
        public Quaternion toolBackRotation; //� ��ġ�� û�ҵ����� ȸ��
        
        public GameObject toolMainEffect;       //û�ҵ����� ����Ʈ
        public GameObject toolSubEffect;      //û�ҵ����� ����Ʈ2
    }
}
