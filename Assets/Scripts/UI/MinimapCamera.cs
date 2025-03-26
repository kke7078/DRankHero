using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //MinimapCamera 클래스 : 미니맵 카메라의 속성 및 동작을 정의하는 클래스
    public class MinimapCamera : MonoBehaviour
    {
        public Transform player;
        public float followHeight = 18.0f;

        private void Update()
        {
            Vector3 newCameraPosition = new Vector3(player.position.x, followHeight, player.position.z);
            transform.position = newCameraPosition;
            
            transform.LookAt(player);
        }
    }
}
