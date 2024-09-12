using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorLockController : MonoBehaviour
{
    [Header("Right Password")]
    public int RightPassword; // 정답이 될 패스워드 설정

    [Header("Input Password")]
    public TMP_InputField InputField; // 텍스트메쉬프로의 인풋필드 객체

    [Header("Buttons")]
    public List<Button> NumberButtons;//숫자 버튼들
    public Button EnterButton; // 엔터 버튼

    [Header("Color T/F")]
    public Color TrueColor; // 정답이 될 때의 컬러
    public Color FalseColor; // 정답이 아닐 때의 컬러
    public Color BasicColor; // 기본 컬러

    [Header("Sounds")]
    public AudioSource ButtonSound;//버튼 클릭음
    public AudioSource CorrectSound;//정답음
    public AudioSource ErrorSound;//에러음

    private string InputPassword = "";//비밀번호 초기화 값
    public float characterSpacing; // 원하는 자간 값
    public bool OpenDoorFlag;//비밀번호를 정확히 입력하여 문이 열렸는지 여부

    public static event System.Action OnDoorLockClosed;//도어락 ui의 정적이벤트를 정의->ContactController에서 구독하여 ui의 활성/비활성을 처리
    //System.Action은 반환값과 매개변수가 없는 메서드

    void Start()
    {
        OpenDoorFlag = false;
       
        SetInputFieldColor(BasicColor);// 인풋필드의 초기 색깔 설정
        InputField.textComponent.characterSpacing = characterSpacing; // 초기 자간 설정

        foreach (var button in NumberButtons) // 버튼 객체 아래에 있는 TMP 텍스트를 받아와, 정수형으로 강제 형변환.
        {
            int number = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text);
            
            button.onClick.AddListener(() => AppendNumber(number)); // 버튼마다 리스너를 등록. 람다식으로 간결히 작성
        }
        EnterButton.onClick.AddListener(EnterButtonClicked); // 엔터버튼 리스너
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))//ui가 활성화 될 때에만 esc키 입력을 체크함.
        {
            OnDoorLockClosed?.Invoke();//esc키가 눌리면 이벤트 호출->이벤트의 구독자가 있으면 Invoke메서드 호출
            //NULL 조건부 연산자를 사용하여 널 참조오류 방지
        }
    }

    void AppendNumber(int number)
    {
        if (InputPassword.Length < 4)
        {
            ButtonSound.Play();
            InputPassword += number.ToString(); // 정수타입의 number를 string으로 형변환
            
            InputField.text = InputPassword; // 인풋필드 객체에 작성되는 text를 string변환된 패스워드로 교체
            
        }
    }

    void EnterButtonClicked()
    {
        if (EnterButton != null)
        {
            int inputPasswordValue;
            bool isNumeric = int.TryParse(InputPassword, out inputPasswordValue); // string값의 숫자체크 후 정수값으로 리턴
            if (isNumeric && inputPasswordValue == RightPassword)
            {
                OpenDoorFlag = true;
                Debug.Log($"OpenDoorFlag = {OpenDoorFlag}");
                CorrectSound.Play();
                SetInputFieldColor(TrueColor);//정답 컬러(녹색)으로 변경
                InputField.textComponent.characterSpacing = 23.0f;//문자 출력이므로 자간 조정
                InputField.text = "OPEN DOOR";//정답 메세지 출력
                
                StartCoroutine(OpenDoorAfterDelay(0.7f));//0.5초 후 UI 닫힘
            }
            else
            {
                OpenDoorFlag = false;
                ErrorSound.Play();
                SetInputFieldColor(FalseColor);//오류 컬러(빨간색)으로 변경
                InputField.textComponent.characterSpacing = 23.0f;//문자 출력이므로 자간 조정
                InputField.text = "ERROR";//에러 메세지 출력
                StartCoroutine(ResetColorAfterDelay(0.7f)); // 색상을 원래대로 되돌리기 위한 코루틴 호출
            }
        }
    }

    void SetInputFieldColor(Color color)//디스플레이 컬러 변경을 위한 메서드
    {
        var colors = InputField.colors;
        colors.normalColor = color;
        InputField.colors = colors;
    }

    IEnumerator ResetColorAfterDelay(float delay)// 비밀번호 오류 시 ERROR 출력 + 빨간색 디스플레이 출력 후 다시 초기화
    {
        yield return new WaitForSeconds(delay);//지정된 시간만큼 대기 후 컬러 초기화
        SetInputFieldColor(BasicColor); // 이미지 컬러 초기화
        InputField.textComponent.characterSpacing = characterSpacing;
        InputPassword = ""; // 비밀번호 초기화
        InputField.text = "0000"; // 텍스트 초기화
    }

    IEnumerator OpenDoorAfterDelay(float delay)//비밀번호 정답 시 OPEN DOOR 출력 + 녹색 디스플레이 출력 후 UI 닫음
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        OnDoorLockClosed?.Invoke();
    }
}
