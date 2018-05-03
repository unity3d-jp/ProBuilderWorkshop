using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

namespace PlayerLocomotion
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        CinemachineFreeLook freeLookCamera;
        OSType osType = OSType.Other; //ターゲットとなるOS
        ControllerType controllerType = ControllerType.Other;

        public enum ControllerType
        {
            Xbox, PS4, Other
        }

        public enum OSType
        {
            Mac, Win, Other
        }

        private PlayerController m_PlayerController;
        public PlayerController playerController
        {
            get
            {
                if (m_PlayerController == null) m_PlayerController = GetComponent<PlayerController>();
                return m_PlayerController;
            }
        }

        private IEnumerator Start()
        {
            while (true)
            {
                var joystickNames = Input.GetJoystickNames();

                for (int i = 0; i < joystickNames.Length; i++)
                {
                    //Debug.LogFormat("{0} {1}", i, joystickNames);

                    var joystickName = joystickNames[i];

                    if (joystickName == "")
                    {
                        continue;
                    }
                    else if (joystickName.Contains("Xbox") || joystickName.Contains("XBOX") || joystickName.Contains("xinput"))
                    {
                        controllerType = ControllerType.Xbox;
                        break;
                    }
                    else if (joystickName.Contains("Wireless Controller"))
                    {
                        controllerType = ControllerType.PS4;
                        break;
                    }
                    else
                    {
                        controllerType = ControllerType.Other;
                        break;
                    }
                }

                if (SystemInfo.operatingSystem.Contains("Windows"))
                {
                    osType = OSType.Win;
                }
                else if (SystemInfo.operatingSystem.Contains("Mac"))
                {
                    osType = OSType.Mac;
                }
                else
                {
                    osType = OSType.Other;
                }

                ClearInputEvent();
                SetKeyboard();
                SetGamepad();
                SetFreelookCamera();

                yield return new WaitForSeconds(1f);
            }

        }

        private void Update()
        {
            // ダッシュ・ジャンプボタンの押下判定
            InputManager.UpdateState();

            // 移動の設定
            playerController.SetHorizontalVelocity(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        // キーボードの設定
        private void ClearInputEvent()
        {
            InputManager.ClearInputEvent();
        }

        // キーボードの設定
        private void SetKeyboard()
        {
            InputManager.RegisterInputEvent("Sprint", InputPhase.Press, playerController.StartSprinting);
            InputManager.RegisterInputEvent("Sprint", InputPhase.Release, playerController.StopSprinting);
            InputManager.RegisterInputEvent("Jump", InputPhase.Press, playerController.JumpStart);
            InputManager.RegisterInputEvent("Jump", InputPhase.Hold, playerController.Jumping);
            InputManager.RegisterInputEvent("Jump", InputPhase.Release, playerController.JumpEnd);
        }

        // ゲームパッドの設定
        private void SetGamepad()
        {
            if (controllerType == ControllerType.Other) return;
            if (osType == OSType.Other) return;

            if (controllerType == ControllerType.Xbox)
            {
                InputManager.RegisterInputEvent("Sprint Xbox", InputPhase.Press, playerController.StartSprinting);
                InputManager.RegisterInputEvent("Sprint Xbox", InputPhase.Release, playerController.StopSprinting);
                InputManager.RegisterInputEvent("Jump Xbox", InputPhase.Press, playerController.JumpStart);
                InputManager.RegisterInputEvent("Jump Xbox", InputPhase.Hold, playerController.Jumping);
                InputManager.RegisterInputEvent("Jump Xbox", InputPhase.Release, playerController.JumpEnd);
            }
            else if (controllerType == ControllerType.PS4)
            {
                InputManager.RegisterInputEvent("Sprint PS4", InputPhase.Press, playerController.StartSprinting);
                InputManager.RegisterInputEvent("Sprint PS4", InputPhase.Release, playerController.StopSprinting);
                InputManager.RegisterInputEvent("Jump PS4", InputPhase.Press, playerController.JumpStart);
                InputManager.RegisterInputEvent("Jump PS4", InputPhase.Hold, playerController.Jumping);
                InputManager.RegisterInputEvent("Jump PS4", InputPhase.Release, playerController.JumpEnd);
            }
        }

        // FreelookCameraの設定
        private void SetFreelookCamera()
        {
            if (!freeLookCamera) return;
            if (controllerType == ControllerType.Other) return;
            if (osType == OSType.Other) return;

            if (osType == OSType.Win)
            {
                if (controllerType == ControllerType.Xbox)
                {
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 4-5";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 4-5";
                }
                else if (controllerType == ControllerType.PS4)
                {
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 3-6";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 3-6";
                }

            }
            else if (osType == OSType.Mac)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 3-4";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 3-4";
                }
                else
                {
                    if (controllerType == ControllerType.Xbox)
                    {
                        freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 4-5";
                        freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 4-5";
                    }
                    else if (controllerType == ControllerType.PS4)
                    {
                        freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 3-6";
                        freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 3-6";
                    }
                }
            }
        }
    }
}