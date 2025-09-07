using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
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
            if (child.CompareTag("Block")) // 태그 검사
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hardDrop();
            }

        }
    }

    // 지정된 시간(fallTime)마다 블록을 아래로 한 칸씩 이동시키는 코루틴
    IEnumerator Fall()
    {
        while (true)
        {
            // fallTime만큼 기다립니다.
            yield return new WaitForSeconds(delay);
            // 아래로 한 칸 이동
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

    // focus 해제시, collider 관련 오브젝트들의 layer를 변경해줘야 하므로 항상 해당 함수로 focus 해제
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
        // 충돌 방지를 위해서 미리 코루틴 정지
        StopCoroutine("Fall");
        float minY = float.MaxValue;
        foreach (GameObject obj in coliderDownList)
        {
            // Layer가 Tetrominoes 또는 Z인 오브젝트와 닿았을 때의 위치 반환
            // 이를 위해서 isFocus가 true인 오브젝트의 collider_? 오브젝트는 Collider 레이어로 변경함
            RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, Vector2.down, 30f, LayerMask.GetMask("Tetrominoes", "Z"));
            if (hit.collider != null)
            {
                // 하드드랍을 진행했을 때 내려갈 수 있는 칸 수 계산
                // 현재 레이어를 쏜 오브젝트의 y 좌표는 0.5를 차감
                // 닿은 오브젝트는 반올림 진행
                float y = obj.transform.position.y - 0.5f - Mathf.Round(hit.point.y);

                // 안해도 되지만 절댓값 처리
                y = Mathf.Abs(y);

                Debug.DrawLine(obj.transform.position, hit.point, Color.red, 2f);

                minY = Mathf.Min(minY, y);
            }
        }
        Debug.Log("now min y : " + minY);
        if (minY != float.MinValue)
        {
            // 칸 수만큼 아래로 이동
            transform.position += Vector3.down * minY;
        }

        // focus 해제
        changeIsFocus();
    }
}