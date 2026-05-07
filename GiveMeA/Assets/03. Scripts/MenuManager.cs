using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Image[] buttonBackground;
    /*
     * 0: Start
     * 1: Test
     */
    private int selectNumber = 0;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectNumber != 0)
                selectNumber--;

            buttonBackground[selectNumber + 1].color = Color.white;
            buttonBackground[selectNumber].color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectNumber == buttonBackground.Length - 1)
                return;

            selectNumber++;
            buttonBackground[selectNumber - 1].color = Color.white;
            buttonBackground[selectNumber].color = Color.green;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            switch (selectNumber)
            {
                case 0:
                    GameStart();
                    break;
                case 1:
                    Test();
                    break;
                default:
                    break;
            }
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStart");
        MySceneManager.Instance.ChangeScene("Tutorial");
    }
    public void Test()
    {
        Debug.Log("Test");
    }

}
