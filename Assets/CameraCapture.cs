using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class CameraCapture : MonoBehaviour
{
    private string csvFilePath;
    private List<string[]> data; // 데이터를 메모리에 저장할 리스트
    private int num;
    public float levelpoint;
    public int initialdiff;
    public float times = 0;

    private void Start()  
    {
        data = new List<string[]>(); // 데이터 리스트 초기화
        num = 1;
        data.Add(new string[] { "Tried", "Real Level", "Felt Level", "Playing time" });
        levelpoint = this.GetComponent<DDATrainer>().LevelPoint;
        initialdiff = this.GetComponent<DDATrainer>().initialDiff;
        csvFilePath = "C: //Users//my coms//Desktop//DDATest" + "/example.csv";

        // CaptureAndSendFrame을 코루틴으로 실행
        StartCoroutine(CaptureAndSendFrame());
    }

    private IEnumerator CaptureAndSendFrame()
    {
        while (true)
        {
            // 비동기로 프레임 캡처
            yield return new WaitForEndOfFrame();

            AddElement();

            // 특정 조건에 따라 파일에 데이터 쓰기
            if (true)
            {
                WriteListsToCSV();
            }

            // 2초 대기 (Thread.Sleep 대신)
            yield return new WaitForSeconds(0.5f);
        }
    }

    void AddElement()
    {
        times += 0.5f;
        string number = num.ToString();
        string levels = levelpoint.ToString("F1");
        string felt = initialdiff.ToString();
        string timed = times.ToString("F1");

        // 데이터를 리스트에 추가
        data.Add(new string[] { number, levels, felt, timed });

        num++;
    }

    void WriteListsToCSV()
    {
        // CSV 파일에 데이터 쓰기
        using (StreamWriter sw = new StreamWriter(csvFilePath, true))
        {
            foreach (var line in data)
            {
                // 한 행의 데이터를 CSV 형식으로 변환하여 파일에 쓰기
                sw.WriteLine(string.Join(",", line));
            }
        }

        // 데이터를 파일에 쓴 후에는 리스트를 비워준다.
        data.Clear();
    }
} 