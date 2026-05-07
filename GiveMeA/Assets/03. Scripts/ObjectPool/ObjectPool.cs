using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject playerBulletPrefab;

    GameObject[] playerBullet;
    

    GameObject[] targetPool;

    void Awake()
    {
        playerBullet = new GameObject[100];

        Generate();
    }


    void Generate()
    {
        for (int i = 0; i < playerBullet.Length; i++)
        {
            playerBullet[i] = Instantiate(playerBulletPrefab);
            playerBullet[i].SetActive(false);
        }
    }


    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "Player_Bullet":
                targetPool = playerBullet;
                break;
            default:
                break;
        }

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (targetPool[i].activeSelf == false)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }
}
