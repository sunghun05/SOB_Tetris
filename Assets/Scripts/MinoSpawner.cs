using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Tomino.Shared;
using UnityEngine;
using UnityEngine.Events;

public class MinoSpawner : MonoBehaviour
{
    public GameObject[] minoPrefabs;

    private Queue<int> nextMino = new Queue<int>();
    
    private GameObject[] blocks = new GameObject[7];

    private GameObject startPosition;

    private BlockControl nowBlockControl = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = GameObject.Find("start_position");

        int idx = 0;
        foreach (GameObject mino in minoPrefabs)
        {
            GameObject obj = Instantiate(mino, transform.position, Quaternion.identity, gameObject.transform);
            obj.SetActive(false);
            MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;
            }

            blocks[idx++] = obj;
        }
        setNextMino();
        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if(nextMino.Count <= 7)
        {
            setNextMino();
        }
    }

    private void setNextMino()
    {
        List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };

        //리스트 셔플을 위해
        //Fisher-Yates 알고리즘 사용
        for (int i = 6; i >= 0; i--)
        {
            int j = Random.Range(0, i + 1);

            // 현재 요소와 무작위 요소를 교환
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        for (int i= 0;i < 7; i++){
            nextMino.Enqueue(list[i]);
        }

        //list.ForEach(i => nextMino.Enqueue(i));
    }

    public void spawn()
    {
        if(nowBlockControl != null) nowBlockControl.focusEvent.RemoveListener(spawn);

        int idx = changeMino();

        GameObject newMino = Instantiate(minoPrefabs[idx], startPosition.transform.position, Quaternion.identity);

        nowBlockControl = newMino.GetComponent<BlockControl>();
        nowBlockControl.focusEvent.AddListener(spawn);
    }

    private int changeMino()
    {
        int now = nextMino.Dequeue();
        int next = nextMino.Peek();
        blocks[now].SetActive(false);
        blocks[next].SetActive(true);
        return now;
    }
}
