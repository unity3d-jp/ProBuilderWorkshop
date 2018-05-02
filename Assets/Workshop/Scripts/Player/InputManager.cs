using System.Collections.Generic;
using UnityEngine;

namespace PlayerLocomotion
{
    public enum InputPhase
    {
        Press,
        Hold,
        Release
    }

    public class InputManager : MonoBehaviour
    {
        public delegate void InputDelegate();

        public class InputEvent
        {
            public string name;
            public InputPhase phase;
            public event InputDelegate method;

            public void ExecuteMethod()
            {
                method();
            }
        }

        private static List<InputEvent> inputEventList;

        public static void RegisterKeyEvent(string name, InputPhase phase, InputDelegate method)
        {
            if (inputEventList == null)
            {
                inputEventList = new List<InputEvent>();
            }

            InputEvent inputEvent = new InputEvent();
            inputEvent.name = name;
            inputEvent.phase = phase;
            inputEvent.method += method;
            inputEventList.Add(inputEvent);
        }

        public static void UpdateState()
        {
            foreach (InputEvent e in inputEventList)
            {
                switch (e.phase)
                {
                    case InputPhase.Press:
                        if (Input.GetButtonDown(e.name))
                        {
                            e.ExecuteMethod();
                        }
                        break;
                    case InputPhase.Hold:
                        if (Input.GetButton(e.name))
                        {
                            e.ExecuteMethod();
                        }
                        break;
                    case InputPhase.Release:
                        if (Input.GetButtonUp(e.name))
                        {
                            e.ExecuteMethod();
                        }
                        break;
                }
            }
        }
    }
}