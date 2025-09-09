using UnityEngine;
using Tomino.Shared;

public class BlockCollider : MonoBehaviour
{
    private BlockControl parent;

    void Start()
    {
        // �θ� ��ũ��Ʈ�� ã�ƿɴϴ�. GetComponentInParent�� ���� ����Դϴ�.
        parent = GetComponentInParent<BlockControl>();
    }

    // Update �޼���� �浹 ������ ������� �����Ƿ� �����ϰų� ����Ӵϴ�.
    void Update()
    {
        // �� ���� ��� �ڵ带 �����մϴ�.
    }

    // 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 'collision'�� ���� �浹�� '����'�� �ݶ��̴��Դϴ�.
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            // �θ��� �Լ��� �����Ͽ� ����� ����ϴ�.
            // this.gameObject.name ��� gameObject.name�� ����ϴ� ���� �Ϲ����Դϴ�.
            parent.StopBlock(gameObject.name);
        }

    }

    //
    private void OnTriggerExit2D(Collider2D collision)
    {
        // ������ "Floor" �Ǵ� "Collider" �±׸� ������ ���� StartBlock�� ȣ���ϵ��� ������ �߰��ϴ� ���� �� �����մϴ�.
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Collider"))
        {
            parent.StartBlock(gameObject.name);
        }

    }

}