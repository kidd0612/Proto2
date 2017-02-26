using UnityEngine;
using System.Collections;

public class EffectFade : MonoBehaviour {

    //G2_4게임 Dummy오브젝트 페이드 하기위한 클래스 + G1_5게임 hit이미지 fade
    public float preWaitTime;
    public float fadeTime;
    public float fadeWait;

    // 활성화될때 한번 불러옴
    void OnEnable()
    {
        StartCoroutine("Fade");

    }

    void Update()
    {

    }

    IEnumerator Fade()
    {
        iTween2.FadeTo(this.gameObject, iTween2.Hash("alpha", 1f, "time", 0f));
        //더미 이미지 이동시간과 같은시간만큼 일단 기다린다음 사라짐
        yield return new WaitForSeconds(preWaitTime);
        iTween2.FadeTo(this.gameObject, iTween2.Hash("alpha", 0f, "time", fadeTime));
        //바로 비활성화가 되므로 딜레이를 주고 비활성화 
        yield return new WaitForSeconds(fadeWait);
        this.transform.gameObject.SetActive(false);
    }
}
