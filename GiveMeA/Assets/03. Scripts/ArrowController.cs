using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("player");

    }

    // Update is called once per frame
    void Update()
    {
        //프레임마다 등속으로 낙하시킨다.
        transform.Translate(0, -0.01f, 0);

        //화면 밖으로 나오면 오브젝트를 소멸시킨다.
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

        //충돌 판정
        Vector2 p1 = transform.position; //화살 중심 좌표
        Vector2 p2 = this.player.transform.position; //플레이어 중심 좌표
        Vector2 dir = p1 - p2;

        float d = dir.magnitude; //dir의 길이
        float r1 = 0.5f; //화살 반경
        float r2 = 1.0f; //플레이어 반경

        //충돌한 경우 화살 지우기
        if (d < r1 + r2)
        {
            Destroy(gameObject);
        }
    }
}
