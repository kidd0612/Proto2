using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class MsgMoveManager : SingleTon<MsgMoveManager>
{
    
    //각 레벨별 다이얼로그 패널(2인용 3인용 4인용)
    public GameObject[] dialogPanel;

    [System.NonSerialized]
    public List<GameObject> chatBox = new List<GameObject>();

    //몇번째 여자친구의 메시지를 움직일지 저장해놓는 리스트
    List<int> chatBoxIdx = new List<int>();
    //몇번째 여자친구를 분노모드 할지
    public List<int> rageModeIdx = new List<int>();
    //여자친구 몇초 분노모드 남았는지
    public List<int> rageRemainSec = new List<int>();

    [System.NonSerialized]
    public GameObject[] msgBox;
  

    //판정용 리스트
    public GameObject[] jDialogPanel;
    List<GameObject> jChatBox = new List<GameObject>();
    GameObject[] jMsgBox;

    Vector3[] msgPos;


    public Sprite[] msgImg;

    //메시지 가로 세로 길이 변수
    [System.NonSerialized]
    public float msgWidth;
    [System.NonSerialized]
    public float msgHeight;

    public int[] moveCount;
    bool rageMode = false;
    public bool rageModeEnd = true;

    //일제송신 
    bool returnAllChance = false;
    public Sprite returnAllImg;

    float msgMoveTime = 1.5f;
    float msgMovingTime = .5f;
    int level;


    #region about Initialize
    public void Init()
    {
        levelSet();
        chatBoxSet();
        msgBoxSet();
        msgPosSet();
        moveCountSet();

        //분노모드관련 초기화
        rageModeIdx.Clear();
        rageMode = false;
        rageModeEnd = true;
        //전체송신도 폴스상태로
        returnAllChance = false;

        allMsgStart();
    }

    public void readyToRestart()
    {
        for (int i = 0; i < chatBox.Count; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                //알파값도 투명하게
                iTween2.FadeTo(msgBox[(i * 7) + j], iTween2.Hash("alpha", 0f, "time", 0f));
                msgBox[(i * 7) + j].transform.position = msgPos[(i * 7) + j];
            }
        }
    }

    //2인 3인 4인용 레벨 세팅
    void levelSet()
    {
        level = GameManager.instance.story;
    }
    

    void moveCountSet()
    {
        moveCount = new int[chatBox.Count];

        for(int i = 0; i < chatBox.Count; i++)
        {
            moveCount[i] = 1;
        }
    }

    //Set TargetChatBox -- find dialogPanel's children and set
    void chatBoxSet()
    {
        chatBox.Clear();
        jChatBox.Clear();
        chatBoxIdx.Clear();
        
        for(int i = 0; i < dialogPanel[level-1].transform.childCount; i++)
        {
            chatBox.Add(dialogPanel[level-1].transform.GetChild(i).gameObject);
            chatBoxIdx.Add(i);
        }

        for (int i = 0; i < jDialogPanel[level-1].transform.childCount; i++)
        {
            jChatBox.Add(jDialogPanel[level-1].transform.GetChild(i).gameObject);
        }
    }

    void msgBoxSet()
    {
        msgBox = new GameObject[chatBox.Count*7];
        jMsgBox = new GameObject[jChatBox.Count*7];
        GameUIManager.instance.gfObjs = new GameObject[chatBox.Count];

        for(int i = 0; i < chatBox.Count; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                msgBox[(i * 7) + j] = chatBox[i].transform.GetChild(j).gameObject;
            }
            //gameui쪽 gfobj를 세팅해줌
            GameUIManager.instance.gfObjs[i] = chatBox[i].transform.GetChild(7).gameObject;
        }

        for (int i = 0; i < jChatBox.Count; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                jMsgBox[(i * 7) + j] = jChatBox[i].transform.GetChild(j).gameObject;
            }
        }
        

        //메시지박스의 높이 넓이값세팅
        msgWidth = msgBox[0].GetComponent<RectTransform>().rect.width;
        msgHeight = msgBox[0].GetComponent<RectTransform>().rect.height;
    }

    //SetMsgPosition
    void msgPosSet()
    {
        msgPos = new Vector3[chatBox.Count * 7];

        for(int i = 0; i < chatBox.Count; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                msgPos[(i * 7) + j] = msgBox[(i * 7) + j].transform.position;
            }
        }
    }
    //init func
    #endregion

    public void allMsgStop()
    {
        StopAllCoroutines();
    }

    public void allMsgStart()
    {
        
        /*
        for(int i = 0; i < chatBox.Count; i++)
        {
            StartCoroutine("moveMsgTimer");
        }
        */
        StartCoroutine("moveMsgTimer");
    }

    IEnumerator rageModeCutInWait()
    {
        //StopAllCoroutines();
        GameUIManager.instance.cutInEffectStart();
        yield return new WaitForSeconds(1f);
        rageModeStart();
    }
    void rageModeStart()
    {
        StartCoroutine("rageModeTimer");
        StartCoroutine("moveMsgTimer");
    }
    //컷ㅎ인을 바로 실행시키면 stopAllCoroutine이 비정상적으로 작동하므려 따로 함수 뺴둠
    //틀리면 발동하게 수정
    void readyRageMode()
    {
        StopAllCoroutines();
        StartCoroutine("rageModeCutInWait");
    }

    //틀렸을때 불러오는 함수
    public void setRageMode(int _idx)
    {
        rageMode = true;

        rageModeIdx.Add(_idx);
        chatBoxIdx.Remove(_idx);
        
        //분노모드가 끝나지 않음을 알리는 불변수
        rageModeEnd = false;

        readyRageMode();
    }

    #region // move func
    IEnumerator moveMsgTimer()
    {
     
        do
        {
            /*
            //분노모드이면
            if (rageMode)
            {
                
                readyRageMode();
                //rageModeStart();
            }
            else
                moveMsg(chatBoxIdx);
                */
            moveMsg(chatBoxIdx);

            /*
            //문제 출제
            makeQuestion(_id);

            for (int i = 0; i < 5; i++)
            {

                iTween2.MoveTo(msgBox[((moveCount[_id] + i) % 7) + (_id * 7)], iTween2.Hash("y", msgPos[(_id * 7) + i].y, "time", msgMovingTime, "name", "moveTag"));
                iTween2.MoveTo(jMsgBox[((moveCount[_id] + i) % 7) + (_id * 7)], iTween2.Hash("y", msgPos[(_id * 7) + i].y, "time", msgMovingTime, "name", "moveTag"));

            }
            //맨 밑 페이드인
            if (UnityEngine.Random.Range(0,100) > 50)
            {
                iTween2.FadeTo(msgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("alpha", 1f, "time", 0f));
            }
            iTween2.MoveTo(msgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 5].y, "time", msgMovingTime, "name", "moveTag"));
            iTween2.MoveTo(jMsgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 5].y, "time", msgMovingTime, "name", "moveTag"));



            //맨위 페이드 아웃하기전에 판정 -movecount 1이상일때만 판정 (리스타트시 실행되는거 방지)
            if(msgBox[(_id * 7) + ((moveCount[_id]) % 7)].GetComponent<Image>().color.a != 0 && moveCount[_id] > 1)
            {
                StartCoroutine(GameUIManager.instance.lifeGaugeMinus());
            }

            //맨위 페이드아웃
            iTween2.FadeTo(msgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("alpha", 0f, "time", 0f));

            iTween2.MoveTo(msgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 6].y, "time", msgMovingTime, "name", "moveTag"));
            iTween2.MoveTo(jMsgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 6].y, "time", msgMovingTime, "name", "moveTag"));


            //checkDeadLine(_id);
            
            //무브 카운트 증가
            moveCount[_id]++;
            */
            yield return new WaitForSeconds(msgMoveTime);

            /*
            //*****분노모드는 4인일때만 발동하게한들어야함
            //분노모드가아니고 4인이상일때
            if (rageModeEnd && chatBoxIdx.Count > 3)
            {
                //10프로 확률로 rage모드
                rageMode = UnityEngine.Random.Range(1, 101) % 5 == 0 ? true : false;

            }
            */

        } while (true);
        
        
    }


    //실제 무브시키는 함수

    void moveMsg(List<int> _list)
    {
        float _movingTime;
        
        //분노모드일때 아닐때 시간 세팅
        if (rageMode)
        {
            _movingTime = msgMovingTime / 3f;
        }
        else
        {
            _movingTime = msgMovingTime;
        }
        

        //움직이는 함수
        for (int j = 0; j < _list.Count; j++)
        {
            int _id = _list[j];
           

            for (int i = 0; i < 5; i++)
            {

                iTween2.MoveTo(msgBox[((moveCount[_id] + i) % 7) + (_id * 7)], iTween2.Hash("y", msgPos[(_id * 7) + i].y, "time", _movingTime, "name", "moveTag"));
                iTween2.MoveTo(jMsgBox[((moveCount[_id] + i) % 7) + (_id * 7)], iTween2.Hash("y", msgPos[(_id * 7) + i].y, "time", _movingTime, "name", "moveTag"));

            }
            //맨 밑 페이드인 (50%확률로 생성된 문제 보여주기)
            if (UnityEngine.Random.Range(0, 100) > 50)
            {
                if (!returnAllChance && UnityEngine.Random.Range(0, 100) < 10 ? true : false)
                {
                    //20퍼확률로 전부송신 버튼 출현 --- 50%의 10%확률이므로 각캐릭당 5% 전체적으로 20퍼?
                    returnAllChance = true;
                    //전체송신 버튼으로 이미지 바꿈
                    msgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)].GetComponent<Image>().sprite = returnAllImg;
                  
                    //*****************************************************분노모드 들어가면 모든 코루틴이 꺼져서 이것도꺼짐
                   // StartCoroutine("returnAllChanceTimer");

                }
                else
                {
                    makeQuestion(_id);
                }
                iTween2.FadeTo(msgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("alpha", 1f, "time", 0f));
            }
            iTween2.MoveTo(msgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 5].y, "time", _movingTime, "name", "moveTag"));
            iTween2.MoveTo(jMsgBox[(_id * 7) + ((moveCount[_id] + 5) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 5].y, "time", _movingTime, "name", "moveTag"));



            //맨위 페이드 아웃하기전에 판정 -movecount 1이상일때만 판정 (리스타트시 실행되는거 방지)
            if (msgBox[(_id * 7) + ((moveCount[_id]) % 7)].GetComponent<Image>().color.a != 0 && moveCount[_id] > 1)
            {
                //StartCoroutine(GameUIManager.instance.lifeGaugeMinus());
                GameUIManager.instance.timeLimitPlusMinus(-1);
            }

            //맨위 페이드아웃
            //iTween2.FadeTo(msgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("alpha", 0f, "time", 0f));
            //맨위로 올라가면서 딜레이0.1주고 페이드아웃
            iTween2.FadeTo(msgBox[(_id * 7) + ((moveCount[_id] ) % 7)], iTween2.Hash("alpha", 0f, "time", 0f,"delay",0.1f));

            iTween2.MoveTo(msgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 6].y, "time", _movingTime, "name", "moveTag"));
            iTween2.MoveTo(jMsgBox[(_id * 7) + ((moveCount[_id] + 6) % 7)], iTween2.Hash("y", msgPos[(_id * 7) + 6].y, "time", _movingTime, "name", "moveTag"));

            //무브카운트 증가
            moveCount[_id]++;
        }

    }

    IEnumerator rageModeTimer()
    {
        rageMode = false;

        //분노모드 이펙트
        for (int i = 0; i < rageModeIdx.Count; i++)
        {
            GameUIManager.instance.rageModEffect(rageModeIdx[i]);
        }

        //일단 단순계산으로 10번 돌리기
        for (int i = 0; i < 10; i++)
        {
            moveMsg(rageModeIdx);

            yield return new WaitForSeconds(msgMoveTime / 3f);
        }
       
        //분노모드 끝난뒤 조정
        chatBoxIdx.Insert(rageModeIdx[0], rageModeIdx[0]);

        //분노모드 이펙트끔
        for (int i = 0; i < rageModeIdx.Count; i++)
        {
            GameUIManager.instance.rageModEffectEnd(rageModeIdx[i]);
        }

        rageModeIdx.Clear();
        
        rageModeEnd = true;

        //다시 메시지 시작
       // allMsgStart();

    }

    #endregion
    //**************************************일제송신 안누르고 그냥 내버려뒀을시 화면밖으로 나간후 false로 바꿔주는 설정도 해야함
    //call back Func 
    //일제송신용 -버튼다운 내지는 클릭
    public void returnAll(int _idx)
    {
        if (msgBox[_idx].GetComponent<Image>().sprite == returnAllImg)
        {
            StopCoroutine("returnAllChanceTimer");
            //일단 자기꺼 지우고
            iTween2.FadeTo(msgBox[_idx], iTween2.Hash("alpha", 0f, "time", 0f));
            //전부 지우기 & 스코어 가산 -----제일 위에있는 박스는 대상외
            for (int i = 0; i < chatBox.Count; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    //메시지박스가 투명하지 않고 젤위에있는 박스가 아니면
                    if (msgBox[(i * 7) + j].GetComponent<Image>().color.a != 0)
                    {
                        iTween2.FadeTo(msgBox[(i * 7) + j], iTween2.Hash("alpha", 0f, "time", 0f));

                        //남은 메시지 1개씩줄임
                        GameManager.instance.nokoriMsg -= 1;
                        //타임리밋2초 추가
                        GameUIManager.instance.timeLimitPlusMinus(2);
                        //스코어 세팅
                        GameUIManager.instance.setScoreUI();
                    }

                }

            }

            //일제 송신 버튼 생성가능한 상태로 만들어줌
            returnAllChance = false;
        }
        else
            return;
    }

    //********************************************분노모드 나오면 이것도꺼져서 다시 세팅해야함
    //------------------------메시지 움직이는 함수에서 페이드 아웃하기전에 이미지 확인해서 false로 만드는걸로해도 될듯
    //----------------------조건 ( 이미지가 일제송신이고 이미지가 불투명하면(알파값이 1이면) 
    IEnumerator returnAllChanceTimer()
    {
        //일제송신 클릭안했을시 10초후에 false로 만듬
        yield return new WaitForSeconds(10f);
        returnAllChance = false;
    }
    //**************************
    void makeQuestion(int _idx)
    {
        int tmp = UnityEngine.Random.Range(0, 3);// GameManager.instance.returnStamp.Length);
      
        msgBox[(_idx * 7) + ((moveCount[_idx] + 5) % 7)].GetComponent<Image>().sprite = GameManager.instance.returnStamp[tmp].transform.GetChild(0).GetComponent<Image>().sprite;
        
    }


}
