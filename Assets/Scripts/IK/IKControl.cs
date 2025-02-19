using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //IKControl : IK를 제어하는 클래스
    public class IKControl : MonoBehaviour
    {
        protected Animator animator;

        public bool isIKActive = false;
        public Transform rightHandObj = null;   //오른손 위치
        public Transform lookObj = null;        //시선 위치

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        //IK를 사용할 때 호출되는 콜백 함수
        private void OnAnimatorIK(int layerIndex)
        {
            if (animator)
            {
                //IK가 활성화 되어있을 때, 위치와 회전을 목표에 직접 설정
                if (isIKActive)
                {
                    //목표 위치가 할당된 경우, 시선 목표 위치를 설정 --> 나중에 가장 가까운 projector로 변경할 것 or 물을 쏘는 위치로 변경할 것
                    if (lookObj != null)
                    {
                        animator.SetLookAtWeight(1);
                        animator.SetLookAtPosition(lookObj.position);
                    }

                    //오른손 목표 위치가 할당된 경우, 오른손 목표 위치와 회전을 설정
                    if (rightHandObj != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                    }
                }

                //IK가 비활성화 되어있을 때, 위치와 회전을 원래 위치로 되돌림
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
