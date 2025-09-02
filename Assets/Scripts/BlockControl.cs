using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
    public float fallTime = 0.5f;
    public int numberOfFactors = 4;

    void Start()
    {
        // 게임이 시작되면 Fall 코루틴을 실행합니다.
        StartCoroutine(Fall());
    }

    // 지정된 시간(fallTime)마다 블록을 아래로 한 칸씩 이동시키는 코루틴
    IEnumerator Fall()
    {
        // 이 컴포넌트가 활성화(enabled)되어 있는 동안 무한 반복
        while (enabled) // 'this.'는 생략 가능하며, 내장 enabled 속성을 가리킴
        {
            // fallTime만큼 기다립니다.
            yield return new WaitForSeconds(fallTime);

            // 아래로 한 칸 이동
            transform.position += Vector3.down;
        }
    }

    // 2D 충돌이 발생했을 때 호출되는 함수
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Floor"이거나 "Block"이라면
        // (바닥이나 다른 쌓인 블록에 닿았다면)
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Block"))
        {
            // 더 이상 이 스크립트가 동작하지 않도록 비활성화합니다.
            // 이 줄이 실행되면 while(enabled) 루프가 멈추게 됩니다.
            enabled = false;
        }
    }
}