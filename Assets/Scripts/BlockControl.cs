using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour
{
    // ����� �� ĭ �Ʒ��� �������� �� �ɸ��� �ð� (��)
    public float fallTime = 0.5f;
    public int numberOfFactors = 4;

    void Start()
    {
        // ������ ���۵Ǹ� Fall �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(Fall());
    }

    // ������ �ð�(fallTime)���� ����� �Ʒ��� �� ĭ�� �̵���Ű�� �ڷ�ƾ
    IEnumerator Fall()
    {
        // �� ������Ʈ�� Ȱ��ȭ(enabled)�Ǿ� �ִ� ���� ���� �ݺ�
        while (enabled) // 'this.'�� ���� �����ϸ�, ���� enabled �Ӽ��� ����Ŵ
        {
            // fallTime��ŭ ��ٸ��ϴ�.
            yield return new WaitForSeconds(fallTime);

            // �Ʒ��� �� ĭ �̵�
            transform.position += Vector3.down;
        }
    }

    // 2D �浹�� �߻����� �� ȣ��Ǵ� �Լ�
    void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹�� ������Ʈ�� �±װ� "Floor"�̰ų� "Block"�̶��
        // (�ٴ��̳� �ٸ� ���� ��Ͽ� ��Ҵٸ�)
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Block"))
        {
            // �� �̻� �� ��ũ��Ʈ�� �������� �ʵ��� ��Ȱ��ȭ�մϴ�.
            // �� ���� ����Ǹ� while(enabled) ������ ���߰� �˴ϴ�.
            enabled = false;
        }
    }
}