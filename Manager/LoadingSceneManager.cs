using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;//다음 씬 이름
    public Image Progress;//로딩화면 이미지

    void Start()
    {
        StartCoroutine(LoadSceneCoroutione());
    }

    public static void LoadScene(string SceneName)//LoadScene을 정적으로 호출하여 다른 스크립트에서 쉽게 호출 가능
    {
        nextScene = SceneName;
        SceneManager.LoadScene("LoadingScene");//로딩씬 호출
    }
    
    IEnumerator LoadSceneCoroutione()//다음 씬을 비동기 방식으로 로드하는 코루틴
    {
        yield return null;//프레임이 끝날 때 까지 대기

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);//다음 씬을 비동기 방식으로 로드 시작

        op.allowSceneActivation = false;//씬의 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인가를 묻는 옵션. 
                                        //false로 설정하여 로딩 완료 시 다음 씬으로 전환되지 않고 대기 -> true가 될 때 마무리 로딩 후 씬 전환

        float timer = 0.0f;
        while(!op.isDone)//로딩이 완료될 때 까지 반복한다.
        {
            yield return null;//한 프레임 대기
            timer+=Time.deltaTime;//타이머 시간 증가

            if(op.progress < 0.9f)//비동기객체의 진행도가 0.9 이하(즉, 로딩 진행중)일때
            {
                //로딩 이미지의 fillAmount를 현재 로딩 진행도에 맞춰 부드럽게 증가시킴.
                Progress.fillAmount = Mathf.Lerp(Progress.fillAmount, op.progress, timer);
                
                if(Progress.fillAmount >=op.progress)//로딩 이미지의 fillAmount가 로딩 진행도보다 크거나 같아지면(즉, 칸이 다 찼는데도 로딩중이면)
                {
                    timer = 0f;//타이머는 다시 0으로.
                }
            }
            else//로딩 진행도가 0.9 이상일 때(거의 완료된 상태)
            {
                Progress.fillAmount = Mathf.Lerp(Progress.fillAmount, 1f, timer);//로딩 이미지의 fillAmount를 1(100%)로 부드럽게 증가시킴
                
                if(Progress.fillAmount ==1.0f)//로딩 이미지의 fillAmount가 1이 되면 다음 씬으로 전환
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    
    }

}
