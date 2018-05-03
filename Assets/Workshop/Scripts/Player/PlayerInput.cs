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
        OSType buildOS = OSType.Win; //ビルドするUnity(このUnity)のOS

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

            //WebGLのときのみキーアサインは特殊仕様になる
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (osType == OSType.Win)
                {
                    InputManager.RegisterInputEvent("Sprint GL Xbox", InputPhase.Press, playerController.StartSprinting);
                    InputManager.RegisterInputEvent("Sprint GL Xbox", InputPhase.Release, playerController.StopSprinting);
                    InputManager.RegisterInputEvent("Jump GL Xbox", InputPhase.Press, playerController.JumpStart);
                    InputManager.RegisterInputEvent("Jump GL Xbox", InputPhase.Hold, playerController.Jumping);
                    InputManager.RegisterInputEvent("Jump GL Xbox", InputPhase.Release, playerController.JumpEnd);
                }
                else if (osType == OSType.Mac)
                {
                    InputManager.RegisterInputEvent("Sprint GL PS4", InputPhase.Press, playerController.StartSprinting);
                    InputManager.RegisterInputEvent("Sprint GL PS4", InputPhase.Release, playerController.StopSprinting);
                    InputManager.RegisterInputEvent("Jump GL PS4", InputPhase.Press, playerController.JumpStart);
                    InputManager.RegisterInputEvent("Jump GL PS4", InputPhase.Hold, playerController.Jumping);
                    InputManager.RegisterInputEvent("Jump GL PS4", InputPhase.Release, playerController.JumpEnd);
                }
            }
            else
            {
                InputManager.RegisterInputEvent("Sprint " + controllerType, InputPhase.Press, playerController.StartSprinting);
                InputManager.RegisterInputEvent("Sprint " + controllerType, InputPhase.Release, playerController.StopSprinting);
                InputManager.RegisterInputEvent("Jump " + controllerType, InputPhase.Press, playerController.JumpStart);
                InputManager.RegisterInputEvent("Jump " + controllerType, InputPhase.Hold, playerController.Jumping);
                InputManager.RegisterInputEvent("Jump " + controllerType, InputPhase.Release, playerController.JumpEnd);
            }
        }

        // FreelookCameraの設定
        private void SetFreelookCamera()
        {
            if (!freeLookCamera) return;
            if (controllerType == ControllerType.Other) return;
            if (osType == OSType.Other) return;

            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
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
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 3-4";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 3-4";
                }
            }
            //WebGL書き出しの場合、ビルドするUnityエディタがWindowsかMacかでキーアサインが変わる
            else
            {
                if (buildOS == OSType.Win)
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
                else if (buildOS == OSType.Mac)
                {
                    if (osType == OSType.Win && controllerType == ControllerType.PS4)
                    {
                        freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 3-6";
                        freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 3-6";
                    }
                    else if (osType == OSType.Mac)
                    {
                        freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal 4-5";
                        freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical 4-5";
                    }
                }
            }
        }
    }
}