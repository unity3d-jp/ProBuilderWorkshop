using System.Collections;
using UnityEngine;

namespace PlayerLocomotion
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        #region Settings

        [Header("Layer Setting")]

        public LayerMask groundLayer = 1 << 0;

        [Header("Basic Setting")]

        [SerializeField]
        private float runSpeed = 5f;

        [SerializeField, Range(1f, 2.5f)]
        private float sprintSpeedUpRatio = 2f;

        [SerializeField]
        private float angularVelocity = 100f;

        [Header("Jump Setting")]

        public float jumpVelocity = 5f;
        public float risingDuration = 0.5f;
        public float additionalFallingGravity = 30f;

        [SerializeField, Range(0, 1f)]
        public float inheritVelocityRatio = 0.8f;

        [Header("Step Setting")]

        [Range(0, 0.5f)]
        public float maxStep = 0.3f;

        #endregion

        #region Values

        private float risingTimeCounter;
        private float groundDistance;
        private Vector3 groundVelocity;
        private bool isJumping, isGrounded, isSprinting;
        private PhysicMaterial defaultPhysics, noFrictionPhysics, slippyPhysics;
        private float speed;
        private float direction;
        private Vector2 horizontalVelocity;
        private Vector3 targetDirection;
        private Ray stepCheckRay;
        private float stepCheckRayLength;

        #endregion

        #region Components

        private Rigidbody m_Rigidbody;
        public Rigidbody _rigidbody
        {
            get
            {
                if (m_Rigidbody == null) m_Rigidbody = GetComponent<Rigidbody>();
                return m_Rigidbody;
            }
        }

        private CapsuleCollider m_CapsuleCollider;
        public CapsuleCollider _capsuleCollider
        {
            get
            {
                if (m_CapsuleCollider == null) m_CapsuleCollider = GetComponent<CapsuleCollider>();
                return m_CapsuleCollider;
            }
        }

        private Animator m_Animator;
        public Animator animator
        {
            get
            {
                if (m_Animator == null) m_Animator = GetComponent<Animator>();
                return m_Animator;
            }
        }

        #endregion

        public void StartSprinting()
        {
            isSprinting = true;
        }

        public void StopSprinting()
        {
            isSprinting = false;
        }

        public void SetHorizontalVelocity(float x, float z)
        {
            horizontalVelocity.x = x;
            horizontalVelocity.y = z;
        }


        private void Awake()
        {
            // デフォルトのPhysics。まったく滑らない
            defaultPhysics = new PhysicMaterial();
            defaultPhysics.name = "defaultPhysics";
            defaultPhysics.staticFriction = 1;
            defaultPhysics.dynamicFriction = 1;
            defaultPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // 摩擦がおきない（主に空中用）
            noFrictionPhysics = new PhysicMaterial();
            noFrictionPhysics.name = "noFrictionPhysics";
            noFrictionPhysics.staticFriction = 0;
            noFrictionPhysics.dynamicFriction = 0;
            noFrictionPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // 凍った床などに使う、ある程度滑るもの
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0.1f;
            slippyPhysics.dynamicFriction = 0.1f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Multiply;
        }

        private void Update()
        {
            UpdateAnimator();
        }

        private void LateUpdate()
        {
            UpdateTargetDirection();
        }

        private void FixedUpdate()
        {
            CheckGround();
            Locomotion();
            Pivot();
            TryToJump();
            AirControl();
        }

        private void OnAnimatorMove()
        {
            if (Time.deltaTime == 0) return;
            transform.rotation = animator.rootRotation;
        }

        private void UpdateAnimator()
        {
            if (!animator.enabled) return;

            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("GroundDistance", groundDistance);
            animator.SetFloat("VerticalVelocity", 0);

            if (!isGrounded)
            {
                animator.SetFloat("VerticalVelocity", _rigidbody.velocity.y);
            }

            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }

        private void Locomotion()
        {
            speed = Mathf.Abs(horizontalVelocity.x) + Mathf.Abs(horizontalVelocity.y);
            speed = Mathf.Clamp01(speed);

            if (isSprinting)
            {
                speed *= sprintSpeedUpRatio;
            }

            if (isGrounded && speed > 0f)
            {
                var vel = transform.forward * runSpeed * speed;
                //vel.y = _rigidbody.velocity.y;
                vel.y = 0;
                _rigidbody.velocity = vel + groundVelocity;
            }

        }

        private void Pivot()
        {
            if (horizontalVelocity != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                var lookDirection = targetDirection.normalized;
                var targetRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var euler = transform.eulerAngles;
                euler.y = targetRotation.eulerAngles.y;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), angularVelocity * Time.fixedDeltaTime);
            }
        }

        // ジャンプ開始
        public void JumpStart()
        {
            bool canJump = isGrounded && !isJumping;
            if (!canJump) return;

            risingTimeCounter = 0;
            isJumping = true;
        }

        // ジャンプ中
        public void Jumping()
        {
            if (!isJumping) return;

            risingTimeCounter += Time.deltaTime;

            if (risingTimeCounter > risingDuration)
            {
                JumpEnd();
            }
        }

        // ジャンプ終了
        public void JumpEnd()
        {
            risingTimeCounter = 0;
            isJumping = false;
        }

        private void TryToJump()
        {
            if (!isJumping) return;

            var vel = _rigidbody.velocity;
            vel.y = jumpVelocity;
            _rigidbody.velocity = vel;
        }

        private void AirControl()
        {
            if (isGrounded) return;

            var vel = transform.forward * runSpeed * speed;
            vel += groundVelocity * inheritVelocityRatio;

            _rigidbody.velocity = new Vector3(vel.x, _rigidbody.velocity.y, vel.z);
        }

        private void CheckGround()
        {
            CheckGroundDistance();

            // 通常は摩擦係数1のdefaultPhysics、空中は摩擦ゼロのものに差し替える
            if (!isGrounded || isJumping)
            {
                _capsuleCollider.material = noFrictionPhysics;
            }
            else
            {
                _capsuleCollider.material = defaultPhysics;
            }

            var onStep = StepOffset();

            // 段差を上るときも摩擦ゼロのものに差し替える
            if (onStep)
            {
                _capsuleCollider.material = noFrictionPhysics;
            }

            if (groundDistance <= 0.05f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;

                // 追加重力
                if (!onStep)
                {
                    _rigidbody.AddForce(transform.up * additionalFallingGravity * -1 * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }
        }

        // 足元から地面の距離を取る
        void CheckGroundDistance()
        {
            var distance = 10f;
            var layLength = 10f;
            RaycastHit groundHit;
            groundVelocity = Vector3.zero;

        // スフィアを飛ばして足場との距離を取る
        Ray ray = new Ray(transform.position + Vector3.up * (_capsuleCollider.radius), Vector3.down);
            if (Physics.SphereCast(ray, _capsuleCollider.radius * 0.9f, out groundHit, _capsuleCollider.radius + layLength, groundLayer))
            {
                distance = (groundHit.distance - _capsuleCollider.radius * 0.1f);

                if (groundHit.rigidbody)
                {
                    groundVelocity = groundHit.rigidbody.velocity;
                }
            }

            groundDistance = (float)System.Math.Round(distance, 2);
        }

        // 段差を判定して上がれるようにする
        bool StepOffset()
        {
            if (!isGrounded || _rigidbody.velocity.y < 0) return false;

            var stepForce = 5f;
            var hit = new RaycastHit();

            stepCheckRay = new Ray((transform.position + new Vector3(0, maxStep, 0) + transform.forward * (_capsuleCollider.radius * 1.5f)), Vector3.down);

            stepCheckRayLength = maxStep - _capsuleCollider.radius * 0.1f;

            if (Physics.Raycast(stepCheckRay, out hit, stepCheckRayLength, groundLayer) && !hit.collider.isTrigger)
            {
                if (hit.point.y > transform.position.y && hit.point.y < transform.position.y + maxStep)
                {
                    if (speed > 0)
                    {
                        //var velocityDirection = (hit.point - transform.position).normalized;
                        //_rigidbody.velocity = velocityDirection * stepForce * speed;

                        _rigidbody.velocity = (hit.point - transform.position) * stepForce;
                        return true;
                    }
                }
            }
            return false;
        }

        private void UpdateTargetDirection()
        {
            if (!Camera.main.transform) return;

            var forward = Camera.main.transform.forward;
            var right = Camera.main.transform.right;
            forward.y = 0;
            targetDirection = horizontalVelocity.x * right + horizontalVelocity.y * forward;
        }

        /* 
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(stepCheckRay.origin, stepCheckRay.origin + Vector3.down * stepCheckRayLength);
            }
        */
    }
}