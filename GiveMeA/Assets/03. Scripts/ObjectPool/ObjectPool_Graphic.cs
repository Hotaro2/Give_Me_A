using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Graphic : MonoBehaviour
{
    public GameObject boss_Graphic_Light_Prefab;
    public GameObject boss_Graphic_BigLight_Prefab;
    public GameObject boss_Graphic_Paint_Prefab;


    GameObject[] boss_Graphic_Light;
    GameObject[] boss_Graphic_BigLight;
    GameObject[] boss_Graphic_Paint;

    

    GameObject[] targetPool;

    void Awake()
    {
        boss_Graphic_Light = new GameObject[10];
        boss_Graphic_BigLight = new GameObject[3];
        boss_Graphic_Paint = new GameObject[100];

        Generate();
    }


    void Generate()
    {
        for (int i = 0; i < boss_Graphic_Light.Length; i++)
        {
            boss_Graphic_Light[i] = Instantiate(boss_Graphic_Light_Prefab);
            boss_Graphic_Light[i].SetActive(false);
        }
        for (int i = 0; i < boss_Graphic_BigLight.Length; i++)
        {
            boss_Graphic_BigLight[i] = Instantiate(boss_Graphic_BigLight_Prefab);
            boss_Graphic_BigLight[i].SetActive(false);
        }
        for (int i = 0; i < boss_Graphic_Paint.Length; i++)
        {
            boss_Graphic_Paint[i] = Instantiate(boss_Graphic_Paint_Prefab);
            boss_Graphic_Paint[i].SetActive(false);
        }
    }


    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "Boss_Graphic_Light":
                targetPool = boss_Graphic_Light;
                break;
            case "Boss_Graphic_BigLight":
                targetPool = boss_Graphic_BigLight;
                break;
            case "Boss_Graphic_Paint":
                targetPool = boss_Graphic_Paint;
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
