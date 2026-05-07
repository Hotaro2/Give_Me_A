using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject player;
    public TutorialPlayerController playerLogic;

    public Transform[] movePoint;
    public Transform[] cameraPoint;
    public Transform elvePos;
    public Image blackOut;

    private int tutorialLevel = 0;
    private bool waitingSpace = false; // true일때 스페이스바로 대화창 넘기기
    private bool isPassOK = false; // true일때 다음 튜토리얼(대화)로 넘어감

    void Start()
    {
        playerLogic = player.GetComponent<TutorialPlayerController>();
        playerLogic.controllLevel = 0;
        playerLogic.anim.SetBool("isRunning", true);
        StartCoroutine(TutorialStart());
    }

    IEnumerator TutorialStart()
    {
        yield return new WaitForSeconds(2f);
        player.transform.DOMoveX(movePoint[0].position.x, 2).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2.5f);
        isPassOK = true;
    }

    public void PassOk()
    {
        isPassOK = true;
    }
    

    public IEnumerator BlackOut(int num)
    {
        playerLogic.breakMove = true;
        playerLogic.anim.SetBool("isRunning", false);
        playerLogic.rb.velocity = Vector2.zero;

        blackOut.DOFade(1, 1f);
        yield return new WaitForSeconds(1.5f);
        player.transform.position = movePoint[num].position;
        Camera.main.transform.position = new Vector3(cameraPoint[num].position.x, cameraPoint[num].position.y, -10);
        yield return new WaitForSeconds(0.5f);
        blackOut.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        isPassOK = true;
    }

    void Update()
    {
        // 튜토리얼 진행 단계에 따라서 해당 코드 작동, 스페이스바 막을때는 skipBreak로
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (waitingSpace == false)
                return;

            waitingSpace = false;
            isPassOK = true;
        }

        // 엘리베이터씬에서 Z만 누르도록, 누르면 못움직이게
        if (tutorialLevel == 6)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                playerLogic.anim.SetBool("Dash", true);
                player.transform.DOMoveX(elvePos.position.x, 0.7f).SetEase(Ease.OutQuart);
                Invoke("PassOk", 0.2f);
                playerLogic.breakMove = true;
            }
        }

        // isOK가 true가 되면 다음으로 진행, 이후 isOK는 false가 됨
        if (isPassOK == false) 
            return;

        switch (tutorialLevel)
        {
            case 0:
                waitingSpace = true;
                playerLogic.anim.SetBool("isRunning", false);
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgStart("헉... 헉...  지고쿠 지고쿠!\n수업 시작까지 얼마나 남았지?", 3f);
                break;
            case 1:
                waitingSpace = true;
                MsgManager.Instance.MsgStart("빨리 ← → 키로 이동해서 오른쪽 건물로 들어가자", 2f);
                break;
            case 2:
                MsgManager.Instance.MsgHide();
                playerLogic.controllLevel = 1;
                break;
            // 포탈타면 진행
            case 3:
                waitingSpace = true;
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgStart("어디보자... 남은 시간이...", 1f);
                break;
            case 4:
                waitingSpace = true;
                MsgManager.Instance.MsgStart("앗! 저 엘리베이터를 놓치면 지각할거야!\nZ키로 빠르게 이동해서 엘리베이터를 타자!\n[ Space바로 진행 ]", 2.5f);
                break;
            case 5:
                MsgManager.Instance.MsgHide();
                playerLogic.controllLevel = 2;
                break;
            // Z를 눌러 엘리베이터 앞에 도착
            case 6:
                waitingSpace = true;
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgStart("잠시만요!!!\n[ Space바로 진행 ]", 0.2f);
                playerLogic.useDash = true;
                break;
            case 7:
                playerLogic.anim.SetBool("Dash", false);
                MsgManager.Instance.MsgHide();
                StartCoroutine(BlackOut(2));
                break;
            // 엘리베이터에 나오고
            case 8:
                waitingSpace = true;
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgStart("교실로 들어가자\n교실앞에 서서 ↑키를 눌르면 됐었나", 1f);
                break;
            case 9:
                MsgManager.Instance.MsgHide();
                playerLogic.breakMove = false;
                break;
            // 문 위에서 윗키 누르면
            case 10:
                MySceneManager.Instance.ChangeScene("Boss_Tutorial");
                break;
            default:
                break;
        }
        tutorialLevel++;
        isPassOK = false;
    }

}
