using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.EventSystems;
using System;

public class ScenarioManager : MonoBehaviour
{

    public Text scenarioTitle;
    public GameObject scenarioLogBox;
    int scenarioNum = 1;
    int textLogCount = 0;
    int scaledFontSize;

    StoryData storyData;
    
    //디폴트 스크린 높이
    float uiBaseScreenHeight = 1920f;
    
    private int GetScaledFontSize(int baseFontSize)
    {
        float uiScale = Screen.height / uiBaseScreenHeight;
        int scaledFontSize = Mathf.RoundToInt(baseFontSize * uiScale);
        return scaledFontSize;
    }

    
    // Use this for initialization
    //-시나리오 선택에서 넘어왔으면 시나리오 타이틀을 보여주고 로그표시, 게임에서 넘어왔으면 로그를 바로 표시
    void Start()
    {
        setScenarioTitle();
        
        storyData = Resources.Load("Data/StoryData") as StoryData;


        //시나리오 로그 텍스트 크기 설정
        scaledFontSize = GetScaledFontSize(80);
    }

    void setScenarioTitle()
    {
        switch (scenarioNum)
        {
            case 1:
                scenarioTitle.text = "「人生の転機」";
                break;
            case 2:
                scenarioTitle.text = "「幼馴染と先輩」";
                break;
            case 3:
                scenarioTitle.text = "「これがリア充」";
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
        }

        StartCoroutine("titleAnimation");



    }

    IEnumerator titleAnimation()
    {
        iTween2.FadeTo(scenarioTitle.gameObject, iTween2.Hash("alpha", 1f, "time", 1f));
        yield return new WaitForSeconds(2.2f);
        iTween2.FadeTo(scenarioTitle.gameObject, iTween2.Hash("alpha", 0f, "time", 1f));
        yield return new WaitForSeconds(0.8f);
        //nextScene();
        logTest();
    }

    public void nextScene()
    {
        Application.LoadLevel("GameScene");
    }

    //대화상자 터치시 다음 대화 나오게하는 함수
    public void nextText()
    {
        //만약 텍스트가 표시되고있는중이면 강제종료하고
        if (m_bPlay)
        {
            StopText(true);

            textLogBox.GetComponentInChildren<Text>().text = storyData.list[textLogCount-1].log;

        }
        else
        {
            //텍스트 폰트 크기조정
            textLogBox.GetComponentInChildren<Text>().fontSize = (int)(scaledFontSize * storyData.list[textLogCount].fontSize);
            //로그표시
            InitializeTxt(storyData.list[textLogCount].log , true);
            textLogCount++;
        }

        //표시할텍스트가끝이면 다음신으로
        if (textLogCount > 104)//storyData.list.Count) 
            nextScene();
    }

    public void logTest()
    {
        StopCoroutine("titleAnimation");
        scenarioLogBox.SetActive(true);
        nextText();
    }

    //----------------단어단위로 출력하는 함수,변수
    /// <summary>
    ///  1. 문자열을 글자 1자 단위로 출력한다.
    ///  2. 문자열을 단어 단위로 출력한다.
    /// </summary>

  
        [System.Serializable]
        public enum EUNIT_TYPE
        {
            eChar,          // 글자 1자 단위
            eWord,          // 단어 단위
        }

        //public UILabel m_txtLabel = null;
        public GameObject textLogBox;
        [SerializeField]
        protected EUNIT_TYPE m_UnitType = EUNIT_TYPE.eChar;       // 출력 단위 설정..
       // [SerializeField]
        protected float m_PrintDelayTime = 0.05f;                    // 출력 단위별 지연시간

        protected StringBuilder m_Builder = null;
        protected bool m_bPlay = false;
        protected string[] m_aStrList = null;
    
       
        public void InitializeTxt(string sContents, bool bStart = false)
        {
            m_Builder = new StringBuilder(sContents);
            m_Builder.Remove(0, m_Builder.Length);

            if (m_UnitType == EUNIT_TYPE.eWord)
                MakeWordList(sContents);

            m_Builder.Append(sContents);

            if (bStart)
                Play();
        }
        //------------------------------------------------------------------
        public void Play()
        {
         //   gameObject.SetActive(true);
            m_bPlay = true;
            StartCoroutine("EnumFunc_StrAniPlay", m_PrintDelayTime);
        }
        //------------------------------------------------------------------
        IEnumerator EnumFunc_StrAniPlay(float fDelay)
        {
            int iCount = 0;
            int iEndIndex = 0;

            while (m_bPlay)
            {
                textLogBox.GetComponentInChildren<Text>().text = m_Builder.ToString(0, iEndIndex);
                yield return new WaitForSeconds(fDelay);

                if (iEndIndex >= m_Builder.Length)
                {
                    iEndIndex = m_Builder.Length;
                    StopText(); break;
                }

                // 단어 단위 계산..
                if (m_UnitType == EUNIT_TYPE.eWord)
                {
                    iEndIndex = GetLengthForWordNum(iCount);
                    ++iCount;
                }
                // 글자 1개 단위    
                else if (m_UnitType == EUNIT_TYPE.eChar)
                {
                    ++iEndIndex;
                }
            }
        }
        //------------------------------------------------------------------
        // 단어갯수만큼의 문자열 길이
        protected int GetLengthForWordNum(int nWordNum)
        {
            if (nWordNum == 0) return 0;

            if (nWordNum > m_aStrList.Length)
                nWordNum = m_aStrList.Length;

            int nTot = m_aStrList[0].Length;

            for (int i = 1; i < nWordNum; i++)
            {
                nTot += (m_aStrList[i].Length + 1);   // + 1 : 공백을 더한다.
            }
            return nTot;
        }
        //------------------------------------------------------------------
        public void StopText(bool bForce = false)
        {
            m_bPlay = false;

            if (bForce)
            {
                StopCoroutine("EnumFunc_StrAniPlay");
            }
        }

        //------------------------------------------------------------------
        public void MakeWordList(string strData)
        {
            m_aStrList = strData.Split(' ');
        }
        //------------------------------------------------------------------
    
}
