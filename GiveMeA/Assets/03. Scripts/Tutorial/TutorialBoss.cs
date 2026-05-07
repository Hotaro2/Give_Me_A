    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

public class TutorialBoss : MonoBehaviour
{
    private int maxBossHealth = 10;
    private int bossHealth = 10;
    public Image healthBar;
    private GameObject bossObject;
    public Transform[] skillPointSkill1 = new Transform[4];
    /*  [ 패턴1 - 위아래 큐브 날리기 ] 
     *  0: 왼쪽 위     2: 오른쪽 위
     *  1: 왼쪽 아래   3: 오른쪽 아래
     */
    public ObjectPool_Program objectPoolLogic;
    private float skillCool;

    int chatLevel = 0;
    bool isAttackStart = false;
    bool isPassOK = false;
    bool waitingSpace = false;


    public void Hit()
    {
        if (isAttackStart == false)
            return;

        bossHealth--;
        healthBar.fillAmount = 1 - bossHealth / (float)maxBossHealth;
        Debug.Log("Health: " + bossHealth);

        if (bossHealth <= 0)
        {
            Camera.main.transform.DOShakePosition(0.5f, 1f).SetEase(Ease.OutCubic);
            isPassOK = true;
            isAttackStart = false;
            StopCoroutine("Skill1Main");
            GameObject[] activeObj = GameObject.FindGameObjectsWithTag("BossBullet");
            for (int i = 0; i < activeObj.Length; i++)
            {
                ResetObjectInfo(activeObj[i]);
            }
        }

    }


    private void Start()
    {
        bossObject = gameObject;
        isPassOK = true;
    }

    private void Update()
    {
        Chat();
        if (isAttackStart)
        {
            if (skillCool > 0)
            {
                skillCool -= Time.deltaTime;
                return;
            }

            skillCool = 5f;
            Skill1();
        }
    }

    void Chat()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (waitingSpace)
            {
                waitingSpace = false;
                isPassOK = true;
            }
        }

        if (isPassOK == false)
            return;
        isPassOK = false;
        switch (chatLevel)
        {
            case 0:
                waitingSpace = true;
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgName("교수");
                MsgManager.Instance.MsgStart("아슬아슬하게 도착했군", 0.5f);
                break;
            case 1:
                waitingSpace = true;
                MsgManager.Instance.MsgStart("그러면 지금부터 바로 과제를 내주도록 하지", 0.5f);
                break;
            case 2:
                waitingSpace = true;
                MsgManager.Instance.MsgName("나");
                MsgManager.Instance.MsgStart("이런 오자마자 과제라니... 너무 많은 과제를 맡으면\n의욕이 꺾여 프로젝트를 완성할 수 없다.", 0.5f);
                break;
            case 3:
                waitingSpace = true;
                MsgManager.Instance.MsgName("나");
                MsgManager.Instance.MsgStart("[X]로 점프하고 [C]로 자료를 제출해서 프로젝트 보고서를 제출하자\n\n[ Space로 진행 ]", 0.5f);
                break;
            case 4:
                isAttackStart = true;
                MsgManager.Instance.MsgHide();
                break;
            case 5:
                waitingSpace = true;
                MsgManager.Instance.MsgSetting();
                MsgManager.Instance.MsgName("교수");
                MsgManager.Instance.MsgStart("훌륭한 보고서군!", 0.5f);
                break;
            case 6:
                waitingSpace = true;
                MsgManager.Instance.MsgStart("그러면 앞으로 프로젝트를 완성해서 보내주게", 0.5f);
                break;
            case 7:
                waitingSpace = true;
                MsgManager.Instance.MsgName("나");
                MsgManager.Instance.MsgStart("이제 모든 과목에서 최종 결과물을 제출해야 한다.", 0.5f);
                break;
            case 8:
                waitingSpace = true;
                MsgManager.Instance.MsgStart("어느과목부터 할까?", 0.5f);
                break;
            case 9:
                waitingSpace = true;
                MsgManager.Instance.MsgHide();
                MySceneManager.Instance.ChangeScene("Lobby");
                break;
            default:
                break;
        }
        chatLevel++;
    }


    private void ResetObjectInfo(GameObject obj)
    {
        obj.transform.DOKill();

        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;

        if (obj.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
        {
            Color objColor = obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            objColor.a = 1;
            obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = objColor;
        }
        if (obj.transform.GetChild(0).GetComponent<MeshRenderer>() != null)
        {
            obj.transform.GetChild(0).transform.DOKill();
            obj.transform.GetChild(0).transform.rotation = Quaternion.Euler(Vector3.zero);

            Color objColor = obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            objColor.a = 1;
            obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = objColor;
            obj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        obj.SetActive(false);
    }



    void Skill1()
    {
        int rightRan = Random.Range(2, 4);
        StartCoroutine(Skill1Main(0, rightRan));
    }
    IEnumerator Skill1Main(float delay, int rightRan)
    {
        /*
         * 오브제코드 날리기 -> 오브제코드 사라지고 큐브 생성 -> 애드포스코드 날리기 -> 애드포스코드 사라지고 큐브 발사
         */
        yield return new WaitForSeconds(delay);

        // 오른쪽에 Gameobject코드 날리기
        GameObject bossCodeGameobjectRight = objectPoolLogic.MakeObj("BossCodeGameobject");
        bossCodeGameobjectRight.transform.position = bossObject.transform.position;
        bossCodeGameobjectRight.transform.DOMove(skillPointSkill1[rightRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeGameobjectRight.transform.localScale = Vector3.zero;
        bossCodeGameobjectRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.7f);

        // 오른쪽 코드 사라지기 = 큐브 나오기 (계속 회전시키기)
        bossCodeGameobjectRight.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeGameobjectRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        GameObject bossCubeRight = objectPoolLogic.MakeObj("BossCube");
        bossCubeRight.transform.position = new Vector3(skillPointSkill1[rightRan].position.x, skillPointSkill1[rightRan].position.y, 1.5f);
        bossCubeRight.transform.GetChild(0).transform.DORotate(new Vector2(0, 2400), 10f).SetEase(Ease.Linear).SetRelative();
        bossCubeRight.transform.localScale = Vector3.zero;
        bossCubeRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.7f);
        yield return new WaitForSeconds(0.3f);

        // 오른쪽에 AddForce코드 날리기, 이 시점보다 0.3초 늦게하면 보기 조흠
        GameObject bossCodeAddForceRight = objectPoolLogic.MakeObj("BossCodeAddForce");
        bossCodeAddForceRight.transform.position = bossObject.transform.position;
        bossCodeAddForceRight.transform.DOMove(skillPointSkill1[rightRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeAddForceRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.3f).SetEase(Ease.Linear);
        bossCodeAddForceRight.transform.localScale = Vector3.zero;
        bossCodeAddForceRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(1f);
        // 오른쪽 AddForce코드 사라지기 = 큐브 날리기
        bossCodeAddForceRight.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeAddForceRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        Rigidbody2D bossCubeRightRigid = bossCubeRight.GetComponent<Rigidbody2D>();
        bossCubeRightRigid.AddForce(Vector2.left * 7f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3f);

        // 사용한 큐브들 초기화, 비활성화
        ResetObjectInfo(bossCodeGameobjectRight);
        ResetObjectInfo(bossCodeAddForceRight);
    }
}