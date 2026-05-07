using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MySceneManager : MonoBehaviour
{
    /*
     *  자기 자신을 싱글톤화 하여 다른 스크립트에서도 자유롭게 호출
     */
    public static MySceneManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static MySceneManager instance;

    private void Start()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    /*
     *  씬 체인지
     *  다른씬으로 바꾸기 전에 화면 어둡게
     */

    public CanvasGroup fadeImage;
    float fadeDuration = 2f;

    public void ChangeScene(string sceneName)
    {
        fadeImage.DOFade(1, fadeDuration)
            .OnStart(() =>
            {
                fadeImage.blocksRaycasts = true;
            })
            .OnComplete(() =>
            {
                StartCoroutine("LoadScene", sceneName);
            });
    }


    public GameObject loading;
    public TextMeshProUGUI loadingText;

    IEnumerator LoadScene(string sceneName)
    {
        loading.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //퍼센트 딜레이용

        float pastTime = 0f;
        float percentage = 0f;

        while (!(async.isDone))
        {
            yield return null;

            pastTime += Time.deltaTime;

            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, pastTime);

                if (percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, pastTime);
                if (percentage >= 90) pastTime = 0;
            }
            loadingText.text = percentage.ToString("0") + "%"; //로딩 퍼센트 표기
        }

    }

    /*
     *  씬 로드
     */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImage.DOFade(0, fadeDuration)
        .OnStart(() => {
            loading.SetActive(false);
        })
        .OnComplete(() => {
            fadeImage.blocksRaycasts = false;
        });
    }




    public Text ending_Demo;

    public void Ending(string end)
    {
        switch (end)
        {
            case "Demo":
                fadeImage.DOFade(1, fadeDuration);
                ending_Demo.DOFade(1, 1f).SetDelay(fadeDuration);
                ending_Demo.DOFade(0, 1f).SetDelay(fadeDuration + 4f).OnComplete(() =>
                {
                    StartCoroutine("LoadScene", "Menu");
                });
                break;
            default:
                break;
        }
    }
}
