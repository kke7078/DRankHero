using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //DirectionGuide : �÷��̾��� �̵� ������ ���̵��ϴ� UI�� �����ϴ� Ŭ����
    public class DirectionGuide : MonoBehaviour
    {
        private void Update()
        {
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
            Vector3 guideFollowTargetPoint = CharacterController.instance.transform.position;
            guideFollowTargetPoint.y = 0.01f;
            transform.position = guideFollowTargetPoint;
        }
    }
}
