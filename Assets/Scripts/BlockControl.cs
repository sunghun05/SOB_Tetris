using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BlockControl : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
    public float delay = 0.5f;

    public GameObject dummyObjectPrefab;

    public bool isFocus = false;
    public UnityEvent focusEvent;

    //private List<GameObject> blocks = new List<GameObject>();
    //private GameObject[] dummyBlocks = new GameObject[4];
    //두 리스트를 하나의 맵으로 관리
    private Dictionary<GameObject, GameObject> blocks = new Dictionary<GameObject, GameObject>();

    private List<GameObject> coliderDownList = new List<GameObject>();

    private bool isLeftMove = true;
    private bool isRightMove = true;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Block")) // 태그 검사
            {
                blocks.Add(child.gameObject, null);
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

        if (isFocus)
        {
            spawnDummyBlock(minLocation());

            // 게임이 시작되면 Fall 코루틴을 실행합니다.
            StartCoroutine("Fall");
        }
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

        if (Input.GetKeyDown(KeyCode.LeftArrow) && isLeftMove)
        {
            deleteDummyBlock();
            transform.position += Vector3.left;
            spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && isRightMove)
        {
            deleteDummyBlock();
            transform.position += Vector3.right;
            spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            deleteDummyBlock();
            SpinBlock(90.0f);
            spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            deleteDummyBlock();
            SpinBlock(-90.0f);
            spawnDummyBlock(minLocation());
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
        foreach(GameObject block in this.blocks.Keys){
            block.gameObject.transform.Rotate(0.0f, 0.0f, -rotate);
        }
    }

    // focus 해제시, collider 관련 오브젝트들의 layer를 변경해줘야 하므로 항상 해당 함수로 focus 해제
    private void changeIsFocus()
    {
        if (!isFocus) return;
        Debug.Log("changeIsFocus 실행 " + gameObject.name );
        isFocus = false;
        
        deleteDummyBlock();

        foreach (GameObject block in blocks.Keys)
        {
            foreach (Transform child in block.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Tetrominoes");
            }
        }

        deleteLine();
        // 포커스 이벤트 발생
        Physics2D.SyncTransforms();
        focusEvent?.Invoke();
    }

    private float minLocation()
    {
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

    private void deleteLine()
    {
        HashSet<GameObject> set = new HashSet<GameObject>();
        HashSet<GameObject> set2 = new HashSet<GameObject>();

        // 20개면 삭제
        foreach (GameObject obj in blocks.Keys)
        {

            RaycastHit2D[] hit_left = Physics2D.RaycastAll(obj.transform.position, Vector2.left, 30f, LayerMask.GetMask("Tetrominoes"));
            RaycastHit2D[] hit_right = Physics2D.RaycastAll(obj.transform.position, Vector2.right, 30f, LayerMask.GetMask("Tetrominoes"));
            
            

            Debug.Log("좌우 충돌 회수 : " + (hit_left.Count() + hit_right.Count()));

            if(hit_left.Count() + hit_right.Count() == 20)
            {
                foreach (RaycastHit2D hit in hit_left)
                {
                    GameObject parent = hit.collider.gameObject.transform.parent.gameObject;
                    set.Add(parent);
                }
                foreach (RaycastHit2D hit in hit_right)
                {
                    GameObject parent = hit.collider.gameObject.transform.parent.gameObject;
                    set.Add(parent);
                }
            }
        }

        

        int downCount = set.Count / 10;
        Debug.Log("setCount : " + set.Count);
        Debug.Log("downCount : " + downCount);

        foreach(GameObject obj in set)
        {
            RaycastHit2D[] hit_up = Physics2D.RaycastAll(obj.transform.position, Vector2.up, 30f, LayerMask.GetMask("Tetrominoes"));
            foreach (RaycastHit2D hit in hit_up)
            {
                GameObject parent = hit.collider.gameObject.transform.parent.gameObject;
                set2.Add(parent);
            }

            Destroy(obj);
        }

        foreach(GameObject obj in set2)
        {
            if(obj != null)
            {
                obj.transform.position += Vector3.down * downCount;
            }
        }

        Debug.Log("Kill line 성공");
    }
}