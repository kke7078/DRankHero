using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //IKControl : IK�� �����ϴ� Ŭ����
    public class IKControl : MonoBehaviour
    {
        protected Animator animator;

        public bool isIKActive = false;
        public Transform rightHandObj = null;   //������ ��ġ
        public Transform lookObj = null;        //�ü� ��ġ

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        //IK�� ����� �� ȣ��Ǵ� �ݹ� �Լ�
        private void OnAnimatorIK(int layerIndex)
        {
            if (animator)
            {
                //IK�� Ȱ��ȭ �Ǿ����� ��, ��ġ�� ȸ���� ��ǥ�� ���� ����
                if (isIKActive)
                {
                    //��ǥ ��ġ�� �Ҵ�� ���, �ü� ��ǥ ��ġ�� ���� --> ���߿� ���� ����� projector�� ������ �� or ���� ��� ��ġ�� ������ ��
                    if (lookObj != null)
                    {
                        animator.SetLookAtWeight(1);
                        animator.SetLookAtPosition(lookObj.position);
                    }

                    //������ ��ǥ ��ġ�� �Ҵ�� ���, ������ ��ǥ ��ġ�� ȸ���� ����
                    if (rightHandObj != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                    }
                }

                //IK�� ��Ȱ��ȭ �Ǿ����� ��, ��ġ�� ȸ���� ���� ��ġ�� �ǵ���
                else
                { 
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }
    }
}
