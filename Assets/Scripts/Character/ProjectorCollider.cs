using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    [System.Serializable]
    public class DirtyObjInteract
    {
        public Sprite interactIcon;
        public string interactKey;
        public string interactMsg;

        public DirtyObjInteract(Sprite icon, string key, string msg)
        {
            interactIcon = icon;
            interactKey = key;
            interactMsg = msg;
        }
    }

    //ProjectorCollider : ���������� �浹�� �����ϴ� Ŭ����
    public class ProjectorCollider : MonoBehaviour
    {
        public enum DirtyType
        {
            Water,
            Vacuum,
            Repair,
        }
        [SerializeField]private DirtyType dirtyType;

        private CleanRoom currentRoom;
        private Projector projector;

        public List<DirtyObjInteract> DirtyObjInteract => dirtyInteract;
        [SerializeField] private List<DirtyObjInteract> dirtyInteract;

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