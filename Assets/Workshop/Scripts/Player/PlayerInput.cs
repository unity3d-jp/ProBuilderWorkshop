using UnityEngine;
using Cinemachine;

namespace PlayerLocomotion
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        CinemachineFreeLook freeLookCamera;

        [SerializeField]
        ControllerType controllerType;

        public enum ControllerType
        {
            Xbox, PS4
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

        private void Start()
        {
            // ダッシュの設定
            InputManager.RegisterKeyEvent("Sprint", InputPhase.Press, playerController.StartSprinting);
            InputManager.RegisterKeyEvent("Sprint", InputPhase.Release, playerController.StopSprinting);

            // ジャンプの設定
            InputManager.RegisterKeyEvent("Jump", InputPhase.Press, playerController.JumpStart);
            InputManager.RegisterKeyEvent("Jump", InputPhase.Hold, playerController.Jumping);
            InputManager.RegisterKeyEvent("Jump", InputPhase.Release, playerController.JumpEnd);

            switch (controllerType)
            {
                case ControllerType.Xbox:
                    // ダッシュの設定
                    InputManager.RegisterKeyEvent("Sprint Xbox", InputPhase.Press, playerController.StartSprinting);
                    InputManager.RegisterKeyEvent("Sprint Xbox", InputPhase.Release, playerController.StopSprinting);

                    // ジャンプの設定
                    InputManager.RegisterKeyEvent("Jump Xbox", InputPhase.Press, playerController.JumpStart);
                    InputManager.RegisterKeyEvent("Jump Xbox", InputPhase.Hold, playerController.Jumping);
                    InputManager.RegisterKeyEvent("Jump Xbox", InputPhase.Release, playerController.JumpEnd);
                    break;

                case ControllerType.PS4:
                    // ダッシュの設定
                    InputManager.RegisterKeyEvent("Sprint PS4", InputPhase.Press, playerController.StartSprinting);
                    InputManager.RegisterKeyEvent("Sprint PS4", InputPhase.Release, playerController.StopSprinting);

                    // ジャンプの設定
                    InputManager.RegisterKeyEvent("Jump PS4", InputPhase.Press, playerController.JumpStart);
                    InputManager.RegisterKeyEvent("Jump PS4", InputPhase.Hold, playerController.Jumping);
                    InputManager.RegisterKeyEvent("Jump PS4", InputPhase.Release, playerController.JumpEnd);
                    break;
            }

            // FreelookCameraの設定
            SetFreelookCamera();
        }

        private void Update()
        {
            // ダッシュ・ジャンプボタンの押下判定
            InputManager.UpdateState();

            // 移動の設定
            playerController.SetHorizontalVelocity(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        private void SetFreelookCamera()
        {
            if (!freeLookCamera) return;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

            switch (controllerType)
            {
                case ControllerType.Xbox:
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal Win Xbox";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical Win Xbox";
                    break;

                case ControllerType.PS4:
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal Win PS4";
                    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical Win PS4";
                    break;
            }


#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            switch (controllerType)
            {
                case ControllerType.Xbox:
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal Mac Xbox";
	            	freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical Mac Xbox";
                    break;

                case ControllerType.PS4:
                    freeLookCamera.m_XAxis.m_InputAxisName = "Right Stick Horizontal Mac PS4";
	        	    freeLookCamera.m_YAxis.m_InputAxisName = "Right Stick Vertical Mac PS4";
                    break;
            }
#endif
        }
    }
}