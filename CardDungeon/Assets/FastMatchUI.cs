using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FastMatchUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> panels;
    [SerializeField]
    private float activeScale = 1.0f; // 활성화 시 크기
    [SerializeField]
    private float inactiveScale = 0.4f; // 비활성화 시 크기
    [SerializeField]
    private float duration = 0.2f;

    public void DoPanelAnim(bool isOn)
    {
        StartCoroutine(PanelAnimationCor(isOn));
    }

    public void TryFastMatch(int headCount)
    {
        BackendManager.Instance.RequestFastMatchMaking(headCount);
    }

    public void CloseBtnAction()
    {
        StartCoroutine(PanelAnimationCor(false));
    }
    
    IEnumerator PanelAnimationCor(bool isOn)
    {
        if (isOn)
        {
            gameObject.SetActive(true);
            
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].transform.DOScale(activeScale, duration);

                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("비활성화 실행");
            
            for (int i = panels.Count - 1; i > 0; i--)
            {
                panels[i].transform.DOScale(inactiveScale, duration);
                
                yield return new WaitForSeconds(0.1f);
            }
            
            UIManager.Instance.PopupListPop();
        }
    }
}
