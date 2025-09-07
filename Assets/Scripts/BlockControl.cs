using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    // ����� �� ĭ �Ʒ��� �������� �� �ɸ��� �ð� (��)
    public float delay = 0.5f;
    public int numberOfFactors = 4;

    private List<GameObject> blocks = new List<GameObject>();
    private List<GameObject> coliderDownList = new List<GameObject>();

    [SerializeField]
    private bool isFocus = true;

    private bool isLeftMove = true;
    private bool isRightMove = true;
    

    void Start()
    {

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Block")) // �±� �˻�
            {
                blocks.Add(child.gameObject);
                foreach (Transform grandChild in child)
                {
                    grandChild.gameObject.layer = LayerMask.NameToLayer("Collider");
                    if (grandChild.gameObject.name == "collider_down")
                    {
                        coliderDownList.Add(grandChild.gameObject);
                    }
                }
            }
        }

        // ������ ���۵Ǹ� Fall �ڷ�ƾ�� �����մϴ�.
        StartCoroutine("Fall");

    }

    void Update()
    {
        if (isFocus)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && isLeftMove)
            {
                transform.position += Vector3.left;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && isRightMove)
            {
                transform.position += Vector3.right;
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SpinBlock(90.0f);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                SpinBlock(-90.0f);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hardDrop();
            }

        }
    }

    // ������ �ð�(fallTime)���� ����� �Ʒ��� �� ĭ�� �̵���Ű�� �ڷ�ƾ
    IEnumerator Fall()
    {
        while (true)
        {
            // fallTime��ŭ ��ٸ��ϴ�.
            yield return new WaitForSeconds(delay);
            // �Ʒ��� �� ĭ �̵�
            if (!isFocus) break;
            transform.position += Vector3.down;
            
        }
    }

    public void StopBlock(string name)
    {
        
        if(name == "collider_down")
        {
            changeIsFocus();
        }
        if(name == "collider_left")
        {
            isLeftMove = false;
        }
        if(name == "collider_right")
        {
            isRightMove = false;
        }
    }

    public void StartBlock(string name)
    {
        if(name == "collider_left")
        {
            isLeftMove = true;
        }
        if(name == "collider_right")
        {
            isRightMove = true;
        }
    }

    private void SpinBlock(float rotate)
    {
        this.transform.Rotate(0.0f, 0.0f, rotate);
        foreach(GameObject block in this.blocks){
            block.gameObject.transform.Rotate(0.0f, 0.0f, -rotate);
        }
    }

    // focus ������, collider ���� ������Ʈ���� layer�� ��������� �ϹǷ� �׻� �ش� �Լ��� focus ����
    private void changeIsFocus()
    {
        isFocus = false;
        foreach (GameObject block in blocks)
        {
            foreach (Transform child in block.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Tetrominoes");
            }
        }
    }

    private void hardDrop()
    {
        // �浹 ������ ���ؼ� �̸� �ڷ�ƾ ����
        StopCoroutine("Fall");
        float minY = float.MaxValue;
        foreach (GameObject obj in coliderDownList)
        {
            // Layer�� Tetrominoes �Ǵ� Z�� ������Ʈ�� ����� ���� ��ġ ��ȯ
            // �̸� ���ؼ� isFocus�� true�� ������Ʈ�� collider_? ������Ʈ�� Collider ���̾�� ������
            RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, Vector2.down, 30f, LayerMask.GetMask("Tetrominoes", "Z"));
            if (hit.collider != null)
            {
                // �ϵ����� �������� �� ������ �� �ִ� ĭ �� ���
                // ���� ���̾ �� ������Ʈ�� y ��ǥ�� 0.5�� ����
                // ���� ������Ʈ�� �ݿø� ����
                float y = obj.transform.position.y - 0.5f - Mathf.Round(hit.point.y);

                // ���ص� ������ ���� ó��
                y = Mathf.Abs(y);

                Debug.DrawLine(obj.transform.position, hit.point, Color.red, 2f);

                minY = Mathf.Min(minY, y);
            }
        }
        Debug.Log("now min y : " + minY);
        if (minY != float.MinValue)
        {
            // ĭ ����ŭ �Ʒ��� �̵�
            transform.position += Vector3.down * minY;
        }

        // focus ����
        changeIsFocus();
    }
}