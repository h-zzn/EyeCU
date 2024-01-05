using UnityEngine;
using System.IO;

public class SavingData : MonoBehaviour
{
    void Start()
    {
        CreateExcelFile();
    }

    void CreateExcelFile()
    {
        // Excel 파일 경로
        string filePath = "C:/Users/cglab/Downloads" + "/SampleExcel.xlsx";

        // 텍스트 데이터
        string data = "Name,Age\nJohn,25\nJane,30";

        // 파일 쓰기
        File.WriteAllText(filePath, data);

        Debug.Log("Excel file created at: " + filePath);
    }
}
