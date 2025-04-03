using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //DirectionGuide : �÷��̾��� �̵� ������ ���̵��ϴ� UI�� �����ϴ� Ŭ����
    public class PlayerUIManager : MonoBehaviour
    {
        private void Update()
        {
            if (!PlayerCharacter.instance.isMoving) return;
            if (LayerMask.LayerToName(gameObject.layer) == "MinimapOnly") return;

            //���콺 ��ġ �������� ȸ��
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
            //�÷��̾� ĳ���͸� ����ٴϵ��� ����
            Vector3 guideFollowTargetPoint = PlayerCharacter.instance.transform.position;
            if (LayerMask.LayerToName(gameObject.layer) == "MinimapOnly") guideFollowTargetPoint.y = 5f;
            else guideFollowTargetPoint.y = 0.01f;

            transform.position = guideFollowTargetPoint;
        }
    }
}
