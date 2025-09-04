using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour
{
    // ����� �� ĭ �Ʒ��� �������� �� �ɸ��� �ð� (��)
    public float fallTime = 0.5f;
    public int numberOfFactors = 4;

    [SerializeField]
    private bool isFocus = true;

    void Start()
    {
        // ������ ���۵Ǹ� Fall �ڷ�ƾ�� �����մϴ�.
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

    // ������ �ð�(fallTime)���� ����� �Ʒ��� �� ĭ�� �̵���Ű�� �ڷ�ƾ
    IEnumerator Fall()
    {
        while (true) // 'this.'�� ���� �����ϸ�, ���� enabled �Ӽ��� ����Ŵ
        {
            // fallTime��ŭ ��ٸ��ϴ�.
            yield return new WaitForSeconds(fallTime);
            // �Ʒ��� �� ĭ �̵�
            transform.position += Vector3.down;
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision2D " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("Collision!");
            // �� �̻� �� ��ũ��Ʈ�� �������� �ʵ��� ��Ȱ��ȭ�մϴ�.
            // �� ���� ����Ǹ� while(enabled) ������ ���߰� �˴ϴ�.
            StopCoroutine("Fall");
            isFocus = false;
        }
    }
}