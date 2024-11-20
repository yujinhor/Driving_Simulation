using UnityEngine;

public class MainCarRadius : MonoBehaviour
{
    public float radius = 50f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius); // 반경 시각화
    }
}
