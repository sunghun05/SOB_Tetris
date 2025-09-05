using Tomino.Shared;
using UnityEngine;
using System;

public class BlockCollider : MonoBehaviour
{
    private BlockControl parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = GetComponentInParent<BlockControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 충돌 발생시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            // 부모의 함수를 실행
            parent.StopBlock(this.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        parent.StartBlock(this.gameObject.name);
    }
}
