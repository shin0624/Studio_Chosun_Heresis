using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightContoller : MonoBehaviour
{
    bool GetLight;
    Light LightComponent;
  
    void Start()
    {
        GetLight = false;
        LightComponent = this.GetComponent<Light>();//손전등(flashlight) 오브젝트가 가진 Light컴포넌트를 불러온다.
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            GetLight = GetLight ? false : true;
            //F키로 손전등을 온오프한다. 처음 시작 시 손전등은 false상태이므로 True를 반환하며 손전등이 켜질 것. false일 때 true를 반환해야 F키 하나로 on/off조절이 가능하다.
            //(true일 때 false를 반환해야 손전등이 꺼질 것이므로) 
        }
        if(GetLight==false)//손전등 꺼짐
        {
            LightComponent.intensity = 0;
        }
        if(GetLight ==true)//손전등 켜짐
        {
            LightComponent.intensity = 3;
        }
    }
}
