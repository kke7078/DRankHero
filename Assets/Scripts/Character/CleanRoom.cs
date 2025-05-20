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
                    GameManager.Singleton.DirtyRoomCount--;

                    //�̴ϸ� ������ ��Ȱ��ȭ
                    minimapIcon.gameObject.SetActive(false);
                }
            }
        }
        private bool isComplete;

        public float ColliderCount { get; set; }

        public string DirtyRoomName => dirtyRoomName;
        [SerializeField] private string dirtyRoomName;

        public float DirtyTotalValue { get; private set; } = 0f;
        public float DirtyCleanValue { get; set; } = 0f;

        [SerializeField] private Canvas minimapIcon;

        private void Start()
        {
            var projectors = GetComponentsInChildren<Projector>();
            for (int i = 0; i < projectors.Length; i++)
            {
                DirtyTotalValue += projectors[i].fieldOfView;
            }
        }
    }
}
