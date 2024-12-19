using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    //Resource의 Load, Instantiate, Destroy를 관리하는 리소스 매니저

    //path에 있는 파일을 로드. 로드 조건은 Object일 때
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    //생성(Instantiate)
    //parent : 프리팹을 생성해서 붙일 곳
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, parent);
    }

    public void Destroy(GameObject obj)
    {
        if (obj == null) return;
        Object.Destroy(obj);
    }

    //--> 싱글톤 Managers에 ResourceManager 인스턴스를 생성하고, 다른 클래스에서 접근할 수 있도록 프로퍼티 형태로 셍성
    //--> 다른 클래스에서 접근하려면 Managers.Resource.Instantiate("경로명"); 형태로 사용
}
