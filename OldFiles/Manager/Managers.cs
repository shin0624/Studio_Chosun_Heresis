using UnityEditor;
using UnityEngine;

public class Managers : MonoBehaviour
{
    //게임 매니저 클래스 정의. 씬이 변경되어도 계속 유지되어야 할 데이터 및 최상위에서 제어되어야 할 데이터를 관리

    static Managers instance;// 싱글톤 패턴을 사용하기 위한 전역 인스턴스 선언
    public static Managers GetInstance() { if (instance == null) { Init(); } return instance; }//다른 클래스에서 Managers에 접근할 때 사용하는 프로퍼티

    private ResourceManager resourceManager = new ResourceManager();//리소스매니저 인스턴스 선언
    public static ResourceManager Resource { get { return instance.resourceManager; } }

    public SceneChangeManager sceneChangeManager = new SceneChangeManager();//씬체인지 매니저 인스턴스 선언
    public static SceneChangeManager SceneChange { get { return instance.sceneChangeManager; } }


    [SerializeField]
    private int PlayerMentalPower;//플레이어 정신력
    [SerializeField]
    private float PlayerFlashIntensity;//손전등 밝기

    private void Awake()// Awake는 Unity에서 객체가 생성될 때 가장 먼저 호출되므로, Mangers객체가 중복 생성되지 않게 하기 위함
    {
        //만약 awake가 호출되었을 때 다른 Managers인스턴스가 존재하면 새로 생성된 객체를 제거
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Init();//다른 Managers 인스턴스가 존재하지 않으면 초기화 진행
    }

    static void Init()//싱글톤 초기화가 클래스 레벨에서 진행되기 때문에, 클래스 내부의 다른 메서드나 외부에서 호출하기 편리하도록 static으로 선언
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("Managers");
            if (go == null)
            {
                go = new GameObject { name = "Managers" };
                instance = go.AddComponent<Managers>();//go 생성 후 바로 Managers컴포넌트 추가
            }
            else
            {
                instance = go.GetComponent<Managers>();
                if (instance == null)
                {
                    instance = go.AddComponent<Managers>();
                }
            }
            DontDestroyOnLoad(go);

        }
    }
    //다른 클래스에서 Managers를 사용할 때 : Managers mg = Managers.GetInstance();형태로 사용
#if WhatIsManagers
    //게임 전반을 관리하기 때문에 Static으로 선언하여 여러 곳에서 동일한 인스턴스를 참조(유일성 보장)
    //매니저 컴포넌트를 리턴받을 수 있는 함수 GetInstance가 필요.
#endif
#if CaseOfInit
    //Init()가 호출되는 경우
    // 1. Managers가 생성되었을 때
    // 2. 다른 스크립트에서 GetInstance() 호출 시 널체크
#endif


}
