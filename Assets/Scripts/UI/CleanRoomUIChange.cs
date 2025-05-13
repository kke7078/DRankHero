using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KGY
{
    public class CleanRoomUIChange : MonoBehaviour
    {
        public Sprite spriteImage;
        public Image icon;
        public TextMeshProUGUI text;

        public void ChangeUI(string changedText) {
            if (spriteImage != null) icon.sprite = spriteImage;
            text.text = changedText;

            LayoutRebuilder.ForceRebuildLayoutImmediate(text.GetComponent<RectTransform>());
        }
    }
}
