using UnityEngine;
using Tomino.Shared;

public class BlockCollider : MonoBehaviour
{
    private BlockControl parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 부모 스크립트를 찾아옵니다. GetComponentInParent는 좋은 방법입니다.
        parent = GetComponentInParent<BlockControl>();
    }

    // Update 메서드는 충돌 감지에 사용하지 않으므로 삭제하거나 비워둡니다.
    void Update()
    {
        // 이 안의 모든 코드를 삭제합니다.
    }

    // 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!parent.isFocus) return;
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            //Debug.Log(parent.gameObject.name + "의 " + " collider 발생! " + this.gameObject.name);
            // 부모의 함수를 실행
            parent.StopBlock(gameObject.name);
        }

    }
}