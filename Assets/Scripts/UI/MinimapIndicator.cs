using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

namespace KGY
{
    //MinimapIndicator Ŭ���� : �̴ϸʿ� ǥ�õ� ������Ʈ�� ��Ÿ���� Ŭ����
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

            //��ǥ�� �̴ϸ��� ������ �����
            if (distance >= threshold)
            {
                //�̴ϸ��� �����ڸ��� ��Ÿ�� ������ ǥ��
                exitIcon.gameObject.SetActive(true);

                //������ ��ġ ����
                Vector3 clampedPosiiton = new Vector3(-dir.normalized.x, -dir.normalized.z, 0) * minimapRadius;
                exitIcon.rectTransform.localPosition = clampedPosiiton;
            }
            else exitIcon.gameObject.SetActive(false);
        }
    }
}
