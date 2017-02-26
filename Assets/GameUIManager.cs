using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : SingleTon<GameUIManager> {

    //UI변수
    public Image lifeBar;
    public GameObject gameOverUI;
    public Text scoreTxt;
    public GameObject pauseUI;
    public Text countDownTxt;

   // float life;
    //float preLife;
  //  public int minusLife = 0;

    int _timeLimit;
    public Text timeLimitText;


    //이펙트관련
    public GameObject[] heartDummies;
    int currentDummy;
    //초기화는 msgMovemanager쪽에서해줌
    public GameObject[] gfObjs;
    public Sprite[] gfImages;
    public GameObject cutInObj;


    public Sprite returnImg;
    public Sprite kabuseta;

    
    
    /*
    public void StartCoroutineLife()
    {
        StartCoroutine("lifeGaugeMinus()");

    }

    public void StopCoroutineLife()
    {
        StopCoroutine("lifeGaugeMinus()");
    }
    */

    //초기화함수
    public void InitUI()
    {
        //타임리밋 초기화
        _timeLimit = GameManager.instance.timeLimit;

        //변수 초기화
        //life = 1000f;
     //   minusLife = 0;
        lifeBar.fillAmount = 1f;
        currentDummy = 0;
        //UI초기화 함수
        setScoreUI();

        //이펙트 초기화

        //게임 타이머 스타트
        StopCoroutine("gameTimer");
        StartCoroutine("gameTimer");
    }

    IEnumerator gameTimer()
    {
        do
        {
            timeLimitText.text = _timeLimit.ToString();
            yield return new WaitForSeconds(1f);
            timeLimitPlusMinus(-1);
        }
        while (_timeLimit > 0);
    }
    //데미지라인에 가면 타임 -1초
    public void timeLimitPlusMinus(int _time)
    {
        _timeLimit += _time;
        timeLimitText.text = _timeLimit.ToString();

        //0이하가 되면 게임오버
        if (_timeLimit <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
    /*
    public IEnumerator lifeGaugeMinus()
    {
        Debug.Log("UIcalled");
        preLife = life;
        minusLife = 1;

        life = life - (minusLife * 10);
        lifeBar.fillAmount = life / 100f;
        minusLife = 0;
        if (life <= 0)
        {
            GameManager.instance.GameOver();
        }

        yield return null;
    }
    */
    //이펙트 초기화 함수인데 0.5초만에 완료되다보니 아마 없어도 될듯?
    void InitEffect()
    {
        for(int i = 0; i < heartDummies.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                //heartDummies[i].transform.GetChild(j).gameObject
            }
        }
    }

    //게임오버 유아이 
    public void activeGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    public void setScoreUI()
    {
        scoreTxt.text = GameManager.instance.nokoriMsg.ToString() + "個";
    }


    #region effect관련
    //이펙트 위치 셋팅후 -> 코루틴 애니메이션 실행
    public void startEffect(int _msgBoxIdx)
    {
        //하트의 랜덤위치 지정을 위한 임시 변수
        int x, y;
        float _msgWidth, _msgHeight;
        //임시 변수 세팅
        _msgWidth = MsgMoveManager.instance.msgWidth;
        _msgHeight = MsgMoveManager.instance.msgHeight;

        //임시 오브젝트 변수
        GameObject _msgBox = MsgMoveManager.instance.msgBox[_msgBoxIdx];
 
        //하트 이펙트 자리 세팅
        for (int i = 0; i < 3; i++)
        {
            x = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
            y = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;

            //붙어있는 itween전부 제거
            iTween2.Stop(heartDummies[currentDummy].transform.GetChild(i).gameObject);

            //각 하트의 자리 세팅
            heartDummies[currentDummy].transform.GetChild(i).position = new Vector3(_msgBox.transform.position.x + (x * UnityEngine.Random.Range(_msgWidth / 6, _msgWidth / 2)), _msgBox.transform.position.y + (y * UnityEngine.Random.Range(_msgHeight / 6, _msgHeight / 2)), _msgBox.transform.position.z);

            // iTween2.FadeTo(effect.transform.GetChild(i).transform.gameObject, iTween2.Hash("alpha", 1f, "delay", 0f, "time", 0f));

            //펀치스케일이 일찍 꺼졌을때를 대비해서 원래 크기 맞춰주기
            heartDummies[currentDummy].transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);
            
        }

        //겹치는 이미지로 바꾸고
        _msgBox.GetComponent<Image>().sprite = kabuseta;
        setGfEffect(_msgBoxIdx);
        //실제 이펙트 코루틴
        StartCoroutine(effectAnim(_msgBox, currentDummy));
        //코루틴 실행시키고 바로 올리기 -> 코루틴이 안끝나도 다음더미가 바로 실행될수있게
        dummyCount();
    }    

    //이펙트 애니메이션
    IEnumerator effectAnim(GameObject _tmpMsg, int _dummyNum)
    {
        //겹치는 이미지->송신 이미지로 바꾸기위한 0.1초
        yield return new WaitForSeconds(0.1f);

        //메시지박스 이미지를 송신이미지로 바꾸고
        _tmpMsg.GetComponent<Image>().sprite = returnImg;
        //송신이 사라지면서
        iTween2.FadeTo(_tmpMsg, iTween2.Hash("alpha", 0f, "delay", .1f, "time", .3f));

        //
        for(int i = 0; i < 3; i++)
        {
            iTween2.FadeTo(heartDummies[_dummyNum].transform.GetChild(i).gameObject, iTween2.Hash("alpha",1f,"time",0f));
         
        }
        //페이드인 후에 살짝 시간을 줘야 오작동이 안생김...
        yield return new WaitForSeconds(0.01f);

        //하트 펀치스케일
        iTween2.PunchScale(heartDummies[_dummyNum].transform.GetChild(0).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.3f, "name", "punchScale"));
        iTween2.PunchScale(heartDummies[_dummyNum].transform.GetChild(1).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.3f, "name", "punchScale"));
        iTween2.PunchScale(heartDummies[_dummyNum].transform.GetChild(2).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.3f, "name", "punchScale"));
        
        //스코어로 이동
        iTween2.MoveTo(heartDummies[_dummyNum].transform.GetChild(0).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f, "time", .3f, "easetype", iTween2.EaseType.linear));
        iTween2.MoveTo(heartDummies[_dummyNum].transform.GetChild(1).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f, "time", .3f, "easetype", iTween2.EaseType.easeInQuad));
        iTween2.MoveTo(heartDummies[_dummyNum].transform.GetChild(2).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f, "time", .3f, "easetype", iTween2.EaseType.easeOutQuad));

        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < 3; i++)
        {
            //더미이미지 페이드 아웃
            iTween2.FadeTo(heartDummies[_dummyNum].transform.GetChild(i).gameObject, iTween2.Hash("alpha", 0f, "time", 0f));

        }
    }

    //더미 풀
    void dummyCount()
    {
        //더미카운트를 하나올림
        currentDummy++;
        //더미이미지를 끝까지 사용했으면 다시 처음부터 사용
        if (currentDummy > heartDummies.Length - 1)
        {
            currentDummy = 0;
        }
    }

    //gf이펙트를 종류별로 세팅해서 스타트
    void setGfEffect(int _msgIdxGf)
    {
        if(_msgIdxGf >= 0 && _msgIdxGf < 7)
        {
          //  setGfEffectPos(0);
            StopCoroutine("gf0EffectAnim");
            StartCoroutine("gf0EffectAnim");
        }
        else if(_msgIdxGf >= 7 && _msgIdxGf < 14)
        {
          //  setGfEffectPos(1);
            StopCoroutine("gf1EffectAnim");
            StartCoroutine("gf1EffectAnim");
        }
        else if(_msgIdxGf >= 14 && _msgIdxGf < 21)
        {
         //  setGfEffectPos(2);
            StopCoroutine("gf2EffectAnim");
            StartCoroutine("gf2EffectAnim");
        }
        else
        {
          //  setGfEffectPos(3);
            StopCoroutine("gf3EffectAnim");
            StartCoroutine("gf3EffectAnim");
        }
    }
    /*
    void setGfEffectPos(int _gfNum)
    {
        //하트의 랜덤위치 지정을 위한 임시 변수
        int x, y;
        float _msgWidth, _msgHeight;
        //임시 변수 세팅
        _msgWidth = MsgMoveManager.instance.msgWidth;
        _msgHeight = MsgMoveManager.instance.msgHeight;

        //임시 오브젝트 변수
        GameObject _gfImg = gfImges[_gfNum].transform.gameObject;


        //하트 이펙트 자리 세팅
        for (int i = 0; i < 3; i++)
        {
            x = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
          //  y = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;

            //붙어있는 itween전부 제거
            iTween2.Stop(_gfImg.transform.GetChild(i).gameObject);

            //성공시 상대방 이미지에 하트 표시하기위한 이미지 위치 세팅 -> 이미지 활성화
             _gfImg.transform.GetChild(i).position = new Vector3(_gfImg.transform.position.x + (x * UnityEngine.Random.Range(_msgWidth / 6, _msgWidth / 2)), _gfImg.transform.position.y + (1 * UnityEngine.Random.Range(_msgHeight / 6, _msgHeight / 2)), _gfImg.transform.position.z);

        }
    }
    */

    IEnumerator gf0EffectAnim()
    {
        Sprite tmp = gfObjs[0].GetComponent<Image>().sprite;

        gfObjs[0].GetComponent<Image>().sprite = gfImages[0];

        for (int i = 0; i < 3; i++)
        {
            gfObjs[0].transform.GetChild(i).gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(0.5f);

        gfObjs[0].GetComponent<Image>().sprite = tmp;

        for (int i = 0; i < 3; i++)
        {
            gfObjs[0].transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    IEnumerator gf1EffectAnim()
    {
        Sprite tmp = gfObjs[1].GetComponent<Image>().sprite;

        gfObjs[1].GetComponent<Image>().sprite = gfImages[1];

        for (int i = 0; i < 3; i++)
        {
            gfObjs[1].transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        gfObjs[1].GetComponent<Image>().sprite = tmp;

        for (int i = 0; i < 3; i++)
        {
            gfObjs[1].transform.GetChild(i).gameObject.SetActive(false);
        }

    }


    IEnumerator gf2EffectAnim()
    {
        Sprite tmp = gfObjs[2].GetComponent<Image>().sprite;

        gfObjs[2].GetComponent<Image>().sprite = gfImages[2];
        for (int i = 0; i < 3; i++)
        {
            gfObjs[2].transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        gfObjs[2].GetComponent<Image>().sprite = tmp;
        for (int i = 0; i < 3; i++)
        {
            gfObjs[2].transform.GetChild(i).gameObject.SetActive(false);
        }

    }


    IEnumerator gf3EffectAnim()
    {
        Sprite tmp = gfObjs[3].GetComponent<Image>().sprite;

        gfObjs[3].GetComponent<Image>().sprite = gfImages[3];
        for (int i = 0; i < 3; i++)
        {
            gfObjs[3].transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        gfObjs[3].GetComponent<Image>().sprite = tmp;
        for (int i = 0; i < 3; i++)
        {
            gfObjs[3].transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    public void rageModEffect(int _rageNum)
    {
        
        GameObject rageOb = gfObjs[_rageNum].transform.GetChild(3).gameObject;
        
        rageOb.SetActive(true);
        iTween2.ScaleTo(rageOb, iTween2.Hash("x", 1.2f, "y", 1.2f, "time", 0.1f, "looptype", "pingPong"));
    }

    public void rageModEffectEnd(int _rageNum)
    {
        GameObject rageOb = gfObjs[_rageNum].transform.GetChild(3).gameObject;

        //iTween2.ScaleTo(rageOb, iTween2.Hash("scale", new Vector3(1f,1f,1f),"time", 0f));
        iTween2.Stop(rageOb);
        rageOb.transform.localScale = new Vector3(1f, 1f, 1f);
        
        rageOb.SetActive(false);
    }

    IEnumerator cutInEffect()
    {
        Vector3 tmpPos = cutInObj.transform.position;
        float width = cutInObj.GetComponent<RectTransform>().rect.width;
        GameObject tmpOb = cutInObj.transform.GetChild(0).gameObject;

        cutInObj.SetActive(true);
        tmpOb.transform.position = new Vector3(tmpPos.x + width/2 ,tmpPos.y,tmpPos.z);

        yield return new WaitForSeconds(0.01f);

        iTween2.MoveTo(tmpOb, iTween2.Hash("x",tmpPos.x, "time", 0.5f));

        yield return new WaitForSeconds(0.95f);

        cutInObj.SetActive(false);
    }

    public void cutInEffectStart()
    {
        StartCoroutine("cutInEffect");
    }

    #endregion
    //-------------------------------------------------------------------------

    //겹쳤을때 이펙트 -> 송신확인 -> 하트가 스코어로
    /*
    public void effectSet(int _returnStamp,int _msgBoxidx)
    {
        //returnEffect.GetComponent<Image>().sprite = GameManager.instance.returnStamp[_returnStamp].GetComponent<Image>().sprite;
        returnEffect.GetComponent<Image>().sprite = test2;
        iTween2.FadeTo(returnEffect, iTween2.Hash("alpha",1f,"time",0f));
        returnEffect.transform.position = MsgMoveManager.instance.msgBox[_msgBoxidx].transform.position;

        //더미를 겹치고
        iTween2.FadeTo(returnEffect, iTween2.Hash("alpha", 0f,"delay",0.1f, "time", .2f));

        //메시지박스 이미지를 송신이미지로 바꾸고
        MsgMoveManager.instance.msgBox[_msgBoxidx].GetComponent<Image>().sprite = testImg;
        //송신이 사라지면서
        iTween2.FadeTo(MsgMoveManager.instance.msgBox[_msgBoxidx], iTween2.Hash("alpha", 0f, "delay", .3f,"time", .3f));

        setEffectPos(MsgMoveManager.instance.msgBox[_msgBoxidx].transform.position);


    }

    public IEnumerator effectSet(int _returnStamp, int _msgBoxidx)
    {
        MsgMoveManager.instance.msgBox[_msgBoxidx].GetComponent<Image>().sprite = kabuseta;

        yield return new WaitForSeconds(.1f);
        
        //메시지박스 이미지를 송신이미지로 바꾸고
        MsgMoveManager.instance.msgBox[_msgBoxidx].GetComponent<Image>().sprite = returnImg;
        //송신이 사라지면서
        iTween2.FadeTo(MsgMoveManager.instance.msgBox[_msgBoxidx], iTween2.Hash("alpha", 0f, "delay", .1f, "time", .3f));

        setEffectPos(MsgMoveManager.instance.msgBox[_msgBoxidx].transform.position);


    }
    
    void setEffectPos(Vector3 _position)
    {
        int x, y;
        //iTween2.MoveTo(effect.transform.GetChild(0).gameObject, iTween2.Hash("position", scoreTxt.transform, "time", .5f, "easetype", iTween2.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "OnAnimationDone"));
        for(int i = 0; i < 3; i++)
        {
            x = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
            y = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
            //effect.transform.GetChild(i).position = _position;
            effect.transform.GetChild(i).position = new Vector3(_position.x + (x* UnityEngine.Random.Range(MsgMoveManager.instance.msgWidth / 6, MsgMoveManager.instance.msgWidth/2)) ,_position.y + (y*UnityEngine.Random.Range(MsgMoveManager.instance.msgHeight / 6, MsgMoveManager.instance.msgHeight/2)),_position.z);

           // iTween2.FadeTo(effect.transform.GetChild(i).transform.gameObject, iTween2.Hash("alpha", 1f, "delay", 0f, "time", 0f));

            iTween2.StopByName(effect.transform.GetChild(i).gameObject, "punchScale");
            effect.transform.GetChild(i).transform.localScale = new Vector3(1f, 1f, 1f);
        }
        

        StartCoroutine("heartAnimation");
    }

    IEnumerator haertAnimation()
    {
        
        yield return null;

        iTween2.PunchScale(effect.transform.GetChild(0).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.4f, "name", "punchScale"));
        iTween2.PunchScale(effect.transform.GetChild(1).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.4f, "name", "punchScale"));
        iTween2.PunchScale(effect.transform.GetChild(2).gameObject, iTween2.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.4f, "name", "punchScale"));
        
        iTween2.MoveTo(effect.transform.GetChild(0).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f,"time", .3f, "easetype", iTween2.EaseType.linear));
        iTween2.MoveTo(effect.transform.GetChild(1).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f, "time", .3f, "easetype", iTween2.EaseType.easeInQuad));
        iTween2.MoveTo(effect.transform.GetChild(2).gameObject, iTween2.Hash("position", scoreTxt.transform.position, "delay", .2f, "time", .3f, "easetype", iTween2.EaseType.easeOutQuad));

    }

    */

}
