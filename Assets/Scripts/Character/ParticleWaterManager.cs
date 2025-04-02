using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //ParticleManager : 캐릭터의 파티클을 관리하는 클래스
    public class ParticleWaterManager : MonoBehaviour
    {
        public GameObject waterEffectPrefab;
        public Transform waterRipple;
        private new ParticleSystem particleSystem;
        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        //Collision 속성 ON, Send Collison Message ON 설정되어야 동작
        private void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 collisionPoint = collisionEvents[i].intersection;   //충돌 위치
                Vector3 collisionNormal = collisionEvents[i].normal;        //충돌 법선

                //잔물결 효과 생성
                GameObject waterEffect = Instantiate(waterEffectPrefab, collisionPoint, Quaternion.LookRotation(collisionNormal), waterRipple);
                Destroy(waterEffect, 0.5f);
            }
        }
    }
}
