using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    //InputSystem : ������� �Է��� �޾� ó���ϴ� Ŭ����
    public class InputSystem : SingletonBase<InputSystem>
    {
        public System.Action<bool> onClean;
        public System.Action<int> onChangeTool;
        public System.Action onInteract;

        public Vector2 MoveInput { get; set; }

        private void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            MoveInput = new Vector2(h, v);

            if (Input.GetMouseButtonDown(0))
            {
                onClean?.Invoke(true);   
            }

            if (Input.GetMouseButtonUp(0))
            {
                onClean?.Invoke(false);
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                onInteract?.Invoke();
            }
        }
    }
}
