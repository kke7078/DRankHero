using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //ProjectorCollider : 프로젝터의 충돌을 관리하는 클래스
    public class ProjectorCollider : MonoBehaviour
    {
        private CleanRoom currentRoom;
        private Projector projector;

        private void Start()
        {
            currentRoom = GetComponentInParent<CleanRoom>();
            projector = GetComponentInParent<Projector>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("WaterRipple")) {
                float currentFOV = 0;

                if (projector.fieldOfView > 10) projector.fieldOfView -= 7f * Time.deltaTime;
                else
                {
                    projector.fieldOfView = 0.001f;
                    projector.gameObject.SetActive(false);
                }

                for (int i = 0; i < currentRoom.GetComponentsInChildren<Projector>().Length; i++)
                {
                    currentFOV += currentRoom.GetComponentsInChildren<Projector>()[i].fieldOfView;
                }

                currentRoom.dirtyCleanValue = currentRoom.dirtyTotalValue - currentFOV;
            }
        }
    }
}