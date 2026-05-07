using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Boss_Gra_LeeVar : Boss_Gra
{
    private int maxBossHealth = 30;
    private int bossHealth = 30;

    private GameObject player;

    public Image healthBar;

    public ObjectPool_Graphic objectPoolLogic;

    public Transform[] createPos_UltSkill1;
    public Transform[] createPos_UltSkill2;
    public Transform[] createPos_BrushSkill;

    public Transform brushHomePos;

    public GameObject brush;
    private SpriteRenderer brushColorSprite;
    private GameObject[] brushPivot = new GameObject[2];
    private GameObject curBrushPivot;

    // 0: ¸Ó¸®, 1: ÇÚµé

    public void Hit()
    {
        if (bossHealth <= 0)
            return;

        bossHealth--;
        healthBar.fillAmount = 1 - bossHealth / (float)maxBossHealth;
        Debug.Log("Health: " + bossHealth);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        brushColorSprite = brush.transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        brushPivot[0] = brush.transform.GetChild(1).gameObject;
        brushPivot[1] = brush.transform.GetChild(2).gameObject;
        PivotChange("None");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Skill1();


        if (Input.GetKeyDown(KeyCode.Q))
            UltSkill1();
        if (Input.GetKeyDown(KeyCode.W))
            UltSkill2();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            BrushSkill1();
        if (Input.GetKeyDown(KeyCode.Keypad2))
            BrushSkill2();
        if (Input.GetKeyDown(KeyCode.Keypad3))
            BrushSkill3();
    }



    // ±âº»Àº None
    public void PivotChange(string type)
    {
        brush.transform.parent = null;
        brushPivot[0].transform.parent = null; // Çìµå
        brushPivot[1].transform.parent = null; // ¼ÕÀâÀÌ³¡
        switch (type)
        {
            case "None":
            case "Body":
                brushPivot[0].transform.parent = brush.transform;
                brushPivot[1].transform.parent = brush.transform;
                curBrushPivot = brush;
                break;
            case "Head":
                brush.transform.parent = brushPivot[0].transform;
                brushPivot[1].transform.parent = brush.transform;
                curBrushPivot = brushPivot[0];
                break;
            case "Handle":
                brush.transform.parent = brushPivot[1].transform;
                brushPivot[0].transform.parent = brush.transform;
                curBrushPivot = brushPivot[1];
                break;
            default:
                break;
        }
    }




    void Skill1()
    {
        StartCoroutine(Skill1Main(0, 3));
    }
    IEnumerator Skill1Main(float delay, int createNum)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] light = new GameObject[createNum];
        for (int i = 0; i < createNum; i++)
        {
            light[i] = objectPoolLogic.MakeObj("Boss_Graphic_Light");
            // »õ·Î¿î ¶óÀÌÆ®¿Í ±âÁ¸ÀÇ ¶óÀÌÆ® °Å¸®°Ë»ç¿ë
            while (true)
            {
                bool isNear = false;
                float ranPos = Random.Range(-80, 81) / 10f;
                // ÀÌ¹ø¶óÀÌÆ® ÀÌÀü¹øÈ£µé ´ë»ó
                for (int j = 0; j < i; j++)
                {
                    // ¸¸¾à °Ë»çÇÑ ¶óÀÌÆ®Áß °Å¸®°¡ 2º¸´Ù °¡±îÀÌÀÖÀ¸¸é ´Ù½ÃÇÏ±â
                    if (Mathf.Abs(ranPos - light[j].transform.position.x) < 2)
                        isNear = true;
                }
                // ¸¸¾à °Ë»çÇÑ ¶óÀÌÆ®Áß °¡±î¿î ¶óÀÌÆ®°¡ ¾Æ¹«°Íµµ ¾øÀ¸¸é Æ÷Áö¼Ç ¼³Á¤, ºüÁ®³ª°¡±â
                if (isNear == false)
                {
                    light[i].transform.position = new Vector2(ranPos, 4.5f);
                    break;
                }
            }
            light[i].transform.rotation = Quaternion.Euler(0, 0, Random.Range(-150, 151) / 10f);

            //Çìµå¶óÀÌÆ® »ý¼º
            light[i].transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(1, 1f);
            // ·¹ÀÌÀú(°æ·Î °æ°í) »ý¼º
            light[i].transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(1, 0.2f).SetDelay(1f);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2f);
        Camera.main.DOShakePosition(7, 0.03f);
        for (int i = 0; i < createNum; i++)
        {
            // ·¹ÀÌÀú Á¦°Å
            light[i].transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            // ºû »ý¼º, È¸Àü
            light[i].transform.GetChild(2).gameObject.SetActive(true);
            light[i].transform.GetChild(2).transform.DOScaleX(1.5f, 0.5f);
            if (light[i].transform.rotation.z <= 0)
                light[i].transform.DORotate(new Vector3(0, 0, 30), 4f).SetRelative().SetEase(Ease.Linear).SetDelay(2f);
            if (light[i].transform.rotation.z > 0)
                light[i].transform.DORotate(new Vector3(0, 0, -30), 4f).SetRelative().SetEase(Ease.Linear).SetDelay(2f);
            // 6ÃÊµÚ Çìµå¶óÀÌÆ® »ç¶óÁö±â
            light[i].transform.GetChild(2).transform.DOScaleX(0, 0.5f).SetDelay(6f);
            light[i].transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(0, 1f).SetDelay(7f);
        }

        yield return new WaitForSeconds(8f);
        // ºû ºñÈ°¼ºÈ­, ¿ÀºêÁ§Æ® ÀüÃ¼ ºñÈ°¼ºÈ­
        for (int i = 0; i < createNum; i++)
        {
            light[i].transform.GetChild(2).gameObject.SetActive(false);
            light[i].SetActive(false);
        }
    }




    // È­·ÁÇÑ Á¶¸íÀÌ ³ª¸¦ °¨½Î³×
    void UltSkill1()
    {
        StartCoroutine(UltSkill1Main(0));
    }
    IEnumerator UltSkill1Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] light = new GameObject[5];
        float ranRight = 1 - 2 * Random.Range(0, 2); // -1 or 1
        for (int i = 0; i < 5; i++)
        {
            int curNum = 1 - i % 2 * 2; // -1 or 1
            light[i] = objectPoolLogic.MakeObj("Boss_Graphic_Light");
            float ranPos = Random.Range(-15, 16) / 10f;
            light[i ].transform.position = createPos_UltSkill1[i].position + new Vector3(ranPos, 0);
            float ranDir = Random.Range(10, 150) / 10f;
            light[i].transform.rotation = Quaternion.Euler(0, 0, ranDir * ranRight * curNum);
            Debug.Log(ranRight * curNum);

            //Çìµå¶óÀÌÆ® »ý¼º
            light[i].transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(1, 1f);

            // ·¹ÀÌÀú(°æ·Î °æ°í) »ý¼º
            light[i].transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(1, 0.2f).SetDelay(1f);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f);
        Camera.main.DOShakePosition(7, 0.05f);
        for (int i = 0; i < 5; i++)
        {
            int curNum = 1 - i % 2 * 2; // -1 or 1
            // ·¹ÀÌÀú Á¦°Å
            light[i].transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            // ºû »ý¼º, È¸Àü
            light[i].transform.GetChild(2).gameObject.SetActive(true);
            light[i].transform.GetChild(2).transform.DOScaleX(1.5f, 0.5f);
            light[i].transform.DORotate(new Vector3(0, 0, -40 * curNum * ranRight), 4f).SetRelative().SetEase(Ease.Linear).SetDelay(2f);
            // 6ÃÊµÚ Çìµå¶óÀÌÆ® »ç¶óÁö±â   
            light[i].transform.GetChild(2).transform.DOScaleX(0, 0.5f).SetDelay(6f);
            light[i].transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(0, 1f).SetDelay(7f);
        }

        yield return new WaitForSeconds(8f);
        // ºû ºñÈ°¼ºÈ­, ¿ÀºêÁ§Æ® ÀüÃ¼ ºñÈ°¼ºÈ­
        for (int i = 0; i < 5; i++)
        {
            light[i].transform.GetChild(2).gameObject.SetActive(false);
            light[i].SetActive(false);
        }
    }
    // ¤¸¤¤È­·ÁÇÑ Á¶¸íÀÌ ³ª¸¦ °¨½Î³×
    void UltSkill2()
    {
        StartCoroutine(UltSkill2Main(0));
    }
    IEnumerator UltSkill2Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        // °ÔÀÓ ¿ÀºêÁ§Æ® ¼±¾ð ¹× ºò¶óÀÌÆ® »ý¼º, È¸Àü
        GameObject bigLight = objectPoolLogic.MakeObj("Boss_Graphic_BigLight");
        GameObject headLight = bigLight.transform.GetChild(0).gameObject;
        GameObject laser = bigLight.transform.GetChild(1).gameObject;
        GameObject light = bigLight.transform.GetChild(2).gameObject;
        GameObject lightTarget = bigLight.transform.GetChild(3).gameObject;
        int ranPos = Random.Range(0, 2);
        bigLight.transform.position = createPos_UltSkill2[ranPos].position;
        bigLight.transform.rotation = Quaternion.Euler(0, 0, 60 * (1 - 2 * ranPos));
        headLight.transform.GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // °æ°í ·¹ÀÌÀú »ý¼º
        laser.transform.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
        yield return new WaitForSeconds(0.5f);

        // ºû ¿øÆÇ ÀÌµ¿
        lightTarget.SetActive(true);
        lightTarget.transform.position = bigLight.transform.position;
        lightTarget.transform.localScale = new Vector2(2, 1);
        lightTarget.transform.DOMove(lightTarget.transform.up * -20, 2).SetRelative().SetEase(Ease.Linear);
        lightTarget.transform.DOScaleX(5, 2).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2);

        // °æ°í ·¹ÀÌÀú Á¦°Å, ºû »ý¼º
        laser.transform.GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
        light.SetActive(true);
        light.transform.DOScaleX(4, 0.5f);
        Camera.main.DOShakePosition(2, 0.2f);
        yield return new WaitForSeconds(2);

        // ºû Á¦°Å, ´Ù½Ã ºû ¿øÆÇ ÀÌµ¿
        light.transform.DOScaleX(0, 2f);
        lightTarget.transform.position = bigLight.transform.position;
        lightTarget.transform.localScale = new Vector2(2, 1);
        lightTarget.transform.DOMove(lightTarget.transform.up * -20, 2).SetRelative().SetEase(Ease.Linear);
        lightTarget.transform.DOScaleX(12, 1).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);

        // ºû ´õ Å©°Ô »ý¼º
        light.transform.DOScaleX(8, 0.5f);
        Camera.main.DOShakePosition(3, 0.35f);
        yield return new WaitForSeconds(3);

        // ºû ´Ù½Ã Á¦°Å
        light.transform.DOScaleX(0, 2f);
        yield return new WaitForSeconds(2);

        // ºû¿øÆÇ, ºû ºñÈ°¼ºÈ­, Çìµå¶óÀÌµå Á¦°Å
        lightTarget.SetActive(false);
        light.SetActive(false);
        headLight.transform.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // ºò¶óÀÌÆ® (ÀüÃ¼ ¿ÀºêÁ§Æ®) ºñÈ°¼ºÈ­
        bigLight.SetActive(false);
    }


    // º×ÀÇÈ£Èí Á¦ 1Çü º×·ÂÀÏ¼¶ 3¿¬: º×ÀÌ ÇÃ·¹ÀÌ¾î¸¦ ÇâÇØ ºü¸£°Ô µ¹Áø
    void BrushSkill1()
    {
        StartCoroutine(BrushSkill1Main(0));
    }
    IEnumerator BrushSkill1Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        curBrushPivot.transform.DORotate(new Vector3(0, 0, 360), 0.2f).SetRelative().SetEase(Ease.Linear);
        curBrushPivot.transform.DOMove(brushHomePos.position + new Vector3(2, 1), 0.2f);
        yield return new WaitForSeconds(0.2f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < 3; i++)
        {
            Vector3 dir = player.transform.position - brush.transform.position;

            float angle = Mathf.Atan2(dir.x, dir.y);
            float degree = -(angle * 180) / Mathf.PI;
            if (degree < 0)
                degree = 360 + degree;

            curBrushPivot.transform.DORotate(new Vector3(0, 0, degree + 360 - brush.transform.rotation.eulerAngles.z), 1500).SetRelative().SetSpeedBased().SetEase(Ease.Linear);
            yield return new WaitForSeconds(1);

            curBrushPivot.transform.DOMove(dir + brush.transform.up * 3, 30).SetRelative().SetSpeedBased();
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(0.5f);
        curBrushPivot.transform.DOMove(brushHomePos.position, 5).SetSpeedBased();
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.Linear);
    }
    
    // º×ÀÇÈ£Èí Á¦ 2Çü ÆRÈ÷ ¹°°¨ ¹ß½Î: ¿©·¯ »öÀÇ ¹°°¨À» ÇÏ³ª¾¿ ÇÃ·¹ÀÌ¾î¿¡°Ô ³¯¸°´Ù.
    void BrushSkill2()
    {
        StartCoroutine(BrushSkill2Main(0));
    }
    IEnumerator BrushSkill2Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        curBrushPivot.transform.DOMove(createPos_BrushSkill[0].position, 1f).SetEase(Ease.Linear);
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 180), 1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2);

        PivotChange("Handle");
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 60), 1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1.5f);

        GameObject[] paint = new GameObject[8];
        // ÆäÀÎÆ® ÇÏ³ª¾¿ »Ñ¸®±â
        for (int i = 0; i < 8; i++)
        {
            // 1 - i %2 * 2 = 1 or -1
            float rot = 240;

            // Ã³À½Àº ¹Ý½Ã°è·Î 300µµ È¸Àü, ÀÌÈÄ 240µµ¾¿ ½Ã°è, ¹Ý½Ã°è È¸ÀüÇÏ
            curBrushPivot.transform.DORotate(new Vector3(0, 0, rot * (1 - i%2 * 2)), 0.4f).SetRelative().SetEase(Ease.OutCubic);

            Vector3 dir = player.transform.position - curBrushPivot.transform.position;

            paint[i] = objectPoolLogic.MakeObj("Boss_Graphic_Paint");
            paint[i].transform.position = curBrushPivot.transform.position + dir.normalized;
            paint[i].transform.up = dir.normalized;
            paint[i].GetComponent<Rigidbody2D>().velocity = paint[i].transform.up * 15f;
            paint[i].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(i/8f, 1, 1);
            brushColorSprite.color = Color.HSVToRGB( i/ 8f, 1, 1);
            yield return new WaitForSeconds(1f - 0.1f * i);
        }
        yield return new WaitForSeconds(0.5f);


        // ÆäÀÎÆ® ÇÑ¹ø¿¡ »Ñ¸®±â
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 360), 0.5f).SetRelative().SetEase(Ease.OutCubic);
        brushColorSprite.color = Color.HSVToRGB(0, 1, 1);
        Camera.main.transform.DOShakePosition(0.2f, 0.1f);
        // ¿ì¼± Èò»öÀ¸·Î ÃµÃµÈ÷ »Ñ¸®±â
        for (int i = 0; i < 8; i++)
        {
            Vector3 dir = player.transform.position - curBrushPivot.transform.position;

            paint[i] = objectPoolLogic.MakeObj("Boss_Graphic_Paint");
            paint[i].transform.position = curBrushPivot.transform.position + Vector3.down;
            paint[i].transform.rotation = Quaternion.Euler(0, 0, - 105 - 150 / 7f * i);
            paint[i].GetComponent<Rigidbody2D>().velocity = paint[i].transform.up * 0.7f;
            paint[i].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(1 - i / 8f, 0.05f, 1);
        }
        yield return new WaitForSeconds(1f);
        curBrushPivot.transform.DORotate(new Vector3(0, 0, -360), 0.5f).SetRelative().SetEase(Ease.OutCubic);
        brushColorSprite.DOColor(Color.HSVToRGB(0.5f, 1, 1), 0.15f);
        brushColorSprite.DOColor(Color.HSVToRGB(1f, 1, 1), 0.15f).SetDelay(0.15f);
        Camera.main.transform.DOShakePosition(0.2f, 0.1f);
        // ÆäÀÎÆ®µé »öÀÔÈ÷°í ¼Óµµ Áõ°¡
        for (int i = 0; i < 8; i++)
        {
            paint[i].GetComponent<Rigidbody2D>().velocity = paint[i].transform.up * 7f;
            paint[i].GetComponent<SpriteRenderer>().DOColor(Color.HSVToRGB(1 - i / 8f, 1, 1), 0.5f);
        }

        yield return new WaitForSeconds(2f);
        PivotChange("None");
        curBrushPivot.transform.DOMove(brushHomePos.position, 5).SetSpeedBased();
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.Linear);
    }

    // º×ÀÇÈ£Èí Á¦ 3Çü ÇüÇü»ö»ö: È­·ÁÇÑ »öÀÇ ¹°°¨À» °è¼Ó ³¯¸°´Ù
    void BrushSkill3()
    {
        StartCoroutine(BrushSkill3Main(0));
    }
    IEnumerator BrushSkill3Main(float delay)
    {
        yield return new WaitForSeconds(delay);

        curBrushPivot.transform.DOMove(createPos_BrushSkill[1].position, 1).SetEase(Ease.Linear);
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 120), 0.6f).SetRelative().SetEase(Ease.Linear);
        yield return new WaitForSeconds(2);

        PivotChange("Handle");
        // ÆäÀÎÆ® 50¹ß ¹ß»ç
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 80), 0.5f).SetEase(Ease.Linear);
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 200), 1f).SetEase(Ease.Linear).SetDelay(1f).SetLoops(-1, LoopType.Yoyo);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 50; i++)
        {
            GameObject paint = objectPoolLogic.MakeObj("Boss_Graphic_Paint");
            paint.transform.position = brush.transform.position;
            paint.transform.up = brush.transform.up;
            paint.GetComponent<Rigidbody2D>().velocity = paint.transform.up * 12f;
            paint.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(i / 18f - i / 18, 1, 1);
            brushColorSprite.color = Color.HSVToRGB( i / 18f - i / 18, 1, 1);
            yield return new WaitForSeconds(0.1f);
        }
        curBrushPivot.transform.DOKill();
        yield return new WaitForSeconds(2f);

        PivotChange("None");
        curBrushPivot.transform.DOMove(brushHomePos.position, 5).SetSpeedBased();
        curBrushPivot.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.Linear);
    }
}
