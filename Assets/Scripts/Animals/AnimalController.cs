using UnityEngine;

namespace Ursaanimation.CubicFarmAnimals
{
    public class AnimalController : MonoBehaviour
    {
        public Animator animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private float moveSpeed = 2.0f; // 이동 속도
        private Rigidbody rb;
        private Vector3 moveDirection; // 이동 방향
        private float lifetime = 10f; // 몇 초 뒤 삭제

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            // Rigidbody 설정 확인
            if (rb != null)
            {
                rb.useGravity = true; // 중력 활성화
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 회전 제한
            }

            // Root Motion 비활성화
            if (animator != null && animator.applyRootMotion)
            {
                animator.applyRootMotion = false;
            }

            // 이동 방향 설정
            SetMoveDirection();

            // 초기 애니메이션 설정
            animator.SetBool(IsWalking, true);

            // 초기 이동 적용
            ApplyInitialMovement();

            // 일정 시간 후 삭제
            Destroy(gameObject, lifetime);
        }

        void FixedUpdate()
        {
            MoveForward();
        }

        void Update()
        {
            // 애니메이션 갱신
            bool isMoving = rb.velocity.magnitude > 0.1f;
            if (animator != null)
            {
                animator.SetBool(IsWalking, isMoving);
            }
        }

        private void MoveForward()
        {
            if (rb == null) return;

            // XZ 평면 속도만 변경, Y축은 중력에 맡김
            Vector3 velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            rb.velocity = velocity;

            // 물체의 방향 업데이트
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        private void SetMoveDirection()
        {
            // XZ 평면에서 랜덤 방향 설정
            moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            Debug.Log($"이동 방향: {moveDirection}");

            // 초기 각도 설정
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        private void ApplyInitialMovement()
        {
            // 초기 이동 속도를 설정하여 애니메이션 딜레이를 방지
            rb.velocity = moveDirection * moveSpeed;
        }
    }
}
