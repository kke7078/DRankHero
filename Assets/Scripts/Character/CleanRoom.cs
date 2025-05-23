using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace KGY
{
    //CleanRoom Ŭ���� : û�ұ����� ��Ÿ���� Ŭ����
    public class CleanRoom : MonoBehaviour
    {
        //���� û�� �Ϸ� ����
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

        //���� �̸�
        public string DirtyRoomName => dirtyRoomName;
        [SerializeField] private string dirtyRoomName;

        public float DirtyTotalValue { get; private set; } = 0f;    //���� û���ؾ��� ��
        public float DirtyCleanValue { get; set; } = 0f;    //���� û�ҵ� ��

        [SerializeField] private InteractionUI interactionUI;
        [SerializeField] private Canvas minimapIcon;
        [SerializeField] private float colliderCount;


        private void Start()
        {
            var projectors = GetComponentsInChildren<Projector>();
            for (int i = 0; i < projectors.Length; i++)
            {
                DirtyTotalValue += projectors[i].fieldOfView;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (IsComplete) return;

                colliderCount++;
                interactionUI.SetGaugeBarName(DirtyRoomName);
                interactionUI.ShowCleanRoomGaugeUI(this);
                interactionUI.UpdateGaugeValue(this);
            }
        }
    }
}
