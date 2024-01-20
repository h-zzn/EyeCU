using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebSocketSharp;
using System.Linq;
using System.Collections.Generic;   

public class CameraCapture : MonoBehaviour
{
    private WebSocket ws;
    [SerializeField] private Camera mainCamera;
    private HashSet<string> receivedMessages = new HashSet<string>();
    private string csvFilePath;
    private List<string> tried;
    private List<string> level;
    private List<string> felt_level;
    private List<string> playing_time;
    private int num;
    public GameObject StageCore;
    private DDATrainer dDATrainer;
    public float levelpoint;
    public int initialdiff;
    public float times = 0;


    private void Start()
    {
        num = 1;

        csvFilePath = "C://Users//my coms//Desktop//DDATest" + "/example.csv";

        dDATrainer = StageCore.GetComponent<DDATrainer>();
        //WebSocket 연결을 Start에서 한 번만 설정
        ws = new WebSocket("ws://localhost:8764");
        ws.OnOpen += (sender, e) => Debug.Log("WebSocket connected");
        ws.Connect();
        tried = new List<string> { "Tried" };

    
        level = new List<string> { "Real Level"};  

    felt_level = new List<string> { //initialDiff  
        "Felt Level"
    };

    playing_time = new List<string>
    {
        "Playing time"
    };
    

    // CaptureAndSendFrame을 코루틴으로 실행
    StartCoroutine(CaptureAndSendFrame());

    InvokeRepeating("WriteListsToCSV", 0f, 0.5f);
    }

void WriteListsToCSV()
{
    // CSV 파일에 기록할 데이터
    List<string[]> data = new List<string[]>();

    // 두 개의 리스트를 하나의 행으로 만들기
    for (int i = 0; i < Mathf.Min(tried.Count, level.Count, felt_level.Count, playing_time.Count); i++)
    {
        string[] row = { tried[i], level[i], felt_level[i], playing_time[i] };
        data.Add(row);
    }

    // 2차원 배열로 변환
    string[][] dataArray = data.ToArray();

    // CSV 파일에 데이터 쓰기
    //WriteAllLines(dataArray);
}

void WriteAllLines(string[][] data)
{
    // CSV 파일에 데이터 쓰기
    using (StreamWriter sw = new StreamWriter(csvFilePath))
    {
        foreach (var line in data)
        {
            // 한 행의 데이터를 CSV 형식으로 변환하여 파일에 쓰기
            sw.WriteLine(string.Join(",", line));
        }
    }
}


private IEnumerator CaptureAndSendFrame()
{
    while (true)
    {
        // 비동기로 프레임 캡처
        yield return new WaitForEndOfFrame();

        // 프레임 캡처 및 전송
        CaptureAndSend();

        AddElement();

        // 2초 대기 (Thread.Sleep 대신)
        yield return new WaitForSeconds(.5f);
    }
}

private void CaptureAndSend()
{
    // 캡처 및 전송 로직
    RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
    mainCamera.targetTexture = rt;
    Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    mainCamera.Render();
    RenderTexture.active = rt;
    screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    mainCamera.targetTexture = null;
    RenderTexture.active = null;
    Destroy(rt);

    byte[] imageBytes = screenShot.EncodeToJPG(40);

    ws.Send(imageBytes);
}
    
    void AddElement()
    {
        string number = num.ToString();
        tried.Add(number);
        num++;
        
        string levels = dDATrainer.LevelPoint.ToString("F3");
        level.Add(levels);
        string felt = dDATrainer.initialDiff.ToString();
        felt_level.Add(felt);
        times = times + 0.5f;
        string timed = times.ToString("F1");
        playing_time.Add(timed);
        
    }
    
}