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
            if (CheckLeftRight(-1) == 0)
            {
                transform.position += Vector3.left;
            }
            spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && isRightMove)
        {
            deleteDummyBlock();
            if (CheckLeftRight(1) == 0)
            {
                transform.position += Vector3.right;
            }
            spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            deleteDummyBlock();
            if (SpinBlock(90.0f)) // 회전이 성공했다면
            {
                spawnDummyBlock(minLocation());
            }
            else // 실패했다면 더미 블록을 원래 위치에 다시 생성
            {
                spawnDummyBlock(minLocation());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            deleteDummyBlock();
            if (SpinBlock(-90.0f)) // 회전이 성공했다면
            {
                spawnDummyBlock(minLocation());
            }
            else // 실패했다면 더미 블록을 원래 위치에 다시 생성
            {
                spawnDummyBlock(minLocation());
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


    private bool SpinBlock(float rotate)
    {
        // 1. 회전 전 현재 상태 저장 (위치, 회전)
        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // 2. 일단 회전 시도
        this.transform.Rotate(0.0f, 0.0f, rotate);

        // 3. Wall Kick 테스트를 위한 오프셋 정의
        // 순서: 이동 없음 -> 왼쪽 1칸 -> 오른쪽 1칸
        Vector3[] kickOffsets = new Vector3[] {
            Vector3.zero,
            Vector3.left,
            Vector3.right
        };

        // 4. 각 오프셋을 순서대로 테스트
        foreach (var offset in kickOffsets)
        {
            // 현재 위치를 오프셋만큼 임시로 이동
            this.transform.position += offset;

            // 이동한 위치가 유효한지 검사
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

        // 5. 모든 오프셋 테스트에 실패한 경우, 모든 것을 원래 상태로 복구
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        return false; // 최종적으로 회전 실패
    }


    /// <summary>
    /// 현재 블록의 위치가 유효한지 검사하는 헬퍼 함수.
    /// </summary>
    /// <returns>유효하면 true, 아니면 false</returns>
    private bool IsValidPosition()
    {
        // 블록을 구성하는 모든 작은 조각(Mino)에 대해 반복
        foreach (GameObject block in this.blocks.Keys)
        {
            // 경계선 체크 (게임 보드의 가로 세로 크기에 맞게 수정 필요)
            int x = Mathf.RoundToInt(block.transform.position.x);
            int y = Mathf.RoundToInt(block.transform.position.y);

            //// 예시: 가로 10, 세로 20 크기의 보드
            if (x < -5 || x >= 5 || y < -7)
            {
                return false;
            }

        }
        return true;
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

    private int CheckLeftRight(int direction)
    {
        int status = 0;
        //direction -1: left, 1: right

        foreach (Transform child in transform)
        {
            // Layer가 Tetrominoes 또는 Z인 오브젝트와 닿았을 때의 위치 반환
            // 이를 위해서 isFocus가 true인 오브젝트의 collider_? 오브젝트는 Collider 레이어로 변경함
            RaycastHit2D hit = Physics2D.Raycast(child.transform.position, Vector2.right * direction, 0.5f, LayerMask.GetMask("Z"));
            if (hit.collider != null)
            {
                // 하드드랍을 진행했을 때 내려갈 수 있는 칸 수 계산
                // 현재 레이어를 쏜 오브젝트의 y 좌표는 0.5를 차감
                // 닿은 오브젝트는 반올림 진행
                status = 1;

                UnityEngine.Debug.DrawLine(child.transform.position, hit.point, Color.red, 2f);

            }
        }

        if (status != 0)
        {
            return -1;
        }
        else
        {
            return 0;
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