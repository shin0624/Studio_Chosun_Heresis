using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배전함의 오픈 및 레버 작동 여부를 관리하는 스크립트
public class OpenShieldController : MonoBehaviour
{
    public static OpenShieldController SHInstance { get; private set; }//레버 유무에 따라 오토셔터가 컨트롤되므로 레버 조작 유무도 전역으로 선언

    public GameObject cup; // 덮개 오브젝트
    private bool isOpen = false;//덮개가 열렸는지 여부
    private GameObject player;

    public bool leverRaised = false;//손잡이가 올라갔는지 여부
    public GameObject Knob;//손잡이 오브젝트

    [SerializeField]
    public float leverRaiseDigit;

    private void Awake()
    {
        if(SHInstance==null)
        {
            SHInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        player = GameObject.FindWithTag("PLAYER"); // 태그를 사용하여 플레이어 객체를 찾는다.
        if (player == null)
        {
            Debug.LogError("Player object not found. Make sure the player object has the 'Player' tag.");
        }
        if(isOpen)
        {
            OpenCupDirectly();//덮개가 열린 상태를 유지
        }
        if(leverRaised)
        { 
            SetLeverUp();//씬 전환 후에도 레버가 올라간 상태를 유지
        }
    }

    private void Update()
    {
        if(player==null)
        {
            player = GameObject.FindWithTag("PLAYER");
        }
        if (player != null && !isOpen)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < 3.0f && Input.GetKeyDown(KeyCode.E)) // 플레이어가 배전함 가까이 있고 E 키를 눌렀다면
            {
                OpenCup();
            }
        }
        else if(isOpen && !leverRaised)//배전함이 열려있고 e키를 눌렀다면
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                LeverUp();
                
            }
            
        }
    }

    private void OpenCup()
    {
        if (!isOpen)
        {
            Vector3 newRotation = cup.transform.eulerAngles;
            newRotation.x = cup.transform.eulerAngles.x;
            newRotation.y =  newRotation.y + 150.0f;
            newRotation.z = cup.transform.eulerAngles.z;

            cup.transform.eulerAngles = newRotation;
            isOpen = true;
            Debug.Log("Shield cup opened!");
        }
    }

    private void LeverUp()
    {             
        if(isOpen)
        {
            leverRaised = true;//레버가 올라간 상태로 변경
            
            SetLeverUp();
            Debug.Log("lever up!");
        }
        
    }

    public void ResetLever()
    {
        leverRaised = false;
        
    }

    private void OpenCupDirectly()
    {
        Vector3 newRotation = cup.transform.eulerAngles;
        newRotation.x = cup.transform.eulerAngles.x;
        newRotation.y = newRotation.y + 150.0f;
        newRotation.z = cup.transform.eulerAngles.z;

        cup.transform.eulerAngles = newRotation;
        isOpen= true;
        Debug.Log("Shield cup state RESTORED!");
    }

    private void SetLeverUp()
    {
        Vector3 newRotation_K = Knob.transform.eulerAngles;
        newRotation_K.x = newRotation_K.x + 111.0f;
        newRotation_K.y = Knob.transform.eulerAngles.y;
        newRotation_K.z = Knob.transform.eulerAngles.z;
        Knob.transform.eulerAngles = newRotation_K;
    }
}
