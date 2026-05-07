using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public string triggerName;
    public bool inDoor = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (inDoor == true)
            {
                switch (triggerName)
                {
                    case "Door":
                        TutorialManager tutorialLogic = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
                        tutorialLogic.PassOk();
                        break;
                    case "Boss1":
                        MySceneManager.Instance.ChangeScene("Boss_PM");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (triggerName == "TutorialPortal1")
            {
                TutorialManager tutorialLogic = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
                tutorialLogic.StartCoroutine(tutorialLogic.BlackOut(1));
            }
            if (triggerName == "Door")
            {
                inDoor = true;
            }
            if (gameObject.tag == "Door")
                inDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (triggerName == "Door")
            {
                inDoor = false;
            }
            if (gameObject.tag == "Door")
                inDoor = false;
        }
    }
}
