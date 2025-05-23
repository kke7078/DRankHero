using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class CleanRoomGaugeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gaugeBarName;  // 청소 게이지 이름
        [SerializeField] private Image gaugeBar;    // 청소 게이지 스프라이트

        //청소 게이지 이름 설정
        public void SetGaugeBarName(string roomName)
        {
            gaugeBarName.text = roomName;
        }

        // 청소 완료 시 게이지 이름 변경 + 애니메이션 동작
        public void ChangeNameAnimation()
        {
            if (gaugeBarName == null) return;

            Animator animator = gaugeBarName.GetComponent<Animator>();
            if (animator == null) return;

            animator.SetTrigger("changeValue");
        }

        // 청소 게이지 값 업데이트
        public void UpdateGaugeValue(CleanRoom roomData)
        {
            if (gaugeBar == null) return;
            gaugeBar.fillAmount = roomData.DirtyCleanValue / roomData.DirtyTotalValue;
        }
    }
}
