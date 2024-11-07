using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWalk : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // "Locom_m_basic Walk_30f" 애니메이션을 재생
            PlayWalkAnimation();
        }
    }

    void PlayWalkAnimation()
    {
        // 애니메이션 이름을 직접 지정하여 재생
        animator.CrossFadeInFixedTime("Locom_m_basic Walk_30f", 1.0f);
    }
}
