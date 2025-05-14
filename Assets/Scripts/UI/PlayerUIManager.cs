using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //PlayerUIManager : 플레이어의 UI(DirectGuide, MinimapIcon 등)를 관리하는 클래스
    public class PlayerUIManager : MonoBehaviour
    {
        private void Update()
        {
            if (!PlayerCharacter.instance.isMoving) return;
            if (LayerMask.LayerToName(gameObject.layer) == "MinimapOnly") return;

            //마우스 위치 방향으로 회전
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 1000f))
            {
                Vector3 direction = hitInfo.point - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);  
                targetRot.eulerAngles = new Vector3(90f, targetRot.eulerAngles.y, 0);
                transform.rotation = targetRot;
            }
        }

        private void LateUpdate()
        {
            //플레이어 캐릭터를 따라다니도록 설정
            Vector3 guideFollowTargetPoint = PlayerCharacter.instance.transform.position;
            if (LayerMask.LayerToName(gameObject.layer) == "MinimapOnly") guideFollowTargetPoint.y = 50f;
            else guideFollowTargetPoint.y = 0.01f;

            transform.position = guideFollowTargetPoint;
        }
    }
}
