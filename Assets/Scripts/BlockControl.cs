using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class BlockControl : MonoBehaviour
{
    // ����� �� ĭ �Ʒ��� �������� �� �ɸ��� �ð� (��)
    public float delay = 0.5f;
    public int numberOfFactors = 4;

    public GameObject dummyObjectPrefab;

    //private List<GameObject> blocks = new List<GameObject>();
    //private GameObject[] dummyBlocks = new GameObject[4];
    //�� ����Ʈ�� �ϳ��� ������ ����

    private Dictionary<GameObject, GameObject> blocks = new Dictionary<GameObject, GameObject>();

    private List<GameObject> coliderDownList = new List<GameObject>();
    private List<Transform> childList = new List<Transform>();

    [SerializeField]
    private bool isFocus = true;

    void Start()
    {

        foreach (Transform child in transform)
        {
            childList.Add(child);
            if (child.CompareTag("Block")) // �±� �˻�
            {
                
                blocks.Add(child.gameObject, null);
                foreach (Transform grandChild in child)
                {

                    grandChild.gameObject.layer = LayerMask.NameToLayer("Collider");
                    if (grandChild.gameObject.name == "collider_down")
                    {
                        coliderDownList.Add(grandChild.gameObject);
                    } 
                    //else if (grandChild.gameObject.name == "collider_left")
                    //{
                    //    coliderLeftList.Add(grandChild.gameObject);
                    //} else if (grandChild.gameObject.name == "collider_right")
                    //{
                    //    coliderRightList.Add(grandChild.gameObject);
                    //}
                }
            }
        }

        spawnDummyBlock(minLocation());

        // ������ ���۵Ǹ� Fall �ڷ�ƾ�� �����մϴ�.
        StartCoroutine("Fall");

    }

    void Update()
    {

        if (!isFocus) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hardDrop();
            deleteDummyBlock();
            return;
        }



        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            deleteDummyBlock();
            if(CheckLeftRight(-1) == 0)
            {
                transform.position += Vector3.left;

            }


                spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            deleteDummyBlock();
            if (CheckLeftRight(1) == 0)
            {
                transform.position += Vector3.right;

            }
            spawnDummyBlock(minLocation());
        }

        //spin
        //�ݽð�
        if (Input.GetKeyDown(KeyCode.Z))
        {
            deleteDummyBlock();
            if (SpinBlock(90.0f)) // ȸ���� �����ߴٸ�
            {
                spawnDummyBlock(minLocation());
            }
            else // �����ߴٸ� ���� ����� ���� ��ġ�� �ٽ� ����
            {
                spawnDummyBlock(minLocation());
            }
        }
        //�ð�
        if (Input.GetKeyDown(KeyCode.X))
        {
            deleteDummyBlock();
            if (SpinBlock(-90.0f)) // ȸ���� �����ߴٸ�
            {
                spawnDummyBlock(minLocation());
            }
            else // �����ߴٸ� ���� ����� ���� ��ġ�� �ٽ� ����
            {
                spawnDummyBlock(minLocation());
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

    public int CheckLeftRight(int direction)
    {
        int status = 0;
        //direction -1: left, 1: right
        
        foreach (Transform child in transform)
        {
            // Layer�� Tetrominoes �Ǵ� Z�� ������Ʈ�� ����� ���� ��ġ ��ȯ
            // �̸� ���ؼ� isFocus�� true�� ������Ʈ�� collider_? ������Ʈ�� Collider ���̾�� ������
            RaycastHit2D hit = Physics2D.Raycast(child.transform.position, Vector2.right * direction, 0.5f, LayerMask.GetMask("Z"));
            if (hit.collider != null)
            {
                // �ϵ����� �������� �� ������ �� �ִ� ĭ �� ���
                // ���� ���̾ �� ������Ʈ�� y ��ǥ�� 0.5�� ����
                // ���� ������Ʈ�� �ݿø� ����
                status = 1;

                UnityEngine.Debug.DrawLine(child.transform.position, hit.point, Color.red, 2f);

            }
        }

        if (status!=0)
        {
            return -1;
        }else
        {
            return 0;
        }
    }

    public void StopBlock(string name)
    {

        if (name == "collider_down")
        {
            changeIsFocus();
        }

    }

    public void StartBlock(string name)
    {

    }

    private bool SpinBlock(float rotate)
    {
        // 1. ȸ�� �� ���� ���� ���� (��ġ, ȸ��)
        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // 2. �ϴ� ȸ�� �õ�
        this.transform.Rotate(0.0f, 0.0f, rotate);

        // 3. Wall Kick �׽�Ʈ�� ���� ������ ����
        // ����: �̵� ���� -> ���� 1ĭ -> ������ 1ĭ
        Vector3[] kickOffsets = new Vector3[] {
            Vector3.zero,
            Vector3.left,
            Vector3.right
        };

        // 4. �� �������� ������� �׽�Ʈ
        foreach (var offset in kickOffsets)
        {
            // ���� ��ġ�� �����¸�ŭ �ӽ÷� �̵�
            this.transform.position += offset;

            // �̵��� ��ġ�� ��ȿ���� �˻�
            if (IsValidPosition())
            {
                //
                foreach (GameObject block in this.blocks.Keys)
                {
                    block.transform.Rotate(0.0f, 0.0f, -rotate);
                }
                return true;
            }

            
            this.transform.position -= offset;
        }

        // 5. ��� ������ �׽�Ʈ�� ������ ���, ��� ���� ���� ���·� ����
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        return false; // ���������� ȸ�� ����
    }

    /// <summary>
    /// ���� ����� ��ġ�� ��ȿ���� �˻��ϴ� ���� �Լ�.
    /// </summary>
    /// <returns>��ȿ�ϸ� true, �ƴϸ� false</returns>
    private bool IsValidPosition()
    {
        // ����� �����ϴ� ��� ���� ����(Mino)�� ���� �ݺ�
        foreach (GameObject block in this.blocks.Keys)
        {
            // ��輱 üũ (���� ������ ���� ���� ũ�⿡ �°� ���� �ʿ�)
            int x = Mathf.RoundToInt(block.transform.position.x);
            int y = Mathf.RoundToInt(block.transform.position.y);

            //// ����: ���� 10, ���� 20 ũ���� ����
            if (x < -5 || x >= 5 || y < -7)
            {
                return false;
            }

        }
        return true;
    }

    // focus ������, collider ���� ������Ʈ���� layer�� ��������� �ϹǷ� �׻� �ش� �Լ��� focus ����
    private void changeIsFocus()
    {
        isFocus = false;
        
        deleteDummyBlock();

        foreach (GameObject block in blocks.Keys)
        {
            foreach (Transform child in block.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Tetrominoes");
            }
        }
    }

    private float minLocation()
    {
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

                UnityEngine.Debug.DrawLine(obj.transform.position, hit.point, Color.red, 2f);

                minY = Mathf.Min(minY, y);
            }
        }
        return minY;
    }

    private void hardDrop()
    {
        StopCoroutine("Fall");
        foreach(KeyValuePair<GameObject, GameObject> block in blocks.ToList())
        {
            block.Key.transform.position = block.Value.transform.position;
        }
    }

    private void spawnDummyBlock(float minY)
    {
        foreach (GameObject block in blocks.Keys.ToList())
        {
            GameObject obj = Instantiate(dummyObjectPrefab, new Vector3(block.transform.position.x, block.transform.position.y - minY), Quaternion.identity);
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            blocks[block] = obj;
        }
    }

    private void deleteDummyBlock()
    {
        foreach (KeyValuePair<GameObject, GameObject> block in blocks.ToList())
        {
            if (block.Value == null) continue;
            Destroy(block.Value);
            blocks[block.Key] = null;
        }
    }
}