using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

namespace KGY
{
    //MinimapIndicator 클래스 : 미니맵에 표시될 오브젝트를 나타내는 클래스
    public class MinimapIndicator : MonoBehaviour
    {
        public Image exitIcon;
        public RectTransform exitTransform;
        public RectTransform playerIcon;

        private float threshold;
        private float minimapRadius;

        private void Start()
        {
            minimapRadius = GetComponent<RectTransform>().rect.width / 2;
            threshold = (minimapRadius * 0.1f) + 6f;
        }

        private void Update()
        {
            if (!GameManager.Singleton.IsCleanComplete) return;

            Vector3 dir = playerIcon.position - exitTransform.position;
            float distance = dir.magnitude;

            //목표가 미니맵의 범위를 벗어나면
            if (distance >= threshold)
            {
                //미니맵의 가장자리에 나타날 아이콘 표시
                exitIcon.gameObject.SetActive(true);

                //아이콘 위치 수정
                Vector3 clampedPosiiton = new Vector3(-dir.normalized.x, -dir.normalized.z, 0) * minimapRadius;
                exitIcon.rectTransform.localPosition = clampedPosiiton;
            }
            else exitIcon.gameObject.SetActive(false);
        }
    }
}
