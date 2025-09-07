using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour
{
    // ����� �� ĭ �Ʒ��� �������� �� �ɸ��� �ð� (��)
    public float fallTime = 0.5f;
    public int numberOfFactors = 4;

    private List<GameObject> blocks = new List<GameObject>();

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

    public void StopBlock(string name)
    {
        
        if(name == "collider_down")
        {
            StopCoroutine("Fall");
            isFocus = false;
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
}