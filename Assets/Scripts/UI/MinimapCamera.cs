using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //MinimapCamera Ŭ���� : �̴ϸ� ī�޶��� �Ӽ� �� ������ �����ϴ� Ŭ����
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
