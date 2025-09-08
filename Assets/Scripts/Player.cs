using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

// ΩÃ±€≈Ê ∞¥√º
public class Player : MonoBehaviour
{

    private static Player instance;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Player();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public List<GameObject> blocks = new List<GameObject>();

    void Start()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Block");
        blocks.AddRange(obj);
    }
}
