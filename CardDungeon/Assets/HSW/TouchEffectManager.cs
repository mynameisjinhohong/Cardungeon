using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TouchEffectManager : Singleton<TouchEffectManager>
{
    private static TouchEffectManager instance;   // 인스턴스
    
    public GameObject startPrefab;
    
    private float spawnsTime;

    public float defaultTime = 0.05f;
    
    private bool isPlaying = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPointerOverButton())
        {
            AudioPlayer.Instance.PlayClip(1);
            //SoundManager.instance.SFXPlay(ResourceManager.Instance.GetAudioClip("TOUCH_Sound"));
        }
        if (Input.GetMouseButton(0) && spawnsTime >= defaultTime)
        {
            StarCreat();
            spawnsTime = 0;
        }
        spawnsTime += Time.deltaTime;
    }
    
    void StarCreat()
    {
        if (Camera.main == null) return;

        Vector3 mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPosition.z = 0.5f;
        Instantiate(startPrefab, mPosition, quaternion.identity);
    }
    
    bool IsPointerOverButton()
    {
        // 마우스 포인터의 위치에서 UI 오브젝트를 찾습니다.
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 버튼 컴포넌트가 있는 UI 오브젝트가 있으면 true를 반환합니다.
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }

}