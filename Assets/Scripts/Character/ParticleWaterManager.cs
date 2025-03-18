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

        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            //충돌한 파티클 정보를 저장할 리스트
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int numCollsiionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            int numParticles = particleSystem.GetParticles(particles);

            bool isToolCollision = other.CompareTag("Player");

            for (int i = 0; i < numCollsiionEvents; i++)
            {
                Vector3 hitPoint = collisionEvents[i].intersection; //충돌 위치
                Vector3 hitNormal = collisionEvents[i].normal;      //충돌 법선

                //충돌한 파티클 입자 사라지게 설정
                for (int j = 0; j < numParticles; j++)
                {
                    if (particles[j].remainingLifetime > 0) {
                        if (isToolCollision) particles[j].remainingLifetime = 0;
                        else {
                            particles[j].remainingLifetime -= 0.1f;
                            if (particles[j].remainingLifetime < 0) particles[j].remainingLifetime = 0;
                        }
                    }
                }

                //잔물결 효과 생성
                if (!isToolCollision) {
                    GameObject waterEffect = Instantiate(waterEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal), waterRipple);
                    Destroy(waterEffect, 0.5f);
                }
            }

            //업데이트 된 파티클 정보 적용
            particleSystem.SetParticles(particles, numParticles);
        }
    }
}
