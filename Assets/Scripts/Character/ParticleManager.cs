using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KGY
{
    //ParticleManager : 캐릭터의 파티클을 관리하는 클래스
    public class ParticleManager : MonoBehaviour
    {
        private ParticleSystem ps;
        protected float fadeDuration = 0.05f;

        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            //충돌한 파티클의 인덱스를 가져오기
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
            int numParticlesAlive = ps.GetParticles(particles);

            //충돌한 파티클을 서서히 사라지게 설정하기
            for (int i = 0; i < numParticlesAlive; i++)
            {
                if (particles[i].remainingLifetime > 0)
                {
                    particles[i].remainingLifetime -= fadeDuration;
                    
                    if (particles[i].remainingLifetime < 0) particles[i].remainingLifetime = 0;               
                }
            }

            //업데이트된 파티클 정보를 적용하기
            ps.SetParticles(particles, numParticlesAlive);
        }
    }
}
