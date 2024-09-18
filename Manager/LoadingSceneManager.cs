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

    [SerializeField]
    private Image Progress;//로딩 바 이미지
    [SerializeField]
    private List<Sprite> ProgressImages = new List<Sprite>();//스프라이트 이미지를 랜덤으로 보여주기 위해, 스프라이트 객체를 담을 리스트를 선언. 리스트는 동적크기할당이 가능한 자료형이라 작은 수의 이미지는 굳이 크기지정을 안해줘도 됨. 아주 많은 이미지를 넣는다면 지정해주면 좋음.
    [SerializeField]
    private Image LoadingPanelImage;//패널에 보여줄 이미지 변수

    void Start()
    {
        SetRandomLoadingImage();
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
        while(!op.isDone)
        {
            yield return null;// 한 프레임 대기
            //로딩 진행도에 맞춰서 fillAmount를 적용.
            float ProgressValue = Mathf.Clamp01(op.progress / 0.9f);//Clamp01을 사용해서 로딩 진행도를 0.0 ~ 1.0으로 맞춘다. Clamp01은 퍼센트값을 다룰 때 유용.
            Progress.fillAmount = ProgressValue;

            if(op.progress >= 0.9f)//로딩 완료 시
            {
                Progress.fillAmount = 1.0f;//로딩 완료 시 100%로 맞춘다.
                op.allowSceneActivation = true;//씬 전환
                yield break;
            }       
        }
    }

    private void SetRandomLoadingImage()//랜덤 이미지를 설정하는 메서드
    {
        if(ProgressImages.Count >0)//리스트에 이미지가 있으면
        {
            int RandomIndex = Random.Range(0, ProgressImages.Count);//랜덤 인덱스 생성
            LoadingPanelImage.sprite = ProgressImages[RandomIndex];//패널 이미지 변경
        }
    }

}
