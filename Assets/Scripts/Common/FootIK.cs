using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    public class FootIK : MonoBehaviour
    {
        private Animator animator;

        public bool enableFootIK = true;
        public LayerMask groundLayer;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator && enableFootIK)
            { 
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
                SetFootIKPosition(AvatarIKGoal.LeftFoot);

                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
                SetFootIKPosition(AvatarIKGoal.RightFoot);
            }
        }

        private void SetFootIKPosition(AvatarIKGoal foot)
        {
            RaycastHit hit;
            Vector3 footPosition = animator.GetIKPosition(foot);

            if (Physics.Raycast(footPosition + Vector3.up, Vector3.down, out hit, 2f, groundLayer))
            {
                Vector3 ikPosition = hit.point;
                Quaternion ikRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;

                animator.SetIKPosition(foot, ikPosition);
                animator.SetIKRotation(foot, ikRotation);
            }
        }
    }
}
