using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public int dmg = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BulletDeadLine")
        {
            gameObject.transform.DOKill();

            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            gameObject.transform.localScale = Vector3.one;

            if (gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
            {
                Color objColor = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                objColor.a = 1;
                gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = objColor;
            }
            if (gameObject.transform.GetChild(0).GetComponent<MeshRenderer>() != null)
            {
                gameObject.transform.GetChild(0).transform.DOKill();
                gameObject.transform.GetChild(0).transform.rotation = Quaternion.Euler(Vector3.zero);

                Color objColor = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                objColor.a = 1;
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = objColor;
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            gameObject.SetActive(false);

            return;
        }

        if (gameObject.tag == "PlayerBullet")
        {
            if (collision.tag == "Enemy")
            {
                if (collision.name == "Boss_PM")
                {
                    Boss_PM bossLogic = collision.GetComponent<Boss_PM>();
                    bossLogic.Hit();
                }
                if (collision.name == "Boss_Graphic")
                {
                    Boss_Gra_LeeVar bossLogic = collision.GetComponent<Boss_Gra_LeeVar>();
                    bossLogic.Hit();
                }
                if (collision.name == "Boss_Tutorial")
                {
                    TutorialBoss bossLogic = collision.GetComponent<TutorialBoss>();
                    bossLogic.Hit();
                }

                gameObject.SetActive(false);
            }
        }
        if (gameObject.tag == "BossBullet")
        {
            if (collision.tag == "Player")
            {
                PlayerController playerLogic = collision.GetComponent<PlayerController>();
                playerLogic.PlayerHit(dmg);
            }
        }
    }
}
