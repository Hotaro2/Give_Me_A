using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    float span = 1.0f;
    float delta = 0; void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span) //delat가 1초 이상이 되면
        {
            this.delta = 0;
            GameObject go = Instantiate(arrowPrefab) as GameObject;

            //화살의 x좌표를 -6부터 6 사이에 불규칙하게 위치하도록 랜덤으로 반환
            int px = Random.Range(-6, 7);
            go.transform.position = new Vector3(px, 7, 0);
        }
    }
}