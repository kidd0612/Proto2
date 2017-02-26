using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

/// <summary>
/// 유니티에서 지원하는 Assetpostprocessor를 이용해서 
///엑셀 파일을 ScriptableObject로 변경하는 스크립트
/// </summary>
public class ImportStoryExel : AssetPostprocessor
{
    static readonly string filePath = "Assets/Editor/Data/Story.xlsx";
    static readonly string storyExportPath = "Assets/Resources/Data/StoryData.asset";
  //  static readonly string enemeyExportPath = "Assets/Resources/Data/EnemyLevelData.asset";

    /// <summary>
    /// 에셋이 유니티 엔진에 추가되면 실행되는 함수
    /// RPGData.xlsx 파일이 filePath 폴더에 추가되면 레벨 데이터를 생성한다.
    /// </summary>
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 임포트 된 모든 파일을 검색.
        foreach (string s in importedAssets)
        {
            // 임포트 된 경로가 filePath인 경우 레벨 데이터 생성.
            if (s == filePath)
            {
                Debug.Log("Excel data convert start");
                CreateStoryData();
                Debug.Log("Excel data convert complete");
            }
        }
    }

    /// <summary>
    /// 유니티 에디터에 메뉴를 추가해서 해당 메뉴를 선택하면 레벨데이터를 생성한다.
    /// </summary>
    [MenuItem("Assets/Create Story Data")]
    static void CreateStoryData()
    {
        Debug.Log("Excel data convert start");
        MakeStoryData();
       // MakeEnemyData();
        Debug.Log("Excel data convert complete");
    }

    /// <summary>
    /// 주인공 정보를 ScriptableObject로 생성
    /// </summary>
    static void MakeStoryData()
    {
        // ScriptableObejct 인스턴스 생성
        StoryData data = ScriptableObject.CreateInstance<StoryData>();
        // ScriptableObject를 파일로 생성
        AssetDatabase.CreateAsset((ScriptableObject)data, storyExportPath);
        // 생성된 파일을 에디터에서 수정하지 못하게 설정.
        data.hideFlags = HideFlags.NotEditable;
        // 데이터 초기화.
        data.list.Clear();

        // 엑셀 파일 Open
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // open된 엑셀 파일을 메모리에 생성.
            IWorkbook book = new XSSFWorkbook(stream);
            // 두번째  Sheet 열기.
            ISheet sheet = book.GetSheetAt(1);

            // 2번째 줄(row)부터 읽기.
            for (int ix = 1; ix <= sheet.LastRowNum; ++ix)
            {
                // 줄 (row) 읽기.
                IRow row = sheet.GetRow(ix);
                // Serialization을 위해서 임시 객체 생성
                StoryData.Attribute a = new StoryData.Attribute();

                // 각 데이터 항목 설정.
               // a.scene = row.GetCell(0).StringCellValue;
                a.character = (string)row.GetCell(1).StringCellValue;
                a.log = (string)row.GetCell(2).StringCellValue;
                a.fontSize = (float)row.GetCell(3).NumericCellValue;
            //    a.background = (string)row.GetCell(4).StringCellValue;
                // 리스트에 정보 추가
                data.list.Add(a);
            }

            stream.Close();
        }

        // 위에서 생성한 ScriptableObject 파일 찾기.
        ScriptableObject obj = AssetDatabase.LoadAssetAtPath(storyExportPath, typeof(ScriptableObject)) as ScriptableObject;
        // 디스크에 쓰기.
        EditorUtility.SetDirty(obj);
    }

    /*
    /// <summary>
    /// 적 정보를 ScriptableObject로 생성
    /// </summary>
    static void MakeEnemyData()
    {
        // ScriptableObejct 인스턴스 생성
        EnemyLevelData data = ScriptableObject.CreateInstance<EnemyLevelData>();
        // ScriptableObject를 파일로 생성
        AssetDatabase.CreateAsset((ScriptableObject)data, enemeyExportPath);
        // 생성된 파일을 에디터에서 수정하지 못하게 설정.
        data.hideFlags = HideFlags.NotEditable;
        // 데이터 초기화.
        data.enemyInfos.Clear();

        // 엑셀 파일 열기
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // Open된 엑셀 파일을 메모리에 생성.
            IWorkbook book = new XSSFWorkbook(stream);

            for (int jx = 2; jx < 5; ++jx)
            {
                ISheet sheet = book.GetSheetAt(jx);
                EnemyLevelData.Race enemyInfo = new EnemyLevelData.Race();
                enemyInfo.name = sheet.SheetName.ToString();

                // 3번째 줄(row)부터 읽기
                for (int ix = 2; ix <= sheet.LastRowNum; ++ix)
                {
                    // 줄 읽기
                    IRow row = sheet.GetRow(ix);
                    // Serialization을 위해서 임시 객체 생성.
                    EnemyLevelData.Attribute a = new EnemyLevelData.Attribute();

                    a.levelName = "Level " + (ix - 1);
                    // 각 레벨 데이터 항목 설정.
                    a.level = (int)row.GetCell(0).NumericCellValue;
                    a.maxHP = (int)row.GetCell(1).NumericCellValue;
                    a.attack = (int)row.GetCell(2).NumericCellValue;
                    a.defence = (int)row.GetCell(3).NumericCellValue;
                    a.gainExp = (int)row.GetCell(4).NumericCellValue;
                    a.walkSpeed = (int)row.GetCell(5).NumericCellValue;
                    a.runSpeed = (int)row.GetCell(6).NumericCellValue;
                    a.turnSpeed = (int)row.GetCell(7).NumericCellValue;
                    a.attackRange = (float)row.GetCell(8).NumericCellValue;
                    a.gainGold = (int)row.GetCell(9).NumericCellValue;

                    // 리스트에 정보 추가.
                    enemyInfo.list.Add(a);
                }

                data.enemyInfos.Add(enemyInfo);
            }

            stream.Close();
        }

        // 위에서 생성한 ScriptableObject 파일 찾기.
        ScriptableObject obj = AssetDatabase.LoadAssetAtPath(enemeyExportPath, typeof(ScriptableObject)) as ScriptableObject;
        // 디스크에 쓰기.
        EditorUtility.SetDirty(obj);
    }
    */
}