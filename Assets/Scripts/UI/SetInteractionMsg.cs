using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class SetInteractionMsg : MonoBehaviour
    {
        public InteractionData.MsgId MsgId
        {
            get => msgId;
            private set => msgId = value;
        }
        [SerializeField] private InteractionData.MsgId msgId;

        public Image MsgIcon => msgIcon;
        [SerializeField] private Image msgIcon;

        public TextMeshProUGUI MsgKey => msgKey;
        [SerializeField] private TextMeshProUGUI msgKey;
        
        public TextMeshProUGUI MsgMessage => msgMessage;
        [SerializeField] private TextMeshProUGUI msgMessage;


        //얘는 InteractionObj 각각의 정보를 보여주는 거임! 헷갈리지 말것!!!
        public void InitMessage(InteractionData.MsgId id) 
        {
            var data = InteractionManager.Singleton.InteractionData;
            var icon = data.GetIcon(id);
            var key = data.GetKey(id);
            var message = data.GetMessage(id);

            if (icon != null)
            {
                MsgIcon.sprite = icon;
                MsgIcon.gameObject.SetActive(true);
                MsgKey.transform.parent.parent.gameObject.SetActive(false);
            }
            else
            {
                MsgKey.text = key;
                MsgKey.transform.parent.parent.gameObject.SetActive(true);
                MsgIcon.gameObject.SetActive(false);
            }

            MsgId = id;
            MsgMessage.text = message;
        }
    }
}
