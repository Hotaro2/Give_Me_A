using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Boss_PM : MonoBehaviour
{
    private int maxBossHealth = 30;
    private int bossHealth = 30;

    public float cooldownSkill1 = 5;
    public float cooldownSkill2 = 5;
    public float cooldownSkill3 = 5;
    public float cooldownSkill4 = 5;
    public float cooldownSkillGravity = 15;

    public Image healthBar;

    private GameObject bossObject;
    private GameObject semiBossObject;
    private GameObject player;
    public Transform[] skillPointSkill1 = new Transform[4];
    /*  [ 패턴1 - 위아래 큐브 날리기 ] 
     *  0: 왼쪽 위     2: 오른쪽 위
     *  1: 왼쪽 아래   3: 오른쪽 아래
     */

    public Transform[] skillPointSkill2 = new Transform[9];
    /* [ 패턴 2 - 위에 따다다다 따다다다다 ]
     * 0~3 = 첫번째 4개
     * 4~8 = 다음 5개
     */

    public Transform[] skillPointSkill3 = new Transform[13];
    /*
     * [ 패턴 3 - 하늘에서 비가 내려와 ]
     */

    public Transform[] skillPointSkill4 = new Transform[3];

    public ObjectPool_Program objectPoolLogic;

    public AnimationCurve gravityAreaEase;
    public AnimationCurve semiBossEase;

    public int vertex = 3; // 도형탄막 꼭짓점 개수 (3 이상)
    public int vertexCount = 4; // 도형탄막 변당 나오는 탄막 수

    List<int> Pattern = new List<int> { 1, 2, 3, 4, 5 }; // 1페이지 스킬 종류, 1~4는 Skill1~4, 5는 중력장


    public void Hit()
    {
        if (bossHealth <= 0)
            return;

        bossHealth--;
        healthBar.fillAmount = 1 - bossHealth / (float)maxBossHealth;
        Debug.Log("Health: " + bossHealth);

        if (bossHealth <= 0)
            DemoEnding();
    }

    private void DemoEnding()
    {
        MsgManager.Instance.demoBossClear = true;
        MsgManager.Instance.msgPassOK = true;

        GameObject[] activeObj = GameObject.FindGameObjectsWithTag("BossBullet");
        Camera.main.transform.DOShakePosition(0.7f, 1f).SetEase(Ease.OutQuart);
        for (int i = 0; i < activeObj.Length; i++)
        {
            ResetObjectInfo(activeObj[i]);
        }

        StopCoroutine("Skill1Main");
        StopCoroutine("Skill2Main");
        StopCoroutine("Skill3Main");
        StopCoroutine("Skill4Main");
        StopCoroutine("Skill5Main");
    }

    private void Awake()
    {
        bossObject = gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Think());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Skill1();
        if (Input.GetKeyDown(KeyCode.S))
            Skill2();
        if (Input.GetKeyDown(KeyCode.D))
            Skill3();
        if (Input.GetKeyDown(KeyCode.F))
            Skill4();
        if (Input.GetKeyDown(KeyCode.G))
            Skill5();


        if (Input.GetKeyDown(KeyCode.Q))
            UltSkill1();
        if (Input.GetKeyDown(KeyCode.W))
            UltSkill2();
        if (Input.GetKeyDown(KeyCode.E))
            UltSkill3();
        if (Input.GetKeyDown(KeyCode.R))
            UltSkill4();

        if (Input.GetKeyDown(KeyCode.O))
            UltSkillSummon();
        if (Input.GetKeyDown(KeyCode.P))
            UltSkillSpecial();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            StartCoroutine(SummonSkill1());
        if (Input.GetKeyDown(KeyCode.Keypad2))
            StartCoroutine(SummonSkill2());
        if (Input.GetKeyDown(KeyCode.Keypad3))
            StartCoroutine(SummonSkill3());



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

    // 1페이즈에서 코드를 날릴때 사용
    private void ThrowCode(GameObject obj, Vector3 targetPosition, float time)
    {
        obj.transform.position = bossObject.transform.position;
        obj.transform.DOMove(targetPosition, time).SetEase(Ease.OutCubic);
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(Vector3.one, time);
        Color objColor = obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        objColor.a = 0;
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = objColor;
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, time);
    }
    // 2페이즈에서 코드를 생성시킬때 사용
    private void CreateCode(GameObject obj, Vector3 targetPosition, float time)
    {
        obj.transform.position = targetPosition;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        obj.transform.localScale = new Vector2(0, 0);
        obj.transform.DORotate(new Vector3(0, 0, 360), time).SetEase(Ease.Linear).SetRelative();
        obj.transform.DOScale(new Vector2(1, 1), time);
        Color objColor = obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        objColor.a = 0;
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = objColor;
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, time);
    }
    private void UseCode(GameObject obj, float time)
    {
        obj.transform.DOScale(new Vector2(2, 2), time).SetEase(Ease.Linear);
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, time).SetEase(Ease.Linear);
    }
    // 코드 이후 오브젝트 생성때 사용
    private void CreateObject(GameObject obj, Vector3 targetPosition, float size, float time)
    {
        obj.transform.position = new Vector3(targetPosition.x, targetPosition.y, 2);
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        obj.transform.localScale = new Vector2(0, 0);
        obj.transform.DOScale(Vector3.one * size, time).SetEase(Ease.Linear);
        Color objColor = obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
        objColor.a = 0;
        obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = objColor;
        obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(1, time);
    }



    void Skill1()
    {
        int leftRan = Random.Range(0, 2);
        int rightRan = Random.Range(2, 4);
        StartCoroutine(Skill1Main(0, leftRan, rightRan));
        StartCoroutine(Skill1Main(2f, 1 - leftRan, 5 - rightRan));

        StartCoroutine(CooldownReset(8 + cooldownSkill1, 1));
    }
    IEnumerator Skill1Main(float delay, int leftRan, int rightRan)
    {
        /*
         * 오브제코드 날리기 -> 오브제코드 사라지고 큐브 생성 -> 애드포스코드 날리기 -> 애드포스코드 사라지고 큐브 발사
         */
        yield return new WaitForSeconds(delay);

        // 왼쪽에 Gameobject코드 날리기
        GameObject bossCodeGameobjectLeft = objectPoolLogic.MakeObj("BossCodeGameobject");
        bossCodeGameobjectLeft.transform.position = bossObject.transform.position;
        bossCodeGameobjectLeft.transform.DOMove(skillPointSkill1[leftRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeGameobjectLeft.transform.localScale = Vector3.zero;
        bossCodeGameobjectLeft.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.3f);

        // 오른쪽에 Gameobject코드 날리기
        GameObject bossCodeGameobjectRight = objectPoolLogic.MakeObj("BossCodeGameobject");
        bossCodeGameobjectRight.transform.position = bossObject.transform.position;
        bossCodeGameobjectRight.transform.DOMove(skillPointSkill1[rightRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeGameobjectRight.transform.localScale = Vector3.zero;
        bossCodeGameobjectRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.7f);

        // 왼쪽 코드 사라지기 = 큐브 나오기 (계속 회전시키기)
        bossCodeGameobjectLeft.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeGameobjectLeft.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        GameObject bossCubeLeft = objectPoolLogic.MakeObj("BossCube");
        bossCubeLeft.transform.position = new Vector3(skillPointSkill1[leftRan].position.x, skillPointSkill1[leftRan].position.y, 1.5f);
        bossCubeLeft.transform.GetChild(0).transform.DORotate(new Vector3(0, -2400), 10f).SetEase(Ease.Linear).SetRelative();
        bossCubeLeft.transform.localScale = Vector3.zero;
        bossCubeLeft.transform.DOScale(Vector3.one, 0.3f);
        // 오른쪽 코드 사라지기 = 큐브 나오기 (계속 회전시키기)
        bossCodeGameobjectRight.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeGameobjectRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        GameObject bossCubeRight = objectPoolLogic.MakeObj("BossCube");
        bossCubeRight.transform.position = new Vector3(skillPointSkill1[rightRan].position.x, skillPointSkill1[rightRan].position.y, 1.5f);
        bossCubeRight.transform.GetChild(0).transform.DORotate(new Vector2(0, 2400), 10f).SetEase(Ease.Linear).SetRelative();
        bossCubeRight.transform.localScale = Vector3.zero;
        bossCubeRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.7f);

        // 왼쪽에 AddForce코드 날리기
        GameObject bossCodeAddForceLeft = objectPoolLogic.MakeObj("BossCodeAddForce");
        bossCodeAddForceLeft.transform.position = bossObject.transform.position;
        bossCodeAddForceLeft.transform.DOMove(skillPointSkill1[leftRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeAddForceLeft.transform.localScale = Vector3.zero;
        bossCodeAddForceLeft.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(0.3f);

        // 오른쪽에 AddForce코드 날리기, 이 시점보다 0.3초 늦게하면 보기 조흠
        GameObject bossCodeAddForceRight = objectPoolLogic.MakeObj("BossCodeAddForce");
        bossCodeAddForceRight.transform.position = bossObject.transform.position;
        bossCodeAddForceRight.transform.DOMove(skillPointSkill1[rightRan].position, 0.3f).SetEase(Ease.OutCubic);
        bossCodeAddForceRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.3f).SetEase(Ease.Linear);
        bossCodeAddForceRight.transform.localScale = Vector3.zero;
        bossCodeAddForceRight.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(1f);

        // 왼쪽 AddForce코드 사라지기 = 큐브 날리기
        bossCodeAddForceLeft.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeAddForceLeft.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        Rigidbody2D bossCubeLeftRigid = bossCubeLeft.GetComponent<Rigidbody2D>();
        bossCubeLeftRigid.AddForce(Vector2.left * -7f, ForceMode2D.Impulse);
        // 오른쪽 AddForce코드 사라지기 = 큐브 날리기
        bossCodeAddForceRight.transform.DOScale(new Vector2(2, 2), 0.3f);
        bossCodeAddForceRight.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
        Rigidbody2D bossCubeRightRigid = bossCubeRight.GetComponent<Rigidbody2D>();
        bossCubeRightRigid.AddForce(Vector2.left * 7f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3f);

        // 사용한 큐브들 초기화, 비활성화
        ResetObjectInfo(bossCodeGameobjectLeft);
        ResetObjectInfo(bossCodeAddForceLeft);
        ResetObjectInfo(bossCodeGameobjectRight);
        ResetObjectInfo(bossCodeAddForceRight);
    }

    void Skill2()
    {
        StartCoroutine(Skill2Main());
        StartCoroutine(CooldownReset(7.75f + cooldownSkill2, 2));
    }
    IEnumerator Skill2Main()
    {
        GameObject[] ball = new GameObject[9];
        GameObject[] codeGameobject = new GameObject[9];
        GameObject[] codeAddForce = new GameObject[9];

        // 0.15초마다 코드 4번 던지기
        for (int i = 0; i < 4; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = bossObject.transform.position;
            codeGameobject[i].transform.DOMove(skillPointSkill2[i].position, 0.3f);
            codeGameobject[i].transform.localScale = Vector3.zero;
            codeGameobject[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.15f);
        }
        // 코드 사라지고 공 생기기 + AddForce코드 날리기
        for (int i = 0; i < 4; i++)
        {
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.3f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            ball[i] = objectPoolLogic.MakeObj("BossBall");
            ball[i].transform.position = new Vector3(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y, 2);
            ball[i].transform.localScale = Vector3.zero;
            ball[i].transform.DOScale(Vector3.one * 0.5f, 0.3f);
            yield return new WaitForSeconds(0.1f);
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = bossObject.transform.position;
            codeAddForce[i].transform.DOMove(new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y), 0.3f);
            codeAddForce[i].transform.localScale = Vector3.zero;
            codeAddForce[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(0.5f);
        // AddForce코드 사라지고 공 날리기
        for (int i = 0; i < 4; i++)
        {
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.3f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            Vector2 ballDir = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y)
                                            - new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            Rigidbody2D ballRigid = ball[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(ballDir.normalized * 25f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 4; i < 9; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = bossObject.transform.position;
            codeGameobject[i].transform.DOMove(skillPointSkill2[i].position, 0.3f);
            codeGameobject[i].transform.localScale = Vector3.zero;
            codeGameobject[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.15f);
        }
        // 위의 행동을 4번이 아닌 5번으로
        for (int i = 4; i < 9; i++)
        {
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.3f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            ball[i] = objectPoolLogic.MakeObj("BossBall");
            ball[i].transform.position = new Vector3(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y, 2);
            ball[i].transform.localScale = Vector3.zero;
            ball[i].transform.DOScale(Vector3.one * 0.5f, 0.3f);
            yield return new WaitForSeconds(0.1f);
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = bossObject.transform.position;
            codeAddForce[i].transform.DOMove(new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y), 0.3f);
            codeAddForce[i].transform.localScale = Vector3.zero;
            codeAddForce[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 4; i < 9; i++)
        {
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.3f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            Vector2 ballDir = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y)
                                            - new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            Rigidbody2D ballRigid = ball[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(ballDir.normalized * 25f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.15f);
        }

        // 정보 초기화, 비활성화
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < 9; i++)
        {
            ResetObjectInfo(codeAddForce[i]);
            ResetObjectInfo(codeGameobject[i]);
        }
    }

    void Skill3()
    {
        StartCoroutine(Skill3Main(0));
        StartCoroutine(Skill3Main(2));
        StartCoroutine(Skill3Main(4));
        StartCoroutine(Skill3Main(6));

        StartCoroutine(CooldownReset(8.55f + cooldownSkill3, 3));
    }
    IEnumerator Skill3Main(float dealy)
    {
        yield return new WaitForSeconds(dealy);

        // list에 0~9 숫자 생성후 랜덤으로 4개제거
        List<int> list = new List<int>();
        for (int i = 0; i < skillPointSkill3.Length; i++)
        {
            list.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            list.RemoveAt(Random.Range(0, list.Count));
        }

        GameObject[] cube = new GameObject[list.Count];
        GameObject[] codeGameobject = new GameObject[list.Count];
        GameObject[] codeAddForce = new GameObject[list.Count];

        // 코드 던지기
        for (int i = 0; i < list.Count; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = bossObject.transform.position;
            codeGameobject[i].transform.DOMove(skillPointSkill3[list[i]].position, 0.3f);
            codeGameobject[i].transform.localScale = Vector3.zero;
            codeGameobject[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        // 해당 자리에 코드 없애고 큐브 생성
        for (int i = 0; i < list.Count; i++)
        {
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            cube[i] = objectPoolLogic.MakeObj("BossCube");
            cube[i].transform.position = new Vector3(skillPointSkill3[list[i]].position.x, skillPointSkill3[list[i]].position.y, 2);
            cube[i].transform.GetChild(0).transform.DORotate(new Vector3(-2400, 0, 0), 10f).SetEase(Ease.Linear).SetRelative();
            cube[i].transform.localScale = Vector3.zero;
            cube[i].transform.DOScale(Vector3.one * 1.2f, 0.3f);
        }
        yield return new WaitForSeconds(0.5f);

        // 0.15초마다 코드 4번 던지기
        for (int i = 0; i < list.Count; i++)
        {
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = bossObject.transform.position;
            codeAddForce[i].transform.DOMove(skillPointSkill3[list[i]].position, 0.3f);
            codeAddForce[i].transform.localScale = Vector3.zero;
            codeAddForce[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.35f);

        // 코드 사라지고 공 떨구기
        for (int i = 0; i < list.Count; i++)
        {
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            Rigidbody2D ballRigid = cube[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
        }

        // 정보 초기화, 비활성화
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < list.Count; i++)
        {
            ResetObjectInfo(codeAddForce[i]);
            ResetObjectInfo(codeGameobject[i]);
        }
    }

    void Skill4()
    {
        StartCoroutine(Skill4Main());
        StartCoroutine(CooldownReset(3.6f + cooldownSkill4, 4));
    }
    IEnumerator Skill4Main()
    {
        GameObject[] ball = new GameObject[12];
        GameObject[] codeGameobject = new GameObject[12];
        GameObject[] codeAddForce = new GameObject[12];
        // 코드 던지기
        for (int i = 0; i < ball.Length; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = bossObject.transform.position;
            Vector2 vec = new Vector2(skillPointSkill4[0].position.x + Mathf.Sin(i * 2f / ball.Length * Mathf.PI) * 2,
                                      skillPointSkill4[0].position.y + Mathf.Cos(i * 2f / ball.Length * Mathf.PI) * 2);
            codeGameobject[i].transform.DOMove(vec, 0.3f);
            codeGameobject[i].transform.localScale = Vector3.zero;
            codeGameobject[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // 코드 사라지고 공나오기
        for (int i = 0; i < ball.Length; i++)
        {
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            ball[i] = objectPoolLogic.MakeObj("BossBall");
            Vector3 vec = new Vector3(skillPointSkill4[0].position.x + Mathf.Sin(i * 2f / ball.Length * Mathf.PI) * 2,
                                      skillPointSkill4[0].position.y + Mathf.Cos(i * 2f / ball.Length * Mathf.PI) * 2, 2);
            ball[i].transform.position = vec;
            ball[i].transform.localScale = Vector3.zero;
            ball[i].transform.DOScale(Vector3.one * 0.5f, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // 코드 던지기
        for (int i = 0; i < ball.Length; i++)
        {
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = bossObject.transform.position;
            Vector2 vec = new Vector2(skillPointSkill4[0].position.x + Mathf.Sin((i * 3 + i / 4) / 6f * Mathf.PI) * 2,
                                      skillPointSkill4[0].position.y + Mathf.Cos((i * 3 + i / 4) / 6f * Mathf.PI) * 2);
            codeAddForce[i].transform.DOMove(vec, 0.3f);
            codeAddForce[i].transform.localScale = Vector3.zero;
            codeAddForce[i].transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // 코드 사라지고 공 날리기
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                // 3 * j + i {0, 3, 6, 9, 1, 4, 7, 10, 2, 5, 8, 11}
                codeAddForce[i * 4 + j].transform.DOScale(new Vector2(2, 2), 0.3f);
                codeAddForce[i * 4 + j].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
                Vector2 vec = new Vector2(Mathf.Sin((3 * j + i) / 6f * Mathf.PI),
                                          Mathf.Cos((3 * j + i) / 6f * Mathf.PI));
                Rigidbody2D ballRigid = ball[3 * j + i].GetComponent<Rigidbody2D>();
                ballRigid.AddForce(vec * 15f, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < ball.Length; i++)
        {
            ResetObjectInfo(codeGameobject[i]);
            ResetObjectInfo(codeAddForce[i]);
        }
    }

    void Skill5()
    {
        StartCoroutine(Skill5Main());
        StartCoroutine(CooldownReset(cooldownSkillGravity, 5));
    }
    IEnumerator Skill5Main()
    {
        float posX = Random.Range(-70, 71) / 10f;
        GameObject codeGravity = objectPoolLogic.MakeObj("BossCodeGravity");
        ThrowCode(codeGravity, new Vector2(posX, -4.5f), 0.3f);
        yield return new WaitForSeconds(1f);

        UseCode(codeGravity, 0.3f);
        GameObject gravityArea = objectPoolLogic.MakeObj("BossGravityArea");
        gravityArea.transform.position = new Vector2(posX, -4.5f);
        gravityArea.transform.localScale = new Vector2(0, 1);
        gravityArea.transform.DOScaleX(1, 2f).SetEase(gravityAreaEase);
        yield return new WaitForSeconds(5f);

        gravityArea.transform.DOScaleX(0, 0.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);

        ResetObjectInfo(codeGravity);
        gravityArea.SetActive(false);
    }




    void UltSkill1()
    {
        int leftRan = Random.Range(0, 2);
        int rightRan = Random.Range(2, 4);
        StartCoroutine(UltSkill1Main(0, leftRan, rightRan));
        StartCoroutine(UltSkill1Main(1f, 1 - leftRan, 5 - rightRan));
        leftRan = Random.Range(0, 2);
        rightRan = Random.Range(2, 4);
        StartCoroutine(UltSkill1Main(2.5f, leftRan, rightRan));
        StartCoroutine(UltSkill1Main(3.5f, 1 - leftRan, 5 - rightRan));
    }
    IEnumerator UltSkill1Main(float delay, int leftRan, int rightRan)
    {
        yield return new WaitForSeconds(delay);

        // 왼쪽에 Gameobject코드 생성
        GameObject codeGameobjectLeft = objectPoolLogic.MakeObj("BossCodeGameobject");
        CreateCode(codeGameobjectLeft,
                   skillPointSkill1[leftRan].position,
                   0.2f);
        // 오른쪽에 Gameobject코드 생성
        GameObject codeGameobjectRight = objectPoolLogic.MakeObj("BossCodeGameobject");
        CreateCode(codeGameobjectRight,
                   skillPointSkill1[rightRan].position,
                   0.2f);
        yield return new WaitForSeconds(0.5f);

        // 왼쪽 코드 사라지기 = 큐브 나오기 (계속 회전시키기)
        UseCode(codeGameobjectLeft, 0.2f);
        GameObject cubeLeft = objectPoolLogic.MakeObj("BossCube");
        CreateObject(cubeLeft,
                     skillPointSkill1[leftRan].position,
                     1f, 0.2f);
        cubeLeft.transform.GetChild(0).transform.DORotate(new Vector3(0, -3000), 10f).SetEase(Ease.Linear).SetRelative();
        // 오른쪽 코드 사라지기 = 큐브 나오기 (계속 회전시키기)
        UseCode(codeGameobjectRight, 0.2f);
        GameObject cubeRight = objectPoolLogic.MakeObj("BossCube");
        CreateObject(cubeRight,
                     skillPointSkill1[rightRan].position,
                     1f, 0.2f);
        cubeRight.transform.GetChild(0).transform.DORotate(new Vector3(0, 3000), 10f).SetEase(Ease.Linear).SetRelative();
        yield return new WaitForSeconds(0.5f);

        // 왼쪽에 AddForce코드 생성
        GameObject codeAddForceLeft = objectPoolLogic.MakeObj("BossCodeAddForce");
        CreateCode(codeAddForceLeft,
                   skillPointSkill1[leftRan].position,
                   0.2f);
        // 오른쪽에 AddForce코드 생성
        GameObject codeAddForceRight = objectPoolLogic.MakeObj("BossCodeAddForce");
        CreateCode(codeAddForceRight,
                   skillPointSkill1[rightRan].position,
                   0.2f);
        yield return new WaitForSeconds(0.5f);

        // 왼쪽 AddForce코드 사라지기 = 큐브 날리기
        UseCode(codeAddForceLeft, 0.2f);
        Rigidbody2D bossCubeLeftRigid = cubeLeft.GetComponent<Rigidbody2D>();
        bossCubeLeftRigid.AddForce(Vector2.left * -12f, ForceMode2D.Impulse);
        // 오른쪽 AddForce코드 사라지기 = 큐브 날리기
        UseCode(codeAddForceRight, 0.2f);
        Rigidbody2D bossCubeRightRigid = cubeRight.GetComponent<Rigidbody2D>();
        bossCubeRightRigid.AddForce(Vector2.left * 12f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(3f);

        // 사용한 큐브들 초기화, 비활성화
        ResetObjectInfo(codeGameobjectLeft);
        ResetObjectInfo(codeAddForceLeft);
        ResetObjectInfo(codeGameobjectRight);
        ResetObjectInfo(codeAddForceRight);
    }

    void UltSkill2()
    {
        StartCoroutine(UltSkill2Main(0));
        StartCoroutine(UltSkill2Main(2.5f));
    }
    IEnumerator UltSkill2Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ball = new GameObject[9];
        GameObject[] codeGameobject = new GameObject[9];
        GameObject[] codeAddForce = new GameObject[9];

        for (int i = 0; i < 4; i++)
        {
            // [4탄] Gameobject 코드 생성
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = skillPointSkill2[i].position;
            codeGameobject[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeGameobject[i].transform.localScale = new Vector2(0, 0);
            codeGameobject[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeGameobject[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 4; i++)
        {
            // [4탄] Gameobject 코드 사라지고 공 생성
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            ball[i] = objectPoolLogic.MakeObj("BossBall");
            ball[i].transform.position = new Vector3(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y, 2);
            ball[i].transform.localScale = Vector3.zero;
            ball[i].transform.DOScale(Vector3.one * 0.5f, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 4; i++)
        {
            // [4탄] AddForce, LookAt 코드 생성
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            codeAddForce[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeAddForce[i].transform.localScale = new Vector2(0, 0);
            codeAddForce[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeAddForce[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 4; i++)
        {
            // [4탄] 두 코드 순서대로 사라지고 플레이어에게 공 날리기
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            Vector2 ballDir = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y)
                                            - new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            Rigidbody2D ballRigid = ball[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(ballDir.normalized * 30f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.15f);
        }
        for (int i = 4; i < 9; i++)
        {
            // [5탄] Gameobject코드 생성
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = skillPointSkill2[i].position;
            codeGameobject[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeGameobject[i].transform.localScale = new Vector2(0, 0);
            codeGameobject[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeGameobject[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 4; i < 9; i++)
        {
            // [5탄] Gameobject 코드 사라지고 공 생성
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            ball[i] = objectPoolLogic.MakeObj("BossBall");
            ball[i].transform.position = new Vector3(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y, 2);
            ball[i].transform.localScale = Vector3.zero;
            ball[i].transform.DOScale(Vector3.one * 0.5f, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 4; i < 9; i++)
        {
            // [5탄] AddForce, LookAt 코드 생성
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            codeAddForce[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeAddForce[i].transform.localScale = new Vector2(0, 0);
            codeAddForce[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeAddForce[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 4; i < 9; i++)
        {
            // [5탄] 두 코드 순서대로 사라지고 플레이어에게 공 날리기
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            Vector2 ballDir = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y)
                                            - new Vector2(skillPointSkill2[i].position.x, skillPointSkill2[i].position.y);
            Rigidbody2D ballRigid = ball[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(ballDir.normalized * 30f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.15f);
        }

        // 정보 초기화, 비활성화
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 9; i++)
        {
            ResetObjectInfo(codeAddForce[i]);
            ResetObjectInfo(codeGameobject[i]);
        }
    }

    void UltSkill3()
    {
        StartCoroutine(UltSkill3Main(0));
        StartCoroutine(UltSkill3Main(0.8f));
        StartCoroutine(UltSkill3Main(1.6f));
        StartCoroutine(UltSkill3Main(2.4f));
        StartCoroutine(UltSkill3Main(3.2f));
    }
    IEnumerator UltSkill3Main(float dealy)
    {
        yield return new WaitForSeconds(dealy);

        // list에 0~7 숫자 생성후 랜덤으로 제거
        List<int> list = new List<int>();
        for (int i = 0; i < skillPointSkill3.Length; i++)
        {
            list.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            list.RemoveAt(Random.Range(0, list.Count));
        }

        GameObject[] cube = new GameObject[list.Count];
        GameObject[] codeGameobject = new GameObject[list.Count];
        GameObject[] codeAddForce = new GameObject[list.Count];

        // 코드 생성
        for (int i = 0; i < list.Count; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            codeGameobject[i].transform.position = skillPointSkill3[list[i]].position;
            codeGameobject[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeGameobject[i].transform.localScale = new Vector2(0, 0);
            codeGameobject[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeGameobject[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.4f);

        // 해당 자리에 코드 없애고 큐브 생성
        for (int i = 0; i < list.Count; i++)
        {
            codeGameobject[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            cube[i] = objectPoolLogic.MakeObj("BossCube");
            cube[i].transform.position = new Vector3(skillPointSkill3[list[i]].position.x, skillPointSkill3[list[i]].position.y, 3);
            cube[i].transform.GetChild(0).DORotate(new Vector3(-2600, 0, 0), 10f).SetEase(Ease.Linear).SetRelative();
            cube[i].transform.localScale = Vector3.zero;
            cube[i].transform.DOScale(Vector3.one * 1.2f, 0.2f);
        }
        yield return new WaitForSeconds(0.4f);

        // 코드 생성
        for (int i = 0; i < list.Count; i++)
        {
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            codeAddForce[i].transform.position = skillPointSkill3[list[i]].position;
            codeAddForce[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            codeAddForce[i].transform.localScale = new Vector2(0, 0);
            codeAddForce[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
            codeAddForce[i].transform.DOScale(new Vector2(1, 1), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        }
        yield return new WaitForSeconds(0.4f);

        // 코드 사라지고 공 떨구기
        for (int i = 0; i < list.Count; i++)
        {
            codeAddForce[i].transform.DOScale(new Vector2(2, 2), 0.2f);
            codeAddForce[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            Rigidbody2D ballRigid = cube[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(Vector2.down * 15f, ForceMode2D.Impulse);
        }

        // 정보 초기화, 비활성화
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < list.Count; i++)
        {
            ResetObjectInfo(codeAddForce[i]);
            ResetObjectInfo(codeGameobject[i]);
        }
    }

    void UltSkill4()
    {
        // 딜레이, 중심위치(0,1,2), 회전값(0,1,2), 구간별 딜레이
        for (int i = 0; i < 18; i++)
        {
            StartCoroutine(UltSkill4Main(0.5f * i, 0, 2 - i % 3, 0.5f));
        }
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine(UltSkill4Main(3 + 0.5f * i, 1, i % 3, 0.5f));
        }
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine(UltSkill4Main(3 + 0.5f * i, 2, i % 3, 0.5f));
        }
    }
    IEnumerator UltSkill4Main(float startDelay, int mainPosition, int dir, float delay)
    {
        // position 0 = 가운데, 1 = 왼쪽, 2 = 오른쪽
        // dir 1 = 시계방향, -1 = 반시계방향
        yield return new WaitForSeconds(startDelay);

        GameObject[] ball = new GameObject[4];
        GameObject[] codeGameobject = new GameObject[4];
        GameObject[] codeAddForce = new GameObject[4];

        // Gameobject코드 생성
        for (int i = 0; i < 4; i++)
        {
            codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
            Vector2 createPos = new Vector2(skillPointSkill4[mainPosition].position.x + Mathf.Sin((i * 3 + dir) / 6f * Mathf.PI) * 2,
                                                 skillPointSkill4[mainPosition].position.y + Mathf.Cos((i * 3 + dir) / 6f * Mathf.PI) * 2);
            CreateCode(codeGameobject[i],
                       createPos,
                       0.2f);
        }
        yield return new WaitForSeconds(delay);

        // 코드 사라지고 공나오기
        for (int i = 0; i < 4; i++)
        {
            UseCode(codeGameobject[i], 0.2f);

            ball[i] = objectPoolLogic.MakeObj("BossBall");
            Vector2 createPos = new Vector2(skillPointSkill4[mainPosition].position.x + Mathf.Sin((i * 3 + dir) / 6f * Mathf.PI) * 2,
                                            skillPointSkill4[mainPosition].position.y + Mathf.Cos((i * 3 + dir) / 6f * Mathf.PI) * 2);
            CreateObject(ball[i], createPos, 0.5f, 0.2f);
        }
        yield return new WaitForSeconds(delay);

        // AddForce코드 생성
        for (int i = 0; i < 4; i++)
        {
            codeAddForce[i] = objectPoolLogic.MakeObj("BossCodeAddForce");
            Vector2 createPos = new Vector2(skillPointSkill4[mainPosition].position.x + Mathf.Sin((i * 3 + dir) / 6f * Mathf.PI) * 2,
                                            skillPointSkill4[mainPosition].position.y + Mathf.Cos((i * 3 + dir) / 6f * Mathf.PI) * 2);
            CreateCode(codeAddForce[i],
                       createPos,
                       0.2f);
        }
        yield return new WaitForSeconds(delay);

        // 코드 사라지고 공 발사
        for (int i = 0; i < 4; i++)
        {
            UseCode(codeAddForce[i], 0.2f);
            Vector2 vec = new Vector2(Mathf.Sin((i * 3 + dir) / 6f * Mathf.PI),
                                      Mathf.Cos((i * 3 + dir) / 6f * Mathf.PI));
            Rigidbody2D ballRigid = ball[i].GetComponent<Rigidbody2D>();
            ballRigid.AddForce(vec * 15f, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 4; i++)
        {
            ResetObjectInfo(ball[i]);
            ResetObjectInfo(codeGameobject[i]);
            ResetObjectInfo(codeAddForce[i]);
        }
    }
    //IEnumerator UltSkill4Main(float delay, int createPosition, int dir, int playNumber)
    //{
    //    // position 0 = 가운데, 1 = 왼쪽, 2 = 오른쪽
    //    // dir 1 = 시계방향, -1 = 반시계방향
    //    yield return new WaitForSeconds(delay);

    //    GameObject[] ball = new GameObject[playNumber * 4];
    //    GameObject[] codeGameobject = new GameObject[playNumber * 4];
    //    GameObject[] codeAddForce = new GameObject[playNumber * 4];
    //    // 코드 생성
    //    for (int i = 0; i < ball.Length; i++)
    //    {
    //        codeGameobject[i] = objectPoolLogic.MakeObj("BossCodeGameobject");
    //        codeGameobject[i].transform.position = new Vector2(skillPointSkill4[createPosition].position.x + Mathf.Sin(i * 2f / ball.Length * Mathf.PI) * 2,
    //                                                           skillPointSkill4[createPosition].position.y + Mathf.Cos(i * 2f / ball.Length * Mathf.PI) * 2);
    //        codeGameobject[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //        codeGameobject[i].transform.localScale = new Vector2(0, 0);
    //        codeGameobject[i].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
    //        codeGameobject[i].transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f);
    //        codeGameobject[i].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
    //    }
    //    yield return new WaitForSeconds(0.5f);
    //    // 코드 사라지고 공나오기
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            codeGameobject[j + i * 4].transform.DOScale(new Vector2(1.6f, 1.6f), 0.2f);
    //            codeGameobject[j + i * 4].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
    //            ball[j + i * 4] = objectPoolLogic.MakeObj("BossBall");
    //            ball[j + i * 4].transform.position = new Vector3(skillPointSkill4[createPosition].position.x + Mathf.Sin((j * 3 + i) / 6f * Mathf.PI) * 2 * dir,
    //                                                             skillPointSkill4[createPosition].position.y + Mathf.Cos((j * 3 + i) / 6f * Mathf.PI) * 2, 2);
    //            ball[j + i * 4].transform.localScale = Vector3.zero;
    //            ball[j + i * 4].transform.DOScale(Vector3.one * 0.5f, 0.2f);
    //        }
    //    }
    //    yield return new WaitForSeconds(0.5f);
    //    // 코드 생성
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            // j * 3 + i {0, 3, 6, 9, 1, 4, 7, 10, 2, 5, 8, 11}
    //            // j + i * 4 {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}
    //            codeAddForce[j + i * 4] = objectPoolLogic.MakeObj("BossCodeAddForce");
    //            codeAddForce[j + i * 4].transform.position = new Vector2(skillPointSkill4[createPosition].position.x + Mathf.Sin((j * 3 + i) / 6f * Mathf.PI) * 2 * dir,
    //                                                                     skillPointSkill4[createPosition].position.y + Mathf.Cos((j * 3 + i) / 6f * Mathf.PI) * 2);
    //            codeAddForce[j + i * 4].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //            codeAddForce[j + i * 4].transform.localScale = new Vector2(0, 0);
    //            codeAddForce[j + i * 4].transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetEase(Ease.Linear).SetRelative();
    //            codeAddForce[j + i * 4].transform.DOScale(new Vector2(0.8f, 0.8f), 0.2f);
    //            codeAddForce[j + i * 4].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
    //        }
    //        yield return new WaitForSeconds(0.3f);
    //    }
    //    // 코드 사라지고 공 날리기
    //    yield return new WaitForSeconds(0.2f);
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            codeAddForce[j + i * 4].transform.DOScale(new Vector2(2, 2), 0.3f);
    //            codeAddForce[j + i * 4].transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
    //            Vector2 vec = new Vector2(Mathf.Sin((j * 3 + i) / 6f * Mathf.PI * dir),
    //                                      Mathf.Cos((j * 3 + i) / 6f * Mathf.PI * dir));
    //            Rigidbody2D ballRigid = ball[j + i * 4].GetComponent<Rigidbody2D>();
    //            ballRigid.AddForce(vec * 15f, ForceMode2D.Impulse);
    //        }
    //        yield return new WaitForSeconds(0.5f);
    //    }

    //    yield return new WaitForSeconds(0.8f);
    //    for (int i = 0; i < ball.Length; i++)
    //    {
    //        ResetObjectInfo(ball[i]);
    //        ResetObjectInfo(codeGameobject[i]);
    //        ResetObjectInfo(codeAddForce[i]);
    //    }
    //}

    void UltSkillSpecial()
    {
        StartCoroutine(UltSkillSpecialMain(0));
        StartCoroutine(UltSkillSpecialMain(10));
    }
    IEnumerator UltSkillSpecialMain(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject codeRevers = objectPoolLogic.MakeObj("BossCodeRevers");
        CreateCode(codeRevers, new Vector2(0, 3), 0.3f);
        yield return new WaitForSeconds(1f);

        codeRevers.transform.DOScale(Vector2.one * 12, 1f).SetEase(Ease.Linear);
        codeRevers.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.5f).SetEase(Ease.Linear);
        Camera.main.transform.DOShakePosition(0.5f, 0.5f);
        PlayerController playerLogic = player.GetComponent<PlayerController>();
        playerLogic.reversDir *= -1;
        Debug.Log(playerLogic.reversDir);
        yield return new WaitForSeconds(1f);

        ResetObjectInfo(codeRevers);
    }

    void UltSkillSummon()
    {
        StartCoroutine(UltSkillSummonMain(0));
    }
    IEnumerator UltSkillSummonMain(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 소환코드 생성
        GameObject codeSummon = objectPoolLogic.MakeObj("BossCodeSummon");
        codeSummon.transform.position = skillPointSkill4[0].position; // 패턴4의 중앙자리 사용
        codeSummon.transform.localScale = Vector2.zero;
        codeSummon.transform.DOScale(Vector2.one, 0.5f);
        Color objColor = codeSummon.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        objColor.a = 0;
        codeSummon.transform.GetChild(0).GetComponent<SpriteRenderer>().color = objColor;
        codeSummon.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 0.5f);

        yield return new WaitForSeconds(1.5f);
        // 소환코드 게이지 채우기, 세미보스 천천히 등장
        GameObject codeSummonPivot = codeSummon.transform.GetChild(1).gameObject;
        codeSummonPivot.transform.localScale = new Vector2(0, 1);
        codeSummonPivot.transform.DOScaleX(1, 2f);
        semiBossObject = objectPoolLogic.MakeObj("BossSummoner");
        semiBossObject.transform.position = skillPointSkill4[0].position;
        semiBossObject.transform.DOMoveY(0.6f, 3f).SetRelative().SetLoops(-1, LoopType.Restart).SetEase(semiBossEase);
        semiBossObject.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0);
        semiBossObject.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 2f);

        yield return new WaitForSeconds(2f);
        // 소환코드 사라지고 둥둥 떠다니기
        codeSummon.transform.DOScale(new Vector2(2, 2), 0.3f).SetEase(Ease.Linear);
        codeSummon.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0f).SetEase(Ease.Linear);
        codeSummonPivot.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, 0.3f).SetEase(Ease.Linear);


        yield return new WaitForSeconds(1f);
        ResetObjectInfo(codeSummonPivot);
        ResetObjectInfo(codeSummon);
    }


    IEnumerator SummonSkill1()
    {
        semiBossObject.transform.DORotate(new Vector3(0, 0, 720), 3f).SetEase(Ease.Linear).SetRelative();
        for (int i = 0; i < (int)3 / 0.15; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject bullet = objectPoolLogic.MakeObj("BossBall");
                bullet.transform.localScale = Vector3.one * 0.3f;
                bullet.transform.right = semiBossObject.transform.right;
                bullet.transform.Rotate(0, 0, j * 90);
                bullet.transform.position = semiBossObject.transform.position + bullet.transform.right;
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                bulletRigid.AddForce(bullet.transform.right * 7f, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
    IEnumerator SummonSkill2()
    {
        Vector2 dir = player.transform.position - semiBossObject.transform.position;
        dir = dir.normalized;

        for (int j = 0; j < 5; j++)
        {
            GameObject bullet = objectPoolLogic.MakeObj("BossBall");
            bullet.transform.localScale = Vector3.one * 0.3f;
            bullet.transform.up = dir;
            bullet.transform.position = semiBossObject.transform.position + new Vector3(dir.x, dir.y) * 1f;
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            bulletRigid.AddForce(bullet.transform.up * 10f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator SummonSkill3()
    {
        for (int j = 0; j < vertex * vertexCount; j++)
        {
            GameObject bullet = objectPoolLogic.MakeObj("BossBall");
            bullet.transform.localScale = Vector3.one * 0.3f;
            bullet.transform.up = Vector2.up;
            bullet.transform.position = semiBossObject.transform.position;
            bullet.transform.Rotate(new Vector3(0, 0, 180 / (float)vertex / (float)vertexCount));

            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Debug.Log(360 / (float)vertex / (float)vertexCount * j);
            bulletRigid.AddForce(bullet.transform.up / Mathf.Sin(360 / (float)vertex / (float)vertexCount * j) * 5f, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator Think()
    {
        if (bossHealth <= 0)
            yield break;
        if (Pattern.Count == 0)
            Pattern = new List<int> { 1, 2, 3, 4, 5 };

        int ran = Random.Range(0, Pattern.Count);
        switch (Pattern[ran])
        {
            case 1:
                Skill1();
                break;
            case 2:
                Skill2();
                break;
            case 3:
                Skill3();
                break;
            case 4:
                Skill4();
                break;
            case 5:
                Skill5();
                break;
            default:
                break;
        }
        Pattern.RemoveAt(ran);
    }
    IEnumerator CooldownReset(float delay, int num)
    {
        yield return new WaitForSeconds(delay);
        Pattern.Add(num);
        StartCoroutine(Think());
    }
}