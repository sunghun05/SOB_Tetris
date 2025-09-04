using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
    public float fallTime = 0.5f;
    public int numberOfFactors = 4;

    [SerializeField]
    private bool isFocus = true;

    void Start()
    {
        // 게임이 시작되면 Fall 코루틴을 실행합니다.
        StartCoroutine("Fall");
    }

    void Update()
    {
        if (isFocus)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.position += Vector3.left;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += Vector3.right;
            }
        }
    }

    // 지정된 시간(fallTime)마다 블록을 아래로 한 칸씩 이동시키는 코루틴
    IEnumerator Fall()
    {
        while (true) // 'this.'는 생략 가능하며, 내장 enabled 속성을 가리킴
        {
            // fallTime만큼 기다립니다.
            yield return new WaitForSeconds(fallTime);
            // 아래로 한 칸 이동
            transform.position += Vector3.down;
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision2D " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("Collision!");
            // 더 이상 이 스크립트가 동작하지 않도록 비활성화합니다.
            // 이 줄이 실행되면 while(enabled) 루프가 멈추게 됩니다.
            StopCoroutine("Fall");
            isFocus = false;
        }
    }
}