using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MsgManager : MonoBehaviour
{
    public GameObject msgObj;
    public GameObject leftStandObj;
    public GameObject rightStandObj;

    public Transform[] movePoint;
    /*  0: 채팅창 숨기기        1: 채팅창 꺼내기
     *  2: 왼쪽스탠드 숨기기     3: 왼쪽 스탠드 꺼내기
     *  4: 오른쪽스탠드 숨기기   5: 오른쪽 스탠드 꺼내기
     */

    private Text chatName;
    private Text chat;

    private int msgLevel = 0;

    public bool msgPassOK = false;
    private bool waitingSpace = false;

    public bool demoBossClear = false;

    public static MsgManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static MsgManager instance;

    private void Start()
    {
        chatName = msgObj.transform.GetChild(0).GetComponent<Text>();
        chat = msgObj.transform.GetChild(1).GetComponent<Text>();

        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }


    // 채팅창과 스탠딩 일러스트 보이게하기
    public void MsgSetting()
    {
        msgObj.transform.DOMove(movePoint[1].position, 1f);
        leftStandObj.transform.DOMove(movePoint[3].position, 1f);
        rightStandObj.transform.DOMove(movePoint[5].position, 1f);
    }

    // 채팅창과 스탠딩 일러스트 숨기기
    public void MsgHide()
    {
        msgObj.transform.DOKill();
        leftStandObj.transform.DOKill();
        rightStandObj.transform.DOKill();

        msgObj.transform.DOMove(movePoint[0].position, 1f);
        leftStandObj.transform.DOMove(movePoint[2].position, 1f);
        rightStandObj.transform.DOMove(movePoint[4].position, 1f);
    }

    public void MsgStart(string msg, float time)
    {
        chat.DOKill();
        chat.text = "";
        chat.DOText(msg, time).SetEase(Ease.Linear);
    }

    public void MsgName(string msgName)
    {
        chatName.text = msgName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (waitingSpace == true)
            {
                waitingSpace = false;
                msgPassOK = true;
            }
        }

        if (demoBossClear == true)
            DemoBossMsg();
    }

    private void DemoBossMsg()
    {
        if (msgPassOK == false)
            return;

        msgPassOK = false;
        switch (msgLevel)
        {
            case 0:
                waitingSpace = true;
                MsgSetting();
                MsgName("교수");
                MsgStart("지금까지 제출한 과제를 보니 훌륭하게 했군!", 0.5f);
                break;
            case 1:
                waitingSpace = true;
                MsgStart("하지만 지금과 같은 실력으론 아직 부족해", 0.5f);
                break;
            case 2:
                waitingSpace = true;
                MsgStart("아직 프로젝트는 시간이 남았으니 더 좋은 결과물을 보여주도록!", 0.6f);
                break;
            case 3:
                MySceneManager.Instance.Ending("Demo");
                msgLevel = -1;
                demoBossClear = false;
                MsgHide();
                break;
            default:
                break;
        }
        msgLevel++;
    }
}
