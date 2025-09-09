using UnityEngine;
using Tomino.Shared;

public class BlockCollider : MonoBehaviour
{
    private BlockControl parent;

    void Start()
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
        // 'collision'은 나와 충돌한 '상대방'의 콜라이더입니다.
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            // 부모의 함수를 실행하여 블록을 멈춥니다.
            // this.gameObject.name 대신 gameObject.name을 사용하는 것이 일반적입니다.
            parent.StopBlock(gameObject.name);
        }

    }

    //
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 상대방이 "Floor" 또는 "Collider" 태그를 가졌을 때만 StartBlock을 호출하도록 조건을 추가하는 것이 더 안전합니다.
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            parent.StartBlock(gameObject.name);
        }

    }

}