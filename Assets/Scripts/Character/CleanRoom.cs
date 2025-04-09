using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //CleanRoom Ŭ���� : û�ұ����� ��Ÿ���� Ŭ����
    public class CleanRoom : MonoBehaviour
    {
        public bool IsComplete
        {
            get { return isComplete; }
            set {
                isComplete = value;
                if (isComplete) {
                    //����HUD�� û���� �� ���� ����
                    GameManager.Singleton.gameHUD.CleanRoomCount--;

                    //�̴ϸ� ������ ��Ȱ��ȭ
                    minimapIcon.gameObject.SetActive(false);

                    //û�� �Ϸ�� ���� �������� ����
                    Destroy(projectors);
                }
            }
        }
        private bool isComplete;

        public string dirtyRoomName;
        public float dirtyTotalValue;
        public float dirtyCleanValue;
        public Canvas minimapIcon;
        public GameObject projectors;

        private void Start()
        {
            for (int i = 0; i < GetComponentsInChildren<Projector>().Length; i++)
            {
                dirtyTotalValue += GetComponentsInChildren<Projector>()[i].fieldOfView;
            }
        }
    }
}
