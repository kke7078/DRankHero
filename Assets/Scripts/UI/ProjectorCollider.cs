using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //ProjectorCollider : 프로젝터의 충돌을 관리하는 클래스
    public class ProjectorCollider : MonoBehaviour
    {
        private Projector projector;

        private void Start()
        {
            projector = GetComponentInParent<Projector>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("WaterRipple")) {
                if (projector.fieldOfView > 10) projector.fieldOfView -= 0.1f;
                else {
                    projector.fieldOfView = 0f;
                    projector.gameObject.SetActive(false);
                }
            }
        }
    }
}
