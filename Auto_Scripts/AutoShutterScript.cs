using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Compilation;
using UnityEngine;

//보안실에서 레버를 올리면 2층으로 가는 계단을 막는 셔터가 올라간다. 이를 제어하기 위한 스크립트.

public class AutoShutterScript : MonoBehaviour
{ 
    public static AutoShutterScript Instance { get; private set; }//오토셔터 인스턴스를 전역으로 선언-> 씬 전환 후에도 셔터의 RaiseShutter값이 변하지 않아야 하기 때문
    public bool RaiseShutter;//셔터가 올라갔는지 유무
    
    private OpenShieldController _sh;//보안실 배전함 스크립트에서 레버가 올라갔는지 유무를 확인하여 참조
  
    private Vector3 RaisPosition;//셔터 올라가는 위치벡터
    public float RaiseLerpTime = 0.1f;//보간시간
    public AudioSource Ado;//셔터 효과음


    public void Awake()//초기화
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       // RaiseShutter = false;
        RaisPosition = transform.position + new Vector3(0, 2.9f, 0);//y축으로 2.96 +되어야 딱 맞음

        _sh = GameObject.FindFirstObjectByType<OpenShieldController>();

        if(_sh==null)
        {
            Debug.Log("OpenShieldController object not found");
        }

        Ado = GetComponent<AudioSource>();//오디오소스 컴포넌트를 찾는다. 본 스크립트가 부착된 오브젝트에서 찾으므로 GameObject는 선언 x
        if (Ado == null)
        {
            Debug.LogError("AudioSource component not assigned!");
            
        }
    }

    
    void Update()
    {
        if (_sh ==null)
        {
            _sh = FindOpenShieldController();
        }

        if(_sh != null && _sh.leverRaised)
        {
            Raise();
        }
        if(RaiseShutter && Vector3.Distance(transform.position, RaisPosition) < 0.01f)
        {
            _sh.ResetLever(); //셔터가 목표 위치까지 올라왔다면 leverRaised값을 false로 변경. 이렇게 해야 사운드 반복재생이 안된다.
        }

    }

    void Raise()
    {
            Debug.Log(" 1F Shutter Raised! ");
            transform.position = Vector3.MoveTowards(transform.position, RaisPosition, Time.deltaTime * RaiseLerpTime);//셔터의 포지션이 변경됨. 레프함수로 보간하여 자연스러운 모션
            RaiseShutter = true;
             Debug.Log($" Position : {transform.position}");
                if (Ado != null && !Ado.isPlaying)
                {
                    Ado.Play();
                    Ado.loop = false;
                }
    }

    private OpenShieldController FindOpenShieldController()
    {
        return GameObject.FindFirstObjectByType<OpenShieldController>();
    }
}
