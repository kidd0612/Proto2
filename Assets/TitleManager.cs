using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;
public class TitleManager : SingleTon<TitleManager> {

    public GameObject[] panels;
    public Sprite[] storyTag;
    public Sprite[] scenarioTape;

    GameObject[] scenarioBtn = new GameObject[3];
    GameObject storyTitle;

    void Awake()
    {
        //PC resolution set
        Screen.SetResolution(360, 640, false);
    }
    // Use this for initialization
    void Start ()
    {
        scenarioBtnSet();
	}
	
    //퍼블릭으로 바꾸고 세팅해도 되는데 걍 코드로 함
    void scenarioBtnSet()
    {
        storyTitle = panels[2].transform.GetChild(1).gameObject;
        for (int i = 0; i < 3; i++)
        {
            scenarioBtn[i] = panels[2].transform.GetChild(2+i).gameObject;
        }
    }

    public void LoadScene()
    {
        Application.LoadLevel("ScenarioScene");
    }

    // 현재 페이지를 인덱스로 불러옴
    public void returnPage(int _num)
    {
        panels[_num].SetActive(false);
        panels[_num-1].SetActive(true);
    }

    //현재 페이지를 인덱스로 불러옴
    public void moveStoryPage()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(true);
    }

    public void moveScenarioPage(int _storyNum)
    {
        panels[1].SetActive(false);

        switch(_storyNum)
        {
            
            case 1:
                //타이틀 교체->이미지 교체 ->시나리오버튼 세팅
                storyTitle.GetComponentInChildren<Text>().text = "「人生の転機」";
                storyTitle.GetComponent<Image>().sprite = storyTag[0];
                for(int i =0; i < 3; i++)
                {
                    scenarioBtn[i].GetComponent<Image>().sprite = scenarioTape[0];
                }
                scenarioBtn[0].GetComponentInChildren<Text>().text = "「人生の転機」";
                scenarioBtn[1].GetComponentInChildren<Text>().text = "「幼馴染と先輩」";
                scenarioBtn[2].GetComponentInChildren<Text>().text = "「これがリア充」";
                break;
            case 2:
                storyTitle.GetComponentInChildren<Text>().text = "ストーリー2";
                storyTitle.GetComponent<Image>().sprite = storyTag[1];
                for (int i = 0; i < 3; i++)
                {
                    scenarioBtn[i].GetComponent<Image>().sprite = scenarioTape[1];
                }
                scenarioBtn[0].GetComponentInChildren<Text>().text = "シナリオa";
                scenarioBtn[1].GetComponentInChildren<Text>().text = "シナリオb";
                scenarioBtn[2].GetComponentInChildren<Text>().text = "シナリオc";
                break;
            case 3:
                storyTitle.GetComponentInChildren<Text>().text = "ストーリー3";
                storyTitle.GetComponent<Image>().sprite = storyTag[2];
                for (int i = 0; i < 3; i++)
                {
                    scenarioBtn[i].GetComponent<Image>().sprite = scenarioTape[2];
                }
                scenarioBtn[0].GetComponentInChildren<Text>().text = "シナリオa";
                scenarioBtn[1].GetComponentInChildren<Text>().text = "シナリオb";
                scenarioBtn[2].GetComponentInChildren<Text>().text = "シナリオc";
                break;
        }
        panels[2].SetActive(true);
    }
}
