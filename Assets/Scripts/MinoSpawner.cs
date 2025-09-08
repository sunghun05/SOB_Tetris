using UnityEngine;

public class MinoSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] minoPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        var child = new GameObject[transform.childCount];
        Debug.Log(transform.childCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
