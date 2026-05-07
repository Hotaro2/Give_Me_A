using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Program : MonoBehaviour
{
    public GameObject boss_Program_CodeGameobject_Prefab;
    public GameObject boss_Program_CodeAddforce_Prefab;
    public GameObject boss_Program_CodeRevers_Prefab;
    public GameObject boss_Program_CodeGravity_Prefab;
    public GameObject boss_Program_CodeSummon_Prefab;
    public GameObject boss_Program_Summoner_Prefab;
    public GameObject boss_Program_GravityArea_Prefab;
    public GameObject boss_Program_Ball_Prefab;
    public GameObject boss_Program_Cube_Prefab;



    GameObject[] playerBullet;

    GameObject[] boss_Program_CodeGameobject;
    GameObject[] boss_Program_CodeAddforce;
    GameObject[] boss_Program_CodeRevers;
    GameObject[] boss_Program_CodeGravity;
    GameObject[] boss_Program_GravityArea;
    GameObject[] boss_Program_CodeSummon;
    GameObject[] boss_Program_Summoner;
    GameObject[] boss_Program_Ball;
    GameObject[] boss_Program_Cube;

    

    GameObject[] targetPool;

    void Awake()
    {
        boss_Program_CodeGameobject = new GameObject[100];
        boss_Program_CodeAddforce = new GameObject[100];
        boss_Program_CodeRevers = new GameObject[100];
        boss_Program_CodeGravity = new GameObject[3];
        boss_Program_GravityArea = new GameObject[3];
        boss_Program_CodeSummon = new GameObject[1];
        boss_Program_Summoner = new GameObject[1];
        boss_Program_Ball = new GameObject[1000];
        boss_Program_Cube = new GameObject[100];

        Generate();
    }


    void Generate()
    {
        for (int i = 0; i < boss_Program_CodeGameobject.Length; i++)
        {
            boss_Program_CodeGameobject[i] = Instantiate(boss_Program_CodeGameobject_Prefab);
            boss_Program_CodeGameobject[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_CodeAddforce.Length; i++)
        {
            boss_Program_CodeAddforce[i] = Instantiate(boss_Program_CodeAddforce_Prefab);
            boss_Program_CodeAddforce[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_CodeRevers.Length; i++)
        {
            boss_Program_CodeRevers[i] = Instantiate(boss_Program_CodeRevers_Prefab);
            boss_Program_CodeRevers[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_CodeGravity.Length; i++)
        {
            boss_Program_CodeGravity[i] = Instantiate(boss_Program_CodeGravity_Prefab);
            boss_Program_CodeGravity[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_GravityArea.Length; i++)
        {
            boss_Program_GravityArea[i] = Instantiate(boss_Program_GravityArea_Prefab);
            boss_Program_GravityArea[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_CodeSummon.Length; i++)
        {
            boss_Program_CodeSummon[i] = Instantiate(boss_Program_CodeSummon_Prefab);
            boss_Program_CodeSummon[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_Summoner.Length; i++)
        {
            boss_Program_Summoner[i] = Instantiate(boss_Program_Summoner_Prefab);
            boss_Program_Summoner[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_Ball.Length; i++)
        {
            boss_Program_Ball[i] = Instantiate(boss_Program_Ball_Prefab);
            boss_Program_Ball[i].SetActive(false);
        }
        for (int i = 0; i < boss_Program_Cube.Length; i++)
        {
            boss_Program_Cube[i] = Instantiate(boss_Program_Cube_Prefab);
            boss_Program_Cube[i].SetActive(false);
        }
    }


    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "BossCodeGameobject":
                targetPool = boss_Program_CodeGameobject;
                break;
            case "BossCodeAddForce":
                targetPool = boss_Program_CodeAddforce;
                break;
            case "BossCodeRevers":
                targetPool = boss_Program_CodeRevers;
                break;
            case "BossCodeGravity":
                targetPool = boss_Program_CodeGravity;
                break;
            case "BossGravityArea":
                targetPool = boss_Program_GravityArea;
                break;
            case "BossCodeSummon":
                targetPool = boss_Program_CodeSummon;
                break;
            case "BossSummoner":
                targetPool = boss_Program_Summoner;
                break;
            case "BossBall":
                targetPool = boss_Program_Ball;
                break;
            case "BossCube":
                targetPool = boss_Program_Cube;
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
