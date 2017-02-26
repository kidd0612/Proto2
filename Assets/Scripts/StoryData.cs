using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StoryData : ScriptableObject
{
    public List<Attribute> list = new List<Attribute>();

    //만든 데이터 형 밖으로 인스펙터창으로 내보내는 코드
    [System.Serializable]
    public class Attribute
    {
        //문자열로 Scene표시(Gameplay나오면 넘기기)
        public string scene;
        //캐릭터명
        public string character;
        //로그
        public string log;
        //폰트사이즈
        public float fontSize;
        //배경지정
        public string background;
       
    }
}
