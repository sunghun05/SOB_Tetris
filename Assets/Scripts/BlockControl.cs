using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
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
            if (child.CompareTag("Block")) // 태그 검사
            {
                blocks.Add(child.gameObject);
            }
        }


        // 게임이 시작되면 Fall 코루틴을 실행합니다.
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