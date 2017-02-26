using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class GameManager : SingleTon<GameManager> {
 
    public enum GameState
    {
        Ready,
        Play,
        Pause,
        Restart,
        Continue,
        GameOver,
        Max
    }
    public GameState GS;
    
    void SetState(GameState newState)
    {
        GS = newState;
    }
    
    float msgMoveTime = 1.5f;

    //------------------------------변수 선언부 
    
   // public GameObject[] stamps;
    public GameObject[] returnStamp;
    
    bool canDrag, dragging;
    

    //story (1~3)
    [System.NonSerialized]
    public int story = 3;
    int answer;
    int returnNum = -1;
    public int nokoriMsg;
    public int timeLimit = 30;
  
    Vector3 startPos;
    Vector3 imagePos;

    void Start()
    {

        Init();
    }

    //initialize
    void Init()
    {
        SetState(GameState.Play);
        canDrag = true;

        nokoriMsg = 100;
        GameUIManager.instance.InitUI();

        //젤마지막
        MsgMoveManager.instance.Init();
        
    }
    //-----------------------------리턴 스탬프관련---------------------------------------------

    public void pointDown(int _idx)
    {
        
        //박스의 이동이 끝나야 드래그 가능 
        if (canDrag)
        {
            dragging = true;
            //이동할 스탬프, 처음 위치, 처음 터치 위치 결정 
            imagePos = returnStamp[_idx].GetComponent<Image>().transform.position;
            startPos = Input.mousePosition;
            StartCoroutine(dragStamp(_idx));
            
            returnNum = _idx;
        }
    }

    IEnumerator dragStamp(int _idx)
    {
        do
        {
            yield return null;

            Vector3 movePos;

            //드래그된 거리만큼 이동 및 회전
            if (dragging)
            {
                Vector3 mouseVector = Input.mousePosition - startPos;
                Vector3 targetVector = returnStamp[_idx].transform.position - imagePos;
                
                movePos = new Vector3(mouseVector.x - targetVector.x, mouseVector.y - targetVector.y , 0);
            }
            else
            {
                movePos = Vector3.zero;
            }

            if (returnStamp[_idx] != null)
            {
                returnStamp[_idx].transform.Translate(movePos, Space.World);
            }
        } while (dragging);
    }

    
    

    public void pointUp(int _idx)
    {
        if (canDrag && dragging)
        {
            dragging = false;

            returnStamp[_idx].transform.position = imagePos;
            
            StartCoroutine(returnNumSet());
        }
    }
   
    //드래그 하다 취소하면 판정없애는 용도
    IEnumerator returnNumSet()
    {

        yield return new WaitForSeconds(0.02f);
        returnNum = -1;
      
    }

    //------------------------------------------------------------------------------------

    //드롭으로 교체

    public void pointDrop(int _idx)
    {
        Debug.Log("enter " + returnNum);
        
        
        //게임플레이중이고 리턴메시지를 선택했으면
        if (returnNum != -1 && GS == GameState.Play)
        {
            //정답이면
            if (MsgMoveManager.instance.msgBox[_idx].GetComponent<Image>().sprite == returnStamp[returnNum].GetComponent<Image>().sprite
                && MsgMoveManager.instance.msgBox[_idx].GetComponent<Image>().color.a != 0f)
            {
                                
                nokoriMsg -= 1;
                //UIㄴ
                GameUIManager.instance.setScoreUI();

                //타임 추가
                GameUIManager.instance.timeLimitPlusMinus(2);

                //이펙트
                GameUIManager.instance.startEffect(_idx);

                returnNum = -1;
            }
        }
        
    }

    //일제송신용
    public void returnAll()
    {

    }

    public void GameOver()
    {
        GS = GameState.GameOver;
        MsgMoveManager.instance.allMsgStop();
        GameUIManager.instance.activeGameOverUI();

    }
    
    
    public void Pause()
    {
        if (GS != GameState.Play)
            return;
        MsgMoveManager.instance.allMsgStop();
        SetState(GameState.Pause);
        GameUIManager.instance.pauseUI.SetActive(true);
    }

    public void Continue(int _time)
    {
        SetState(GameState.Continue);
        StartCoroutine(countDown(_time,GS));
    }

    public void Restart(int _time)
    {
        SetState(GameState.Restart);
        StartCoroutine(countDown(_time,GS));
    }

  
    public void returnTitle()
    {
        SetState(GameState.Ready);
        GameUIManager.instance.pauseUI.SetActive(false);
        Application.LoadLevel(0);
    }
    //
    IEnumerator countDown(int _count, GameState _gs)
    {

        GameUIManager.instance.pauseUI.SetActive(false);
        GameUIManager.instance.countDownTxt.gameObject.SetActive(true);

        do
        {
            GameUIManager.instance.countDownTxt.text = _count.ToString();
            _count--;
            yield return new WaitForSeconds(1f);

        }
        while (_count > 0);

        GameUIManager.instance.countDownTxt.gameObject.SetActive(false);

        switch(_gs)
        {
            case GameState.Restart:
                MsgMoveManager.instance.readyToRestart();
                Init();
                break;

            case GameState.Continue:
                SetState(GameState.Play);
                //StartCoroutine("moveMSG");
                MsgMoveManager.instance.allMsgStart();
                break;
        }
        
        
    }
}


