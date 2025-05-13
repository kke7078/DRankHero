using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

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
                    GameManager.Singleton.gameHUD.CountDirtyRooms--;

                    //�̴ϸ� ������ ��Ȱ��ȭ
                    minimapIcon.gameObject.SetActive(false);
                }
            }
        }
        private bool isComplete;

        public float ColliderCount
        {
            get { return colliderCount; }
            set { colliderCount = value; }
        }
        [SerializeField] private float colliderCount;

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
