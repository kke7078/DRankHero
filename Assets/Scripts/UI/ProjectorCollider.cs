using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //ProjectorCollider : ���������� �浹�� �����ϴ� Ŭ����
    public class ProjectorCollider : MonoBehaviour, IHasInteractionIds
    {
        public enum DirtyType
        {
            Water,
            Vacuum,
            Repair,
        }
        [SerializeField] private DirtyType dirtyType;

        public List<InteractionData.MsgId> InteractionIdList => interactionIdList;
        [SerializeField] private List<InteractionData.MsgId> interactionIdList = new List<InteractionData.MsgId>();

        private CleanRoom currentRoom;
        private Projector projector;

        private void Start()
        {
            currentRoom = GetComponentInParent<CleanRoom>();
            projector = GetComponentInParent<Projector>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("WaterRipple"))
            {
                float currentFOV = 0;

                // ���ٱ� �浹 �� ���������� Field of View�� �ٿ���
                projector.fieldOfView = Mathf.Max(projector.fieldOfView - 7f * Time.deltaTime, 0.001f);
                if (projector.fieldOfView <= 0.001f) projector.gameObject.SetActive(false);

                var projectors = currentRoom.GetComponentsInChildren<Projector>();
                // ���������� Field of View�� ��� ���Ͽ� û�ҵ� ������ ���
                for (int i = 0; i < projectors.Length; i++)
                {
                    currentFOV += projectors[i].fieldOfView;
                }

                currentRoom.DirtyCleanValue = currentRoom.DirtyTotalValue - currentFOV;
            }
        }
    }
}