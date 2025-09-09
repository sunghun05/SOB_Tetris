using Tomino.Shared;
using UnityEngine;
using System;

public class BlockCollider : MonoBehaviour
{
    private BlockControl parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        parent = GetComponentInParent<BlockControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �浹 �߻���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!parent.isFocus) return;
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            //Debug.Log(parent.gameObject.name + "�� " + " collider �߻�! " + this.gameObject.name);
            // �θ��� �Լ��� ����
            parent.StopBlock(this.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!parent.isFocus) return;
        parent.StartBlock(this.gameObject.name);
    }
}
