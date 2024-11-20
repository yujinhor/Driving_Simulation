using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // 캐릭터 프리팹
    public int initialPoolSize = 10; // 초기 생성할 오브젝트 수
    private Queue<GameObject> pool = new Queue<GameObject>();

    // 풀 초기화
    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // 비활성화 상태로 풀에 추가
            pool.Enqueue(obj);
        }
    }

    // 오브젝트 가져오기
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // 필요 시 추가 생성
            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }
    }

    // 오브젝트 반환
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
